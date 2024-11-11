using System;
using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Collection;
using Cr7Sund.FrameWork.Util;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Cr7Sund.AssetLoader.Impl
{

    public class AddressableSceneLoader : ISceneLoader
    {

        private Dictionary<IAssetKey, AsyncChainOperations<SceneInstance>> _assetKeyToHandles
            = new();
        private List<TupleStruct<IAssetKey, PromiseTaskSource>> _activeScenePromise = new();
        public bool IsInit { get; set; }
        private readonly Action<AsyncChainOperations<SceneInstance>> _onUnloadedAction = OnUnloadHandle;

        public void Init()
        {
            if (!IsInit)
            {
                IsInit = true;
            }
        }

        public void Dispose()
        {
            IsInit = false;
        }

        public async PromiseTask LoadSceneAsync(IAssetKey assetKey,
            LoadSceneMode loadMode, bool activateOnLoad, UnsafeCancellationToken cancellation)
        {
            if (_assetKeyToHandles.TryGetValue(assetKey, out var chainOperation))
            {
                await chainOperation.Chain();
                return;
            }
            var sceneKey = assetKey.Key;

            var handler = Addressables.LoadSceneAsync(sceneKey, loadMode, activateOnLoad);
            // var handler = SceneManager.LoadSceneAsync(sceneKey, loadMode);
            // handler.allowSceneActivation = activateOnLoad;
            
            //PLAN : Compare with Cancel Load and cancel instantite
            
            chainOperation = AsyncChainOperations<SceneInstance>.Start(ref handler, assetKey.Key, cancellation);
            _assetKeyToHandles.Add(assetKey, chainOperation);
            RegisterCancel(assetKey, cancellation);

            // await addressableHandle.Task;
            await chainOperation.Chain();
        }

        public async PromiseTask UnloadScene(IAssetKey assetKey)
        {
            if (!_assetKeyToHandles.ContainsKey(assetKey))
            {
                return;
                // throw new MyException($"not loaded scene {key}");
            }

            var asyncChainOperation = _assetKeyToHandles[assetKey];
            _assetKeyToHandles.Remove(assetKey);

            await asyncChainOperation.Chain();
            asyncChainOperation.RegisterUnload(_onUnloadedAction);
        }

        public async PromiseTask ActiveSceneAsync(IAssetKey key)
        {
            if (!_assetKeyToHandles.ContainsKey(key))
            {
                // throw new MyException($"not loaded scene {key}");
                return;
            }

            if (!_assetKeyToHandles[key].Handler.IsValid())
            {
                Console.Warn("try to active an invalid scene");
                return;
            }

            if (!_assetKeyToHandles[key].Handler.IsDone)
            {
                throw new MyException(AssetLoaderExceptionType.ACTIVE_UNLOADED_SCENE);
            }

            await ActiveAsync(key);
        }

        public void LateUpdate(int milliseconds)
        {
            for (int i = _activeScenePromise.Count - 1; i >= 0; i--)
            {
                var item = _activeScenePromise[i];
                AsyncChainOperations<SceneInstance> asyncChainOperations = _assetKeyToHandles[item.Item1];
                if (asyncChainOperations.Handler.IsDone &&
                    asyncChainOperations.GetResult<SceneInstance>().Scene.isLoaded)
                {
                    item.Item2.TryResolve();
                    _activeScenePromise.RemoveAt(i);
                }
            }
        }

        private static void OnUnloadHandle(AsyncChainOperations<SceneInstance> asyncChain)
        {
            // since unload scene automatically when switch scene
            if (asyncChain.Handler.IsValid())
            {
                // we need to resolve the loading promise by active the scene 
                var sceneInstance = asyncChain.GetResult<SceneInstance>();
                if (!sceneInstance.Scene.isLoaded)
                {
                    sceneInstance.ActivateAsync();
                }

                if (SceneManager.sceneCount > 1)
                {
                    //Unloading the last loaded scene is not supported
                    try
                    {
                        Addressables.UnloadSceneAsync(asyncChain.Handler);
                    }
                    catch (System.Exception ex)
                    {
                        Console.Error(ex);
                    }
                }
            }

            asyncChain.TryReturn();
        }

        private async PromiseTask ActiveAsync(IAssetKey assetKey)
        {
            var sceneInstance = _assetKeyToHandles[assetKey].GetResult<SceneInstance>();
            sceneInstance.ActivateAsync();

            if (_assetKeyToHandles[assetKey].GetResult<SceneInstance>().Scene.isLoaded)
            {
                return;
            }
            var activePromise = PromiseTaskSource.Create();
            _activeScenePromise.Add(new TupleStruct<IAssetKey, PromiseTaskSource>(assetKey, activePromise));
            await activePromise.Task;
        }

        private void RegisterCancel(IAssetKey assetKey, UnsafeCancellationToken cancellation)
        {
            if (cancellation.IsCancellationRequested)
            {
                OnCancel(assetKey, cancellation);
            }
            else
            {
                cancellation.Register(() => OnCancel(assetKey, cancellation));
            }
        }

        private void OnCancel(IAssetKey assetKey, UnsafeCancellationToken cancellation)
        {
            // unload win
            if (!_assetKeyToHandles.ContainsKey(assetKey))
            {
                return;
            }

            AsyncChainOperations<SceneInstance> chainOperation = _assetKeyToHandles[assetKey];

            //AsyncInstantiateOperation.Cancel
            chainOperation.RegisterCancel(assetKey.Key, cancellation);
            chainOperation.RegisterUnload(_onUnloadedAction);
            _assetKeyToHandles.Remove(assetKey);
            for (int i = _activeScenePromise.Count - 1; i >= 0; i--)
            {
                if (_activeScenePromise[i].Item1 == assetKey)
                {
                    _activeScenePromise.RemoveAt(i);
                }
            }
        }
    }
}
