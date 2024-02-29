using System;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class UpdateController : BaseController, IUpdatable
    {
        public void Update(int millisecond)
        {
            if (MacroDefine.NoCatchMode)
            {
                OnUpdate(millisecond);
            }
            else
            {
                try
                {
                    OnUpdate(millisecond);
                }
                catch (Exception e)
                {
                    Console.Error(e, "{TypeName}.OnUpdate Error: ", GetType().FullName);
                    throw;
                }
            }
        }

        protected abstract void OnUpdate(int millisecond);
    }
}
