namespace Cr7Sund.Server.Apis
{
    public interface ISceneInstanceContainer : IInstanceContainer
    {
        public string SceneName { get; }

        public void Init(string sceneName);
    }
}