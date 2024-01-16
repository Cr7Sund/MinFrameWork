using System;
using Cr7Sund.Framework.Util;

namespace Cr7Sund.Server.Impl
{
    public delegate SceneBuilder CreateSceneBuilderDelegate();

    public class SceneKey
    {
        private readonly string _key;
        private readonly Type _builderType;


        public SceneKey(string key, Type builderType)
        {
            AssertUtilEditor.IsAssignableFrom(typeof(SceneBuilder), builderType, SceneExceptionType.INVALID_SCENE_BUILDER_TYPE);

            this._key = key;
            this._builderType = builderType;
        }

        public SceneBuilder Create() => Activator.CreateInstance(_builderType) as SceneBuilder;
        public static implicit operator string(SceneKey sceneKey) => sceneKey._key;
        public override string ToString() => _key;
    }
}
