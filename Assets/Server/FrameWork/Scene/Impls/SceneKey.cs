using System;

namespace Cr7Sund.Server.Impl
{   
    public delegate SceneBuilder CreateSceneBuilderDelegate();

    public class SceneKey
    {
        private readonly string _key;
        
        public SceneBuilder SceneBuilder { get; private set; }


        public SceneKey(string key, SceneBuilder sceneBuilder) 
        {
            this._key = key;
            this.SceneBuilder = sceneBuilder;
        }


        public static implicit operator string(SceneKey sceneKey) => sceneKey._key;
        public override string ToString() => _key;
    }
}
