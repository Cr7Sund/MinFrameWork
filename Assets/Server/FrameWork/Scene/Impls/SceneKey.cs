using System;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Framework.Util;

namespace Cr7Sund.Server.Impl
{
    public delegate SceneBuilder CreateSceneBuilderDelegate();

    public class SceneKey : IAssetKey
    {
        private readonly Type _builderType;

        public string Key { get; private set; }

        public SceneKey(string key, Type builderType)
        {
            // AssertUtilEditor.IsAssignableFrom(typeof(SceneBuilder), builderType, SceneExceptionType.INVALID_SCENE_BUILDER_TYPE);

            this.Key = key;
            this._builderType = builderType;
        }

        public SceneBuilder Create() => Activator.CreateInstance(_builderType) as SceneBuilder;
        public static implicit operator string(SceneKey sceneKey) => sceneKey.Key;
        public override string ToString() => Key;
    }
}
