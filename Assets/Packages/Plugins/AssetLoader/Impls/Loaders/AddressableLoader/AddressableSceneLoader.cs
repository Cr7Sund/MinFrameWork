using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.PackageTest.Util;
using Cr7Sund.NodeTree.Api;
using UnityEngine;
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

 
        public IPromise LoadSceneAsync(IAssetKey key,
            LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {

            if (_assetKeyToHandles.ContainsKey(key))
            {
                // currently dont support duplicate scene load
                throw new MyException($"already load scene {key}");
            }

            var resultPromise = new Promise();
            var asyncOperation = Addressables.LoadSceneAsync(key.Key, loadMode, activateOnLoad);
            _assetKeyToHandles.Add(key, asyncOperation);
            asyncOperation.Completed += handler =>
            {
                if (handler.Status == AsyncOperationStatus.Succeeded)
                {
                    resultPromise.Resolve();
                }
                else if (handler.Status == AsyncOperationStatus.Failed)
                {
                    resultPromise.Reject(handler.OperationException);
                }
            };
            return resultPromise;
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
        public IPromise ActiveSceneAsync(IAssetKey key)
        {
            if (!_assetKeyToHandles.ContainsKey(key))
            {
                // currently dont support duplicate scene load 
                throw new MyException($"not loaded scene {key}");
            }

            var resultPromise = new Promise();

            if (!_assetKeyToHandles[key].IsDone)
            {
                _assetKeyToHandles[key].Completed += _ => ActiveAsync(resultPromise, key);
            }
            else
            {
                ActiveAsync(resultPromise, key);
            }

            return resultPromise;
        }
        private void ActiveAsync(Promise resultPromise, IAssetKey key)
        {
            if (!_assetKeyToHandles[key].IsValid()) return;

            _assetKeyToHandles[key].Result.ActivateAsync().completed += handler =>
            {
                if (handler.isDone)
                {
                    resultPromise.Resolve();
                }
                else
                {
                    // exception happen in Active scene
                    resultPromise.Reject(new MyException(""));
                }
            };
        }
    }
}
