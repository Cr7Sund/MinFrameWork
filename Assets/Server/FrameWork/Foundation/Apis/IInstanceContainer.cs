using System;
using Cr7Sund.AssetLoader.Api;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Apis
{
    public interface IInstanceContainer : IDisposable
    {
        public GameObject Create(string name, params Type[] components);
        public ComponentT Create<ComponentT>(string name) where ComponentT : Object;
        public IAssetPromise Instantiate(IAssetKey assetKey, string name) ;
        public IAssetPromise InstantiateAsync(IAssetKey assetKey, string name) ;
        public GameObject GetInstance(IAssetKey assetKey, string name);

        void ReturnInstance(string name);
        void ReturnInstance(string name, IAssetKey assetKey);
    }
}