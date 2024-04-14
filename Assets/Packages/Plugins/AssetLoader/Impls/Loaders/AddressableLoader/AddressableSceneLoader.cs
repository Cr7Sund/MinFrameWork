using System.Collections.Generic;
using Cr7Sund.FrameWork.Util;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Impl
{
    public class AddressableSceneLoader : ISceneLoader
    {
        protected Dictionary<IAssetKey, AsyncOperationHandle<SceneInstance>> _assetKeyToHandles
            = new Dictionary<IAssetKey, AsyncOperationHandle<SceneInstance>>();


        public async PromiseTask LoadSceneAsync(IAssetKey key,
            LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            if (_assetKeyToHandles.ContainsKey(key))
            {
                // currently dont support duplicate scene load
                throw new MyException($"already load scene {key}");
            }

            var asyncOperation = Addressables.LoadSceneAsync(key.Key, loadMode, activateOnLoad);
            _assetKeyToHandles.Add(key, asyncOperation);
            await asyncOperation.Task;
        }

        public void UnloadScene(IAssetKey key)
        {
            if (!_assetKeyToHandles.ContainsKey(key))
            {
                // currently dont support duplicate scene load
                throw new MyException($"not loaded scene {key}");
            }

            Addressables.UnloadSceneAsync(_assetKeyToHandles[key]);
            _assetKeyToHandles.Remove(key);
        }
        public async PromiseTask ActiveSceneAsync(IAssetKey key)
        {
            if (!_assetKeyToHandles.ContainsKey(key))
            {
                // currently dont support duplicate scene load 
                throw new MyException($"not loaded scene {key}");
            }


            if (!_assetKeyToHandles[key].IsDone)
            {
                await _assetKeyToHandles[key].Task;
                ActiveAsync(key);
            }
            else
            {
                ActiveAsync(key);
            }
        }

        private void ActiveAsync(IAssetKey key)
        {
            if (!_assetKeyToHandles[key].IsValid()) return;

            _assetKeyToHandles[key].Result.ActivateAsync();
        }
    }
}
