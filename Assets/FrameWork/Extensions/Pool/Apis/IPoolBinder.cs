using System;

namespace Cr7Sund.Framework.Api
{
    public interface IPoolBinder : IBinder
    {
        /// <summary>
        /// Get a no value pool based on the provided Type
        /// if not exist will return a new create value instead
        /// </summary>
        IPool GetOrCreate(Type type);
        
        /// <summary>
        /// Get a value pool based on the provided Type
        /// if not exist will return a new create value instead
        /// </summary>
        IPool<T> GetOrCreate<T>() where T : class, new();
        
        /// Get a no value pool based on the provided Type
        /// </summary>
        IPool Get(Type type);
        
        
        /// if not exist will return a new create value instead
        /// </summary>
        IPool<T> Get<T>() where T : class, new();
    }
}