using System;

namespace Cr7Sund.FrameWork.Util
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