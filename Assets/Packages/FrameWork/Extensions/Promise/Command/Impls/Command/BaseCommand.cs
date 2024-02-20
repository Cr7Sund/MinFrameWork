using System;
using Cr7Sund.Package.Api;
namespace Cr7Sund.Package.Impl
{
    public abstract class BaseCommand : IBaseCommand
    {
        public virtual void OnCatch(Exception e)
        {
        }

        public virtual void OnProgress(float progress) { }
    }

}
