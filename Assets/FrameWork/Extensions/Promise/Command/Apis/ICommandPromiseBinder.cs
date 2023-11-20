using System;

namespace Cr7Sund.Framework.Api
{
    public interface ICommandPromiseBinder<PromisedT>
    {
        void ReactTo(object trigger, PromisedT data);

        ICommandPromiseBinding<PromisedT> Bind(object trigger);
        ICommandPromiseBinding<PromisedT> GetBinding(object key);
        ICommandPromiseBinding<PromisedT> GetBinding(object key, object name);
    }

}