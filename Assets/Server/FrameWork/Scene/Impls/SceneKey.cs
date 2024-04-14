using System;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Scene.Apis;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Scene.Impl
{
    public delegate SceneBuilder CreateSceneBuilderDelegate();

    public class SceneKey : ISceneKey
    {
        private readonly Type _builderType;

        public LoadSceneMode LoadSceneMode { get; private set; }
        public string Key { get; private set; }
        public bool ActivateOnLoad { get; private set; }
        public bool IsVirtualScene { get; private set; }


        public SceneKey(string key, Type builderType, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, bool isVirtualScene = false)
        {
            AssertUtilEditor.IsAssignableFrom(typeof(SceneBuilder), builderType, SceneExceptionType.INVALID_SCENE_BUILDER_TYPE);

            Key = key;
            _builderType = builderType;
            LoadSceneMode = loadMode;
            ActivateOnLoad = activateOnLoad;
            IsVirtualScene = isVirtualScene;
        }


        public SceneBuilder Create() => Activator.CreateInstance(_builderType) as SceneBuilder;
        public static implicit operator string(SceneKey sceneKey) => sceneKey.Key;
        public override string ToString() => Key;

        public override bool Equals(object obj)
        {
            if (obj is IAssetKey assetKey)
            {
                return Key.Equals(assetKey.Key);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}
