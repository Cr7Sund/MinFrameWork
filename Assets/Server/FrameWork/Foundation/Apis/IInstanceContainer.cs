using System;
using Cr7Sund.AssetLoader.Api;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Apis
{
    public interface IInstanceContainer : IDisposable
    {
        public GameObject CreateInstance(string name, params Type[] components);
        public GameObject CreateInstance(string name, IAssetPromise assetPromise);
        public T CreateInstanceWithComponent<T>(string name) where T : Object;
        public GameObject LoadInstance(IAssetKey assetKey, string name);
        IAssetPromise LoadInstanceAsync(IAssetKey assetKey, string name);
        void ReturnInstance(string name);
    }
}