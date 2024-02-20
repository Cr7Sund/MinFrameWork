using Cr7Sund.Package.Api;

namespace Cr7Sund.Server.UI.Api
{
    public interface IUIController : ILifeTime, IRunnable
    {
        // Called before view crate and do some async work
        public IPromise Prepare(object intent = null);


        // Called just before this page is displayed by the Push transition.
        public IPromise WillPushEnter();
        // Called just after this page is displayed by the Push transition.
        public IPromise DidPushEnter();
        // Called just before this page is hidden by the Push transition.
        public IPromise WillPushExit();
        // Called just after this page is hidden by the Push transition.
        public IPromise DidPushExit();
        // Called just before this page is displayed by the Pop transition.
        public IPromise WillPopEnter();
        // Called just after this page is displayed by the Pop transition.
        public IPromise DidPopEnter();
        // Called just before this page is hidden by the Pop transition.
        public IPromise WillPopExit();
        // Called just after this page is hidden by the Pop transition.
        public IPromise DidPopExit();
    }
}