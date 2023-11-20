using System;
using Cr7Sund.Framework.Api;


namespace Cr7Sund.Framework.Impl
{
    public abstract class BaseCommand : IBaseCommand
    {
        [Inject] private IPoolBinder poolBinder;
        [Inject] private IInjectionBinder injectionBinder;
        protected int retainCount;

        public bool IsRetain => retainCount > 0;

        public virtual void OnCatch(Exception e)
        {
        }

        public virtual void OnProgress(float progress) { }

        public void Release()
        {
            // retainCount--;
            if (retainCount == 0)
            {
                // Restore();
                // _commandBinder
            }
        }

        public void Restore()
        {
            if (injectionBinder != null)
            {
                injectionBinder.Injector.Uninject(this);
            }
        }

        public void Retain()
        {
            // retainCount++;
        }
    }
}
