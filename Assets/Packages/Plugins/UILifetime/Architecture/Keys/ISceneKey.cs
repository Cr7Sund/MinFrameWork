using UnityEngine.SceneManagement;
namespace Cr7Sund.LifeTime
{
    public interface ISceneKey : IRouteKey
    {
        LoadSceneMode LoadSceneMode
        {
            get;
        }
        // Allow Scenes to be activated as soon as it is ready.
        // https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html
        bool ActivateOnLoad
        {
            get;
        }
    }
}