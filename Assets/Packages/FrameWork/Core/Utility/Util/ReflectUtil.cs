using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cr7Sund.FrameWork.Util
{
    public static class ReflectUtil
    {
        public static Type GetElementType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
            {
                return type.GetGenericArguments()[0];
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return typeof(object);
            }

            return type;
        }

        public static bool IsCollection(Type type)
        {
            return type.IsArray
            || typeof(IEnumerable).IsAssignableFrom(type)
            || typeof(IEnumerable<object>).IsAssignableFrom(type);
        }

        public static int GetCollectionLength(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var type = instance.GetType();
            if (type.IsArray)
            {
                return ((Array)instance).Length;
            }
            else if (typeof(IEnumerable<object>).IsAssignableFrom(type))
            {
                return ((IEnumerable<object>)instance).Count();
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                int count = 0;
                foreach (var item in (IEnumerable)instance)
                {
                    count++;
                }
                return count;
            }
            else
            {
                throw new ArgumentException("The provided instance is neither an array nor an IEnumerable<object>.");
            }
        }
    }
}