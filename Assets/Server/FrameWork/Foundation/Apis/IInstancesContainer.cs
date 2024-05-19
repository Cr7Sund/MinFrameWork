using System;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Api
{
    public interface IInstancesContainer : IAssetContainer
    {
        public GameObject Create(string name, params Type[] components);
        public ComponentT Create<ComponentT>(string name) where ComponentT : Object;
        public PromiseTask<T> Instantiate<T>(IAssetKey assetKey, string name) where T : Object;
        public PromiseTask<T> InstantiateAsync<T>(IAssetKey assetKey, string name, UnsafeCancellationToken cancellation) where T : Object;
        public GameObject GetInstance(IAssetKey assetKey, string name);
        public void ReturnInstance(string name);
        public PromiseTask ReturnInstance(string name, IAssetKey assetKey);
    }
}