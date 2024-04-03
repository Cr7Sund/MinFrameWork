using System;
using System.Collections.Generic;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Collection.Generic;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Apis;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Impl
{
    public abstract class BaseInstancesContainer : BaseAssetContainer, IInstanceContainer
    {
        private Dictionary<string, GameObject> _instanceContainers = new();
        private readonly Dictionary<IAssetKey, UnsafeUnOrderList<IAssetPromise>> _instantiatePromises = new();



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


        public IAssetPromise Instantiate(IAssetKey assetKey, string name)
        {
            if (_instantiatePromises.TryGetValue(assetKey, out var instances))
            {
                int count = instances.Count;
                foreach (var item in instances)
                {
                    if (item.Name == name)
                    {
                        return item;
                    }
                }
            }

            if (_instanceContainers.ContainsKey(name))
            {
                return AssetPromise.Rejected(new MyException("DuplicateName")) as IAssetPromise;
            }


            var asset = LoadAsset(assetKey).ForceGetResult<Object>(); // resolved
            var instance = InstantiateAsset(asset, name);
            var instancePromise = new AssetPromise();
            var promiseList = new UnsafeUnOrderList<IAssetPromise>();

            instancePromise.WithName(name);
            instancePromise.Resolve(instance);

            promiseList.AddLast(instancePromise);
            _instantiatePromises.Add(assetKey, promiseList);

            return instancePromise;
        }

        public IAssetPromise InstantiateAsync(IAssetKey assetKey, string name)
        {
            if (_instantiatePromises.TryGetValue(assetKey, out var instances))
            {
                int count = instances.Count;
                foreach (var item in instances)
                {
                    if (item.Name == name)
                    {
                        return item;
                    }
                }
            }

            if (_instanceContainers.ContainsKey(name))
            {
                return AssetPromise.Rejected(new MyException("DuplicateName")) as IAssetPromise;
            }

            var instancePromise = new AssetPromise();
            instancePromise.WithName(name);
            var promiseList = new UnsafeUnOrderList<IAssetPromise>();
            promiseList.AddLast(instancePromise);
            _instantiatePromises.Add(assetKey, promiseList);

            LoadAssetAsync(assetKey).Then((asset) =>
            {
                var instance = InstantiateAsset(asset, name);
                instancePromise.Resolve(instance);
            });

            return instancePromise;
        }

        public GameObject GetInstance(IAssetKey assetKey, string name)
        {
            if (_instanceContainers.ContainsKey(name))
            {
                return _instanceContainers[name];
            }
            return null;
        }

        public void ReturnInstance(string name)
        {
            if (!_instanceContainers.TryGetValue(name, out var instance))
            {
                GameObject.Destroy(instance);
            }
            else
            {
                if (MacroDefine.IsDebug)
                {
                    throw new MyException("Return fail, try return with assetKey");
                }
            }
        }

        public void ReturnInstance(string name, IAssetKey assetKey)
        {
            if (!_instanceContainers.TryGetValue(name, out var instance))
            {
                GameObject.Destroy(instance);
            }

            if (_instantiatePromises.TryGetValue(assetKey, out var instances))
            {
                int count = instances.Count;
                IAssetPromise targetPromise = null;
                foreach (var item in instances)
                {
                    if (item.Name == name)
                    {
                        targetPromise = item;
                        break;
                    }
                }

                _instantiatePromises[assetKey].Remove(targetPromise);
            }
            if (_instantiatePromises[assetKey].Count == 0)
            {
                base.Unload(assetKey);
            }
        }

        public override void Dispose()
        {
            foreach (var item in _instanceContainers)
            {
                GameObject.Destroy(item.Value);
            }

            foreach (var item in _instantiatePromises)
            {
                foreach (var instance in item.Value)
                {
                    instance.Dispose();
                }
                item.Value.Clear();
            }
            base.Dispose();
            _instanceContainers.Clear();
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