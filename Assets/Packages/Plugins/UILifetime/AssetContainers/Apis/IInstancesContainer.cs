using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Cr7Sund.LifeTime
{
    public interface IInstancesContainer : IAssetContainer
    {
        /// <summary>
        /// create GameObjectWith components
        /// </summary>
        /// <param name="name"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        public Object Create(string name, params Type[] components);
        public ComponentT Create<ComponentT>(string name) where ComponentT : Object;
        /// <summary>
        /// Instantiate Object from asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetKey"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public PromiseTask<IEnumerable<Object>> Instantiate<T>(IAssetKey assetKey, string name,int count = 0) where T : Object;
        public PromiseTask<IEnumerable<Object>> InstantiateAsync<T>(IAssetKey assetKey, string name, UnsafeCancellationToken cancellation,int count = 0) where T : Object;
        public Object GetInstance(IAssetKey assetKey, string name);
        public void ReturnInstance(string name);
        public PromiseTask ReturnInstance(string name, IAssetKey assetKey);
    }
}