using System;
using System.Collections.Generic;
using System.Threading;
using Cr7Sund.Collection.Generic;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Api;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public abstract class BaseInstancesContainer : BaseAssetContainer, IInstancesContainer
    {
        private readonly Dictionary<string, GameObject> _instanceContainers = new();
        private readonly Dictionary<IAssetKey, UnsafeUnOrderList<GameObject>> _instantiatePromises = new();



        public GameObject Create(string name, params Type[] components)
        {
            if (!_instanceContainers.TryGetValue(name, out var instance))
            {
                instance = new GameObject(name, components);
                MoveInstanceToScene(instance);
                _instanceContainers.Add(name, instance);
            }

            return instance;
        }

        public ComponentT Create<ComponentT>(string name) where ComponentT : Object
        {
            if (!_instanceContainers.TryGetValue(name, out var instance))
            {
                instance = new GameObject(name, typeof(ComponentT));
                MoveInstanceToScene(instance);
                _instanceContainers.Add(name, instance);
            }

            return instance.GetComponent<ComponentT>();
        }

        public async PromiseTask<T> Instantiate<T>(IAssetKey assetKey, string name) where T : Object
        {
            if (_instantiatePromises.TryGetValue(assetKey, out var instances))
            {
                foreach (var item in instances)
                {
                    if (item.name == name)
                    {
                        return item as T;
                    }
                }
            }

            // there exist same game object instantiate not by asset
            AssertUtil.IsFalse(_instanceContainers.ContainsKey(name));

            var asset = await LoadAsset<T>(assetKey); // resolved
            var instance = InstantiateAsset(asset, name);
            if (!_instantiatePromises.TryGetValue(assetKey, out var promiseList))
            {
                promiseList = new UnsafeUnOrderList<GameObject>();
                _instantiatePromises.Add(assetKey, promiseList);
            }
            promiseList.AddLast(instance as GameObject);

            return instance;
        }

        public async PromiseTask<T> InstantiateAsync<T>(IAssetKey assetKey, string name, CancellationToken cancellation) where T : Object
        {
            if (_instantiatePromises.TryGetValue(assetKey, out var instances))
            {
                foreach (var item in instances)
                {
                    if (item.name == name)
                    {
                        return item as T;
                    }
                }
            }

            // there exist same game object instantiate not by asset
            AssertUtil.IsFalse(_instanceContainers.ContainsKey(name));

            var asset = await LoadAssetAsync<T>(assetKey, cancellation);
            var instance = InstantiateAsset(asset, name);
            if (!_instantiatePromises.TryGetValue(assetKey, out var promiseList))
            {
                promiseList = new UnsafeUnOrderList<GameObject>();
                _instantiatePromises.Add(assetKey, promiseList);
            }
            promiseList.AddLast(instance as GameObject);

            return instance;
        }

        public GameObject GetInstance(IAssetKey assetKey, string name)
        {
            return _instanceContainers.GetValueOrDefault(name);
        }

        public void ReturnInstance(string name)
        {
            if (_instanceContainers.TryGetValue(name, out var instance))
            {
                Object.Destroy(instance);
            }
            else
            {
                if (MacroDefine.IsDebug)
                {
                    throw new MyException("Return fail, try return with assetKey");
                }
            }
        }

        public async PromiseTask ReturnInstance(string name, IAssetKey assetKey)
        {
            if (_instanceContainers.TryGetValue(name, out var instance))
            {
                GameObject.Destroy(instance);
            }

            if (_instantiatePromises.TryGetValue(assetKey, out var instances))
            {
                GameObject target = null;
                foreach (var item in instances)
                {
                    if (item.name == name)
                    {
                        target = item;
                        break;
                    }
                }

                _instantiatePromises[assetKey].Remove(target);
            }
            if (_instantiatePromises[assetKey].Count == 0)
            {
                await base.Unload(assetKey);
                _instantiatePromises.Remove(assetKey);
            }
        }

        public async override PromiseTask UnloadAll()
        {
            foreach (var item in _instanceContainers)
            {
                Object.Destroy(item.Value);
            }
            _instanceContainers.Clear();

            foreach (var item in _instantiatePromises)
            {
                foreach (var instance in item.Value)
                {
                    Object.Destroy(instance);
                }
                await base.Unload(item.Key);
                item.Value.Clear();
            }
            _instantiatePromises.Clear();

            await base.UnloadAll();
        }

        public override void Dispose()
        {
            base.Dispose();
            AssertUtil.LessOrEqual(_instanceContainers.Count, 0);
        }

        protected abstract void MoveInstanceToScene(GameObject instance);

        private T InstantiateAsset<T>(T asset, string name) where T : Object
        {
            var instance = GameObject.Instantiate(asset);
            // OnInstantiate(instance);
            instance.name = name;
            MoveInstanceToScene(instance as GameObject);
            _instanceContainers.Add(name, instance as GameObject);
            return instance;
        }

    }
}
