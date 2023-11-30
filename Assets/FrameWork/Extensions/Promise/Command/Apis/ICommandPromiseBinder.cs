namespace Cr7Sund.Framework.Api
{
    public interface ICommandPromiseBinder<PromisedT> : IBinder
    {
        void ReactTo(object trigger, PromisedT data);

        new ICommandPromiseBinding<PromisedT> Bind(object trigger);
        new ICommandPromiseBinding<PromisedT> GetBinding(object key);
        new ICommandPromiseBinding<PromisedT> GetBinding(object key, object name);
    }

}
