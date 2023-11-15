using System;

namespace Cr7Sund.Framework.Api
{
    public interface IPromiseCommandBinder
    {
        void ReactTo(object trigger);
        void ReactTo<PromisedT>(object trigger, PromisedT data);

        IPromiseCommandBinding Bind(object trigger);
        IPromiseCommandBinding GetBinding(object key);
        IPromiseCommandBinding GetBinding(object key, object name);
    }


}