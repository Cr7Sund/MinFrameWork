using System;
using System.Collections.Generic;
using System.Threading;
using Cr7Sund.FrameWork.Util;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Impl
{

    internal class AddressableSceneLoader : ISceneLoader
    {
        protected Dictionary<IAssetKey, AsynChainOperations> _assetKeyToHandles
            = new();

        public async PromiseTask LoadSceneAsync(IAssetKey key,
            LoadSceneMode loadMode, bool activateOnLoad)
        {
            if (_assetKeyToHandles.TryGetValue(key, out var setter))
            {
                await setter.Chain();
                return;
            }
            var sceneKey = key.Key;

            AsyncOperationHandle handler = Addressables.LoadSceneAsync(sceneKey, loadMode, activateOnLoad);
            setter = AsynChainOperations.Start(ref handler);
            _assetKeyToHandles.Add(key, setter);

            // await addressableHandle.Task;
            await setter.Chain();
        }

        public void UnloadScene(IAssetKey key)
        {
            if (!_assetKeyToHandles.ContainsKey(key))
            {
                throw new MyException($"not loaded scene {key}");
            }

            _assetKeyToHandles[key].Unload(OnUnloadHandle);
            _assetKeyToHandles.Remove(key);
        }

        private static void OnUnloadHandle(AsynChainOperations asynChain)
        {
            if (SceneManager.sceneCount > 1)
            {
                //Unloading the last loaded scene is not supported
                Addressables.UnloadSceneAsync(asynChain.Handler);
            }
            asynChain.TryReturn();
        }

        public async PromiseTask ActiveSceneAsync(IAssetKey key)
        {
            if (!_assetKeyToHandles.ContainsKey(key))
            {
                // currently dont support duplicate scene load 
                throw new MyException($"not loaded scene {key}");
            }

            if (_assetKeyToHandles[key].Handler.IsValid() && !_assetKeyToHandles[key].Handler.IsDone)
            {
                await _assetKeyToHandles[key].Chain();
                return;
            }

            ActiveAsync(key);
        }

        public void RegisterCancelLoad(IAssetKey assetKey, CancellationToken cancellation)
        {
            throw new NotImplementedException();
            var key = assetKey.Key;

            if (!_assetKeyToHandles.ContainsKey(assetKey))
            {
                throw new MyException(
                    $"There is no asset that has been requested for release (Asset:{key}).");
            }

            var handler = _assetKeyToHandles[assetKey];
            cancellation.Register(() =>
            {
                // UnloadScene(assetKey);
                handler.Cancel(cancellation);
            });
        }

        private void ActiveAsync(IAssetKey key)
        {
            var sceneInstance = _assetKeyToHandles[key].GetResult<SceneInstance>();
            sceneInstance.ActivateAsync();
        }


    }
}
