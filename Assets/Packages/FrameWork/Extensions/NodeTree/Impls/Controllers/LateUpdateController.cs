using System;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class LateUpdateController : BaseController, ILateUpdate
    {
        public void LateUpdate(int millisecond)
        {
            OnLateUpdate(millisecond);
        }

        protected abstract void OnLateUpdate(int millisecond);
    }
}
