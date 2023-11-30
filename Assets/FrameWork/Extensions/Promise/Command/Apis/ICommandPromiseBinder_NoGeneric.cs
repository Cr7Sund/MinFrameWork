namespace Cr7Sund.Framework.Api
{
    public interface ICommandPromiseBinder : IBinder
    {
        void ReactTo(object trigger);

        new ICommandPromiseBinding Bind(object trigger);
        new ICommandPromiseBinding GetBinding(object key);
        new ICommandPromiseBinding GetBinding(object key, object name);
    }


}
