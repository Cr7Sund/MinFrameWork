namespace Cr7Sund.Server.Api
{
    public interface ISceneInstanceContainer : IInstancesContainer
    {
        public string SceneName { get; }

        public void Init(string sceneName);
    }
}