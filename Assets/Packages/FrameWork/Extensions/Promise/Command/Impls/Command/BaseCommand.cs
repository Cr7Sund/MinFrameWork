using System;
using Cr7Sund.PackageTest.Api;
namespace Cr7Sund.PackageTest.Impl
{
    public abstract class BaseCommand : IBaseCommand
    {
        public virtual void OnCatch(Exception e)
        {
        }

        public virtual void OnProgress(float progress) { }
    }

}
