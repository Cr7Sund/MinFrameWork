namespace Cr7Sund.Server.Impl
{   
    public delegate SceneBuilder CreateSceneBuilderDelegate();

    public class SceneKey
    {
        private readonly string _key;
        
        internal CreateSceneBuilderDelegate Factory { get; private set; }
        
        
        public SceneKey(string key, CreateSceneBuilderDelegate factory)
        {
            this._key = key;
            this.Factory = factory;
        }

        public static implicit operator string(SceneKey sceneKey) => sceneKey._key;
        public override string ToString() => _key;
    }
}
