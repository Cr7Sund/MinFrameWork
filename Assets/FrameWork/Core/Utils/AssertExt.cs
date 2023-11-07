using System;
using System.Collections.Generic;

namespace Cr7Sund.Framework.Util
{
    public static class AssertExt
    {
        /// <summary>
        ///  Verifies that a value is within a given range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public static void InRange<T>(T actual, T low, T high) where T : IComparable
        {
            InRange(actual, low, high, GetComparer<T>());
        }

        public static void InRange<T>(T actual, T low, T high, IComparer<T> comparer)
        {
            if (comparer.Compare(low, actual) > 0 || comparer.Compare(actual, high) > 0)
            {
                throw new IndexOutOfRangeException($"Assert.InRange() Failure : Actual: {actual}, High :{high} Low: {low}");
            }
        }

        private static IComparer<T> GetComparer<T>() where T : IComparable
        {
            return new AssertComparer<T>();
        }

        private class AssertComparer<T> : IComparer<T> where T : IComparable
        {
            public int Compare(T x, T y)
            {
                Type typeFromHandle = typeof(T);
                if (!typeFromHandle.IsValueType || (typeFromHandle.IsGenericType && typeFromHandle.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>))))
                {
                    if (object.Equals(x, default(T)))
                    {
                        if (object.Equals(y, default(T)))
                        {
                            return 0;
                        }

                        return -1;
                    }

                    if (object.Equals(y, default(T)))
                    {
                        return -1;
                    }
                }

                if (x.GetType() != y.GetType())
                {
                    return -1;
                }

                return (x as IComparable<T>)?.CompareTo(y) ?? x.CompareTo(y);
            }
        }
    }
}