using System;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Scene.Apis;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Scene.Impl
{
    public delegate SceneBuilder CreateSceneBuilderDelegate();

    public class SceneKey : ISceneKey
    {
        private readonly Type _builderType;
        private readonly bool _isVirtualScene;

        public LoadSceneMode LoadSceneMode { get; private set; }
        public string Key { get; private set; }
        public bool ActivateOnLoad { get; private set; }
        public bool IsVirtualScene
        {
            get
            {
                if (MacroDefine.IsEditor)
                {
                    if (!Application.isPlaying)
                    {
                        return true;
                    }
                }
                return _isVirtualScene;
            }
        }


        public SceneKey(string key, Type builderType, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, bool isVirtualScene = false)
        {
            AssertUtilEditor.IsAssignableFrom(typeof(SceneBuilder), builderType, SceneExceptionType.INVALID_SCENE_BUILDER_TYPE);

            Key = key;
            _builderType = builderType;
            LoadSceneMode = loadMode;
            ActivateOnLoad = activateOnLoad;
            _isVirtualScene = isVirtualScene;
        }


        public SceneBuilder Create() => Activator.CreateInstance(_builderType) as SceneBuilder;
        public static implicit operator string(SceneKey sceneKey) => sceneKey.Key;
        public override string ToString() => Key;
    }
}
