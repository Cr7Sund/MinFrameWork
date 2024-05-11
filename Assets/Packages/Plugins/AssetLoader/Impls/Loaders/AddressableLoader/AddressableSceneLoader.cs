using System;
using System.Collections.Generic;
using System.Threading;
using Cr7Sund.Collection;
using Cr7Sund.FrameWork.Util;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Impl
{

    internal class AddressableSceneLoader : ISceneLoader
    {

        private Dictionary<IAssetKey, AsyncChainOperations> _assetKeyToHandles
            = new();
        private List<TupleStruct<IAssetKey, PromiseTaskSource>> _activeScenePromise = new();
        public bool IsInit { get; set; }

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

        public async PromiseTask LoadSceneAsync(IAssetKey key,
            LoadSceneMode loadMode, bool activateOnLoad, CancellationToken cancellation)
        {
            if (_assetKeyToHandles.TryGetValue(key, out var chainOperation))
            {
                await chainOperation.Chain();
                return;
            }
            var sceneKey = key.Key;

            AsyncOperationHandle handler = Addressables.LoadSceneAsync(sceneKey, loadMode, activateOnLoad);
            chainOperation = AsyncChainOperations.Start(ref handler, cancellation);

            _assetKeyToHandles.Add(key, chainOperation);

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

            await _assetKeyToHandles[assetKey].Chain();
            if (_assetKeyToHandles[assetKey].Handler.IsValid())
            {
                var sceneInstance = _assetKeyToHandles[assetKey].GetResult<SceneInstance>();
                if (!sceneInstance.Scene.isLoaded)
                {
                    await ActiveSceneAsync(assetKey);
                }
            }

            _assetKeyToHandles[assetKey].Unload(OnUnloadHandle);
            _assetKeyToHandles.Remove(assetKey);
        }

        public async PromiseTask ActiveSceneAsync(IAssetKey key)
        {
            if (!_assetKeyToHandles.ContainsKey(key))
            {
                // currently dont support duplicate scene load 
                // throw new MyException($"not loaded scene {key}");
                return;
            }

            if (_assetKeyToHandles[key].Handler.IsValid() && !_assetKeyToHandles[key].Handler.IsDone)
            {
                await _assetKeyToHandles[key].Chain();
                return;
            }

            await ActiveAsync(key);
        }

        public async PromiseTask RegisterCancelLoad(IAssetKey assetKey, CancellationToken cancellation)
        {
            var key = assetKey.Key;

            if (!_assetKeyToHandles.ContainsKey(assetKey))
            {
                throw new MyException(
                    $"There is no asset that has been requested for release (Asset:{key}).");
            }

            var chainOperation = _assetKeyToHandles[assetKey];

            await chainOperation.Chain();
            await UnloadScene(assetKey);
        }

        public void LateUpdate(int milliseconds)
        {
            for (int i = _activeScenePromise.Count - 1; i >= 0; i--)
            {
                var item = _activeScenePromise[i];
                AsyncChainOperations asyncChainOperations = _assetKeyToHandles[item.Item1];
                if (asyncChainOperations.Handler.IsDone &&
                    asyncChainOperations.GetResult<SceneInstance>().Scene.isLoaded)
                {
                    item.Item2.Resolve();
                    _activeScenePromise.RemoveAt(i);
                }
            }
        }
        private static void OnUnloadHandle(AsyncChainOperations asinChain)
        {
            if (SceneManager.sceneCount > 1)
            {
                //Unloading the last loaded scene is not supported
                Addressables.UnloadSceneAsync(asinChain.Handler);
            }
            asinChain.TryReturn();
        }

        private async PromiseTask ActiveAsync(IAssetKey assetKey)
        {
            var sceneInstance = _assetKeyToHandles[assetKey].GetResult<SceneInstance>();
            sceneInstance.ActivateAsync();

            if (_assetKeyToHandles[assetKey].GetResult<SceneInstance>().Scene.isLoaded)
            {
                return;
            }
            else
            {
                var activePromise = PromiseTaskSource.Create();
                _activeScenePromise.Add(new TupleStruct<IAssetKey, PromiseTaskSource>(assetKey, activePromise));
                await new PromiseTask(activePromise, 0);
            }
        }
    }
}
