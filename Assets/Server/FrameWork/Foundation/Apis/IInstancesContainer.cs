using System;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Api
{
    public interface IInstancesContainer : IAssetContainer
    {
        /// <summary>
        /// create GameObjectWith components
        /// </summary>
        /// <param name="name"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        public GameObject Create(string name, params Type[] components);
        public ComponentT Create<ComponentT>(string name) where ComponentT : Object;
        /// <summary>
        /// Instantiate GameObject from asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetKey"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public PromiseTask<T> Instantiate<T>(IAssetKey assetKey, string name) where T : Object;
        public PromiseTask<T> InstantiateAsync<T>(IAssetKey assetKey, string name, UnsafeCancellationToken cancellation) where T : Object;
        public GameObject GetInstance(IAssetKey assetKey, string name);
        public void ReturnInstance(string name);
        public PromiseTask ReturnInstance(string name, IAssetKey assetKey);
    }
}