using System;

namespace Cr7Sund.Framework.Api
{
    public interface ICommandPromiseBinder
    {
        void ReactTo(object trigger);

        ICommandPromiseBinding Bind(object trigger);
        ICommandPromiseBinding GetBinding(object key);
        ICommandPromiseBinding GetBinding(object key, object name);
    }


}