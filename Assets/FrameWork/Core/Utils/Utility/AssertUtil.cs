using NUnit.Framework;
using System;
using System.Collections.Generic;
namespace Cr7Sund.Framework.Util
{
    public static class AssertUtil
    {

        #region Extension
        /// <summary>
        ///     Verifies that a value is within a given range.
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
                throw new AssertionException($"Assert.InRange() Failure : Actual: {actual}, High :{high} Low: {low}");
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
                var typeFromHandle = typeof(T);
                if (!typeFromHandle.IsValueType || typeFromHandle.IsGenericType && typeFromHandle.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>)))
                {
                    if (Equals(x, default(T)))
                    {
                        if (Equals(y, default(T)))
                        {
                            return 0;
                        }

                        return -1;
                    }

                    if (Equals(y, default(T)))
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
        #endregion


        #region Assert Origin
        public static void NotNull(object anObject)
        {
            if (anObject == null)
            {
                NotNull(anObject, new AssertionException($"expected value is not null, but it is null"));
            }
        }

        public static void NotNull(object anObject, string message)
        {
            if (anObject == null)
            {
                throw new AssertionException(message);
            }
        }

        public static void NotNull(object anObject, Exception e)
        {
            if (anObject == null)
            {
                throw e;
            }
        }

        public static void IsInstanceOf<T>(object actual)
        {
            if (!typeof(T).IsInstanceOfType(actual))
            {
                throw new AssertionException($"expect type{typeof(T)} but it is {actual.GetType()}");
            }
        }

        public static void IsInstanceOf<T>(object actual, string message)
        {
            if (!typeof(T).IsInstanceOfType(actual))
            {
                throw new AssertionException(message);
            }
        }

        public static void IsInstanceOf<T>(object actual, Exception e)
        {
            if (!typeof(T).IsInstanceOfType(actual))
            {
                throw e;
            }
        }

        public static void IsInstanceOf(Type excepted, object actual)
        {
            if (!excepted.IsInstanceOfType(actual))
            {
                throw new AssertionException($"type{excepted} disMatch {actual.GetType()}");
            }
        }

        public static void IsInstanceOf(Type excepted, object actual, string message)
        {
            if (!excepted.IsInstanceOfType(actual))
            {
                throw new AssertionException(message);
            }
        }

        public static void IsInstanceOf(Type excepted, object actual, Exception e)
        {
            if (!excepted.IsInstanceOfType(actual))
            {
                throw e;
            }
        }

        public static void Greater(int arg1, int arg2)
        {
            if (arg1 <= arg2)
                throw new AssertionException($"excepted {arg1} greater than {arg2}");
        }

        public static void Greater(int arg1, int arg2, Exception e)
        {
            if (arg1 <= arg2)
                throw e;
        }

        public static void LessOrEqual(int arg1, int arg2)
        {
            if (arg1 > arg2)
                throw new AssertionException($"excepted {arg1} LessOrEqual {arg2}");
        }

        public static void LessOrEqual(int arg1, int arg2, Exception e)
        {
            if (arg1 > arg2)
                throw e;
        }


        public static void IsFalse(bool expected)
        {
            if (expected)
            {
                throw new AssertionException("Expected false, but it's true");
            }
        }

        public static void IsFalse(bool expected, string message)
        {
            if (expected)
            {
                throw new AssertionException(message);
            }
        }


        public static void IsFalse(bool expected, Exception e)
        {
            if (expected)
            {
                throw e;
            }

        }

        public static void IsTrue(bool expected)
        {
            if (!expected)
            {
                throw new AssertionException("Expected true, but it's false");
            }
        }

        public static void IsTrue(bool expected, string message)
        {
            if (!expected)
            {
                throw new AssertionException(message);
            }
        }

        public static void IsTrue(bool expected, Exception e)
        {
            if (!expected)
            {
                throw e;
            }

        }

        public static void AreEqual(object expected, object actual)
        {
            if (!expected.Equals(actual))
            {
                throw new AssertionException($"Expected {expected}  but it's {actual}");
            }
        }

        public static void AreEqual(object expected, object actual, string message)
        {
            if (!expected.Equals(actual))
            {
                throw new AssertionException(message);
            }
        }

        public static void AreEqual(object expected, object actual, Exception e)
        {
            if (!expected.Equals(actual))
            {
                throw e;
            }
        }
        #endregion
    }
}
