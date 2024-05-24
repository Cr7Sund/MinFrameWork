using System;
using System.Collections.Generic;

namespace Cr7Sund.FrameWork.Util
{
    public static class ReflectUtil
    {
        public static Type GetRealType(Type type)
        {
            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if(typeof(IEnumerable<object>).IsAssignableFrom(type))
            {
                type = type.GetGenericArguments()[0];
            }

            return type;
        }
    }
}