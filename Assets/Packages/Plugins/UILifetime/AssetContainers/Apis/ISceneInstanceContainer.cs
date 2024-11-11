namespace Cr7Sund.LifeTime
{
    public interface ISceneInstanceContainer : IInstancesContainer
    {
        public string SceneName { get; }

        public void Init(string sceneName);
    }
}