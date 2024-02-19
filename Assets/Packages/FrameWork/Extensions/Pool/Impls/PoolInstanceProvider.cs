using System;
using Cr7Sund.PackageTest.Api;
namespace Cr7Sund.PackageTest.Impl
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
