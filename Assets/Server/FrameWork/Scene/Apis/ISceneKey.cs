using Cr7Sund.AssetLoader.Api;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Server.Apis
{
    public interface ISceneKey : IAssetKey
    {
        public LoadSceneMode LoadSceneMode { get;   }
        public bool ActivateOnLoad { get;   }
    }
}
