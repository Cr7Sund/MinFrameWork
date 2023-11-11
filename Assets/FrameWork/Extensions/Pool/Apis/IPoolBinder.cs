using System;

namespace Cr7Sund.Framework.Api
{
    public interface IPoolBinder : IBinder
    {
        /// <summary> Get a binding based on the provided Type </summary>
        IPool Get(Type type);
        IPool<T> Get<T>() where T : class, new();
    }
}