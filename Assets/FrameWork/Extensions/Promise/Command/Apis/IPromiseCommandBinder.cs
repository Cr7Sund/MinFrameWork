using System;

namespace Cr7Sund.Framework.Api
{
    public interface IPromiseCommandBinder
    {
        IPromiseCommandBinding Bind(object trigger);
        IPromiseCommandBinding GetBinding(object key);
        IPromiseCommandBinding GetBinding(object key, object name);
    }


}