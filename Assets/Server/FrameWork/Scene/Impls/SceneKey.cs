using System;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Framework.Util;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Impl
{
    public delegate SceneBuilder CreateSceneBuilderDelegate();

    public class SceneKey : IAssetKey
    {
        private readonly Type _builderType;

        public LoadSceneMode LoadSceneMode { get; private set; }
        public string Key { get; private set; }
        public bool ActivateOnLoad { get; private set; }


        public SceneKey(string key, Type builderType) : this(key, builderType, LoadSceneMode.Single, true)
        {
        }
        public SceneKey(string key, Type builderType, LoadSceneMode loadMode) : this(key, builderType, loadMode, true)
        {
        }
        public SceneKey(string key, Type builderType, LoadSceneMode loadMode, bool activateOnLoad)
        {
            AssertUtilEditor.IsAssignableFrom(typeof(SceneBuilder), builderType, SceneExceptionType.INVALID_SCENE_BUILDER_TYPE);

            Key = key;
            _builderType = builderType;
            LoadSceneMode = loadMode;
            ActivateOnLoad = activateOnLoad;
        }


        public SceneBuilder Create() => Activator.CreateInstance(_builderType) as SceneBuilder;
        public static implicit operator string(SceneKey sceneKey) => sceneKey.Key;
        public override string ToString() => Key;
    }
}
