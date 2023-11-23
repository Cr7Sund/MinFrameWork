using Cr7Sund.Framework.Api;
using System;
namespace Cr7Sund.Framework.Impl
{
    public class EventInstanceProvider : IInstanceProvider
    {
        public T GetInstance<T>()
        {
            object instance = new TmEvent();
            var retVal = (T)instance;
            return retVal;
        }

        public object GetInstance(Type key)
        {
            return new TmEvent();
        }
    }
}
