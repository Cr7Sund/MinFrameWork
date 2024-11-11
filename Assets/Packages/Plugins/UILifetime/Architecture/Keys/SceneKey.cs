using System;
using UnityEngine.SceneManagement;
namespace Cr7Sund.LifeTime
{
    public class SceneKey : RouteKey
    {
        public LoadSceneMode LoadSceneMode
        {
            get;
            set;
        }
        public bool ActivateOnLoad
        {
            get;
            set;
        }

        public SceneKey(string sceneKey, Type sceneCtrlType, Type contextType) : base(sceneKey, sceneCtrlType, contextType)
        {

        }

        public SceneKey(string sceneKey, Type sceneCtrlType) : base(sceneKey, sceneCtrlType)
        {
            this.ContextKey = typeof(SceneContext);
        }

    }
}
