using Cr7Sund.NodeTree.Impl;
using UnityEngine.PlayerLoop;

namespace Cr7Sund.Selector.Impl
{
    public class EditorMainController : UpdateController
    {
        protected override void OnStart()
        {
            base.OnStart();
            Debug.Info("Start");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }


        protected override void OnUpdate(int millisecond)
        {
            Log.Info("Update");
        }
    }
}