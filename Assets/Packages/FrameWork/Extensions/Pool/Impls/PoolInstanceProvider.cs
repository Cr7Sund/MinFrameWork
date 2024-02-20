using System;
using Cr7Sund.Package.Api;
namespace Cr7Sund.Package.Impl
{
    public class PoolInstanceProvider : IInstanceProvider
    {
        public T GetInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }

        public object GetInstance(Type key)
        {
            return Activator.CreateInstance(key);
        }
    }
}
