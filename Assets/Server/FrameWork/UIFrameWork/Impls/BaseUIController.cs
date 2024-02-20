using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;
namespace Cr7Sund.Server.UI.Impl
{
    public abstract class BaseUIController : BaseController, IUIController
    {
        public virtual IPromise Prepare(object intent = null)
        {
            return Promise.Resolved();
        }
        public virtual IPromise WillPushEnter()
        {
             return Promise.Resolved();
        }
        public virtual IPromise DidPushEnter()
        {
             return Promise.Resolved();
        }
        public virtual IPromise WillPushExit()
        {
             return Promise.Resolved();
        }
        public virtual IPromise DidPushExit()
        {
             return Promise.Resolved();
        }
        public virtual IPromise WillPopEnter()
        {
             return Promise.Resolved();
        }
        public virtual IPromise DidPopEnter()
        {
             return Promise.Resolved();
        }
        public virtual IPromise WillPopExit()
        {
             return Promise.Resolved();
        }
        public virtual IPromise DidPopExit()
        {
             return Promise.Resolved();
        }
    }
}
