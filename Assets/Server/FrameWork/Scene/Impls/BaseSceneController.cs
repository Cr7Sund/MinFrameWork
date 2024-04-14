using System;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.Scene.Impl
{
    public abstract class BaseSceneController : UpdateController, ILateUpdate
    {
        [Inject(ServerBindDefine.SceneLogger)] protected IInternalLog Debug;


        public void LateUpdate(int millisecond)
        {
            if (MacroDefine.NoCatchMode)
            {
                OnLateUpdate(millisecond);
            }
            else
            {
                try
                {
                    OnLateUpdate(millisecond);
                }
                catch (Exception e)
                {
                    Console.Error(e, "{TypeName}.OnLateUpdate Error: ", GetType().FullName);
                    throw;
                }
            }
        }

        protected virtual void OnLateUpdate(int millisecond){}
        protected override void OnUpdate(int millisecond)
        {
        }
    }
}
