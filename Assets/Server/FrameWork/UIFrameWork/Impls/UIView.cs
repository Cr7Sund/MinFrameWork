using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class UIView : IUIView
    {
        public IPromise Animate(bool push)
        {
            return Promise.Resolved();
        }

        public void Hide(bool push)
        {
        }

        public void Show(bool push)
        {
        }
    }
}