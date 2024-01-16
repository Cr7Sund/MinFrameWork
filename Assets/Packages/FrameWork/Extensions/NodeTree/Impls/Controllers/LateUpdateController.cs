using System;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class LateUpdateController : BaseController, ILateUpdate
    {
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
                    Debug.Error($"{GetType().FullName}.OnUpdate Error: \n", e);
                    throw;
                }
            }
        }

        protected abstract void OnLateUpdate(int millisecond);
    }
}
