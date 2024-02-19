using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Scene.Apis
{
    public interface ISceneKey : IAssetKey
    {
        public LoadSceneMode LoadSceneMode { get;   }
        public bool ActivateOnLoad { get;   }
    }
}
