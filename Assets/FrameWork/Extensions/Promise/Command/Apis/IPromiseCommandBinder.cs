using System;

namespace Cr7Sund.Framework.Api
{
    public interface IPromiseCommandBinder<PromisedT>
    {
        void ReactTo(object trigger, PromisedT data);

        IPromiseCommandBinding<PromisedT> Bind(object trigger);
        IPromiseCommandBinding<PromisedT> GetBinding(object key);
        IPromiseCommandBinding<PromisedT> GetBinding(object key, object name);
    }

}