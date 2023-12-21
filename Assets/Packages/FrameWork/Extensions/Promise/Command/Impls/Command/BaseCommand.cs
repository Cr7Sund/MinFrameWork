using Cr7Sund.Framework.Api;
using System;
namespace Cr7Sund.Framework.Impl
{
    public abstract class BaseCommand : IBaseCommand
    {
        public virtual void OnCatch(Exception e)
        {
        }

        public virtual void OnProgress(float progress) { }
    }

}
