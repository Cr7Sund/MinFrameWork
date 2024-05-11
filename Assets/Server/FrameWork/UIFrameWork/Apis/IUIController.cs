namespace Cr7Sund.Server.UI.Api
{
    public interface IUIController : ILifeTime, IRunnable
    {
        // Called before view crate and do some async work
        public PromiseTask Prepare(object intent = null);

        // Called just before this page is displayed by the Push transition.
        public PromiseTask WillPushEnter();
        // Called just after this page is displayed by the Push transition.
        public PromiseTask DidPushEnter();
        // Called just before this page is hidden by the Push transition.
        public PromiseTask WillPushExit();
        // Called just after this page is hidden by the Push transition.
        public PromiseTask DidPushExit();
        // Called just before this page is displayed by the Pop transition.
        public PromiseTask WillPopEnter();
        // Called just after this page is displayed by the Pop transition.
        public PromiseTask DidPopEnter();
        // Called just before this page is hidden by the Pop transition.
        public PromiseTask WillPopExit();
        // Called just after this page is hidden by the Pop transition.
        public PromiseTask DidPopExit();
    }
}