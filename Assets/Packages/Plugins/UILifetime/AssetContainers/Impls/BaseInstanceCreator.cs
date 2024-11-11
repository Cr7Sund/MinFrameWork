using Cr7Sund.FrameWork.Util;
using Cr7Sund.LifeTime;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Cr7Sund.LifeTime
{
    public abstract class BaseInstanceCreator : BaseLifeCycleAwareComponent
    {
        private string SceneName { get;
            set; }


        internal BaseInstanceCreator(string sceneName)
        {
            SceneName = sceneName;

        }
        
        protected abstract void OnCreate(IAssetKey assetKey, Object instance);

        protected void InstantiateAssets<T>(T asset, string name, int count, IAssetKey assetKey) where T : Object
        {
            for (int i = 0; i < count; i++)
            {
                var instance = Object.Instantiate(asset);
                InstantiateAsset(assetKey, instance, name, i);
            }
        }

        protected async PromiseTask InstantiateAssetsAsync<T>(T asset, string name, int count, UnsafeCancellationToken cancellation,
            IAssetKey assetKey) where T : Object
        {
            if (cancellation.IsCancellationRequested)
            {
                return;
            }

            #if UNITY_2022_3_OR_NEWER
            //https://unity.com/releases/editor/whats-new/2022.3.20
            //https://x.com/_kzr/status/1851968293823001009

            var instantiateOperation = Object.InstantiateAsync(asset, count);
            instantiateOperation.priority = assetKey.Priority;
            
            cancellation.Register(() =>
            {
                instantiateOperation.Cancel();
            });

            T[] results = await instantiateOperation;

            for (int i = 0; i < count; i++)
            {
                InstantiateAsset(assetKey, results[i], name, i);
            }
            #else
            // separate load task and instantiate task
            InstantiateAssets(asset, name, count);
            #endif
        }

        void InstantiateAsset(IAssetKey assetKey, Object instance, string name, int index)
        {
            string id = name;
            // if (index > 0)
            // {
            //     id = $"{name}_{index}";
            // }
            instance.name = id;

            OnCreate(assetKey, instance);
        }


        protected virtual void OnInstantiate(Object instance)
        {
            if (string.IsNullOrEmpty(SceneName))
            {
                Object.DontDestroyOnLoad(instance);
            }
            else
            {
                if (instance is GameObject go)
                {
                    var scene = SceneManager.GetSceneByName(SceneName);
                    AssertUtil.NotNull(scene, AssetContainerExceptionType.create_from_invalidScene);
                    SceneManager.MoveGameObjectToScene(go, scene);
                }
            }

        }
    }
}
