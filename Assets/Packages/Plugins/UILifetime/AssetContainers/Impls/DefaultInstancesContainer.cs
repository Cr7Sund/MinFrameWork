using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cr7Sund.Collection.Generic;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.AssetLoader.Api;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.LifeTime
{
    public abstract class DefaultInstancesContainer : BaseInstanceCreator, IInstancesContainer
    {
        private readonly Dictionary<string, Object> _instanceContainers = new();
        private readonly Dictionary<IAssetKey, UnsafeUnOrderList<Object>> _instantiateObjs = new();
        [Inject] private IAssetLoader _assetLoader;

        internal DefaultInstancesContainer(string sceneName):base(sceneName)
        {
            
        }

        public Object Create(string name, params Type[] components)
        {
            if (!_instanceContainers.TryGetValue(name, out var instance))
            {
                instance = new GameObject(name, components);
                OnInstantiate(instance);
                _instanceContainers.Add(name, instance);
            }

            return instance;
        }

        public ComponentT Create<ComponentT>(string name) where ComponentT : Object
        {
            if (!_instanceContainers.TryGetValue(name, out var instance))
            {
                instance = new GameObject(name, typeof(ComponentT));
                OnInstantiate(instance);
                _instanceContainers.Add(name, instance);
            }

            var go = instance as GameObject;
            return go?.GetComponent<ComponentT>();
        }

        public async PromiseTask<IEnumerable<Object>> Instantiate<T>(IAssetKey assetKey, string name, int count = 0) where T : Object
        {
            if (!_instantiateObjs.TryGetValue(assetKey, out var instances))
            {
                var asset = await _assetLoader.Load<T>(assetKey); // resolved
                InstantiateAssets(asset, name, count, assetKey);
            }
            return instances;
        }

        public async PromiseTask<IEnumerable<Object>> InstantiateAsync<T>(IAssetKey assetKey, string name,
            UnsafeCancellationToken cancellation, int count = 0) where T : Object
        {
            if (!_instantiateObjs.TryGetValue(assetKey, out var instances))
            {
                var asset = await _assetLoader.LoadAsync<T>(assetKey, cancellation);
                await InstantiateAssetsAsync(asset, name, count, cancellation, assetKey);
            }

            return instances;
        }

        public Object GetInstance(IAssetKey assetKey, string name)
        {
            return _instanceContainers.GetValueOrDefault(name);
        }

        public void ReturnInstance(string name)
        {
            if (_instanceContainers.TryGetValue(name, out var instance))
            {
                GameObjectUtil.Destroy(instance);
                _instanceContainers.Remove(name);
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
                GameObjectUtil.Destroy(instance);
                _instanceContainers.Remove(name);
            }

            if (_instantiateObjs.TryGetValue(assetKey, out var instances))
            {
                Object target = null;
                foreach (var item in instances)
                {
                    var goName = GetGameObjectName(item);
                    if (name == goName)
                    {
                        target = item;
                        break;
                    }
                }

                _instantiateObjs[assetKey].Remove(target);
            }

            if (_instantiateObjs[assetKey].Count == 0)
            {
                await _assetLoader.Unload(assetKey);
                _instantiateObjs.Remove(assetKey);
            }
        }

        public PromiseTask Unload(IAssetKey key)
        {
            throw new NotSupportedException("please use returnInstance");
        }

        public async PromiseTask UnloadAll()
        {
            foreach (var item in _instanceContainers)
            {
                GameObjectUtil.Destroy(item.Value);
            }
            _instanceContainers.Clear();

            foreach (var item in _instantiateObjs)
            {
                foreach (var instance in item.Value)
                {
                    GameObjectUtil.Destroy(instance);
                }
                await _assetLoader.Unload(item.Key);
                item.Value.Clear();
            }
            _instantiateObjs.Clear();
        }

        public virtual void Dispose()
        {
            AssertUtil.LessOrEqual(_instanceContainers.Count, 0);
        }


        private static string GetGameObjectName(Object instance)
        {
            if (MacroDefine.IsRelease)
            {
                return instance.name;
            }
            else
            {
                var regex = new Regex(@"\((.*?)\)");
                return regex.Match(instance.name).Value;
            }
        }

        protected override void OnInstantiate(Object instance)
        {
            base.OnInstantiate(instance);
            _instanceContainers.Add(instance.name, instance);
        }

        protected sealed override void OnCreate(IAssetKey assetKey, Object instance)
        {
            if (!_instantiateObjs.TryGetValue(assetKey, out var objectList))
            {
                objectList = new UnsafeUnOrderList<Object>();
                _instantiateObjs.Add(assetKey, objectList);
            }
            objectList.AddLast(instance);
            OnInstantiate(instance);
        }
    }
}
