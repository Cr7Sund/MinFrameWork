/*
 * we don't recommend the api provide message like below
 *
        public static void IsFalse(bool expected, string message)
        {
            if (expected)
            {
                throw new   MyException(message);
            }
        }
        
 * since we want to utilize the assert message, the last but not the least to delay the string message to generate
 * and if you want to show your custom error message,
 * instead we recommend pass an new specific exception type to provide meaningful error messages.
 */
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
                throw new MyException($"Assert.InRange() Failure : Actual: {actual}, High :{high} Low: {low}");
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
                throw new MyException($"expected value is not null, but it is null");
            }
        }

        public static void NotNull<TEnum>(object anObject, TEnum errorCode) where TEnum : Enum
        {
            if (anObject == null)
            {
                throw new MyException(errorCode);
            }
        }

        public static void NotNull(object anObject, Exception exception)
        {
            if (anObject == null)
            {
                throw exception;
            }
        }

        public static void IsInstanceOf<T>(object actual)
        {
            if (!typeof(T).IsInstanceOfType(actual))
            {
                throw new MyException($"expect type{typeof(T)} but it is {actual?.GetType()}");
            }
        }

        public static void IsInstanceOf<T, TEnum>(object actual, TEnum errorCode) where TEnum : Enum
        {
            if (!typeof(T).IsInstanceOfType(actual))
            {
                throw new MyException(errorCode);
            }
        }

        public static void IsInstanceOf(Type excepted, object actual)
        {
            if (!excepted.IsInstanceOfType(actual))
            {
                throw new MyException($"expected type: {excepted} disMatch actual type: {actual?.GetType()}");
            }
        }

        public static void IsInstanceOf<TEnum>(Type excepted, object actual, TEnum errorCode) where TEnum : Enum
        {
            if (!excepted.IsInstanceOfType(actual))
            {
                throw new MyException(
                     $"ErrorCode:{errorCode}. expected type: {excepted} disMatch actual type: {actual?.GetType()}",
                     errorCode);
            }
        }

        public static void IsAssignableFrom<TEnum>(Type excepted, Type actual, TEnum errorCode) where TEnum : Enum
        {
            if (!excepted.IsAssignableFrom(actual))
            {
                throw new MyException(
                     $"ErrorCode:{errorCode}. expected type: {excepted} is not assign from actual type: {actual?.GetType()}",
                     errorCode);
            }
        }

        public static void Greater(int arg1, int arg2)
        {
            if (arg1 <= arg2)
                throw new MyException($"excepted {arg1} greater than {arg2}");
        }


        public static void Greater<TEnum>(int arg1, int arg2, TEnum errorCode) where TEnum : Enum
        {
            if (arg1 <= arg2)
                throw new MyException(errorCode);
        }

        public static void LessOrEqual(int arg1, int arg2)
        {
            if (arg1 > arg2)
                throw new MyException($"the value is {arg1} but expected lessOrEqual {arg2}");
        }
        public static void LessOrEqual(uint arg1, uint arg2)
        {
            if (arg1 > arg2)
                throw new MyException($"the value is {arg1} but expected lessOrEqual {arg2}");
        }

        public static void LessOrEqual<TEnum>(int arg1, int arg2, TEnum errorCode) where TEnum : Enum
        {
            if (arg1 > arg2)
                throw new MyException(errorCode);
        }

        public static void LessOrEqual<TEnum>(uint arg1, uint arg2, TEnum errorCode) where TEnum : Enum
        {
            if (arg1 > arg2)
                throw new MyException(errorCode);
        }

        public static void IsFalse(bool expected)
        {
            if (expected)
            {
                throw new MyException("Expected false, but it's true");
            }
        }

        public static void IsFalse(bool expected, string message)
        {
            if (expected)
            {
                throw new MyException(message);
            }
        }
        public static void IsFalse<TEnum>(bool expected, TEnum errorCode) where TEnum : Enum
        {
            if (expected)
            {
                throw new MyException(errorCode);
            }

        }

        public static void IsTrue(bool expected)
        {
            if (!expected)
            {
                throw new MyException("Expected true, but it's false");
            }
        }

        public static void IsTrue<TEnum>(bool expected, TEnum errorCode) where TEnum : Enum
        {
            if (!expected)
            {
                throw new MyException(errorCode);
            }

        }

        public static void AreEqual(object expected, object actual)
        {
            if (!EqualWithoutBoxing(expected, actual))
            {
                throw new MyException($"Expected {expected}  but it's {actual}");
            }
        }
        public static void AreEqual(int expected, int actual)
        {
            if (expected != actual)
            {
                throw new MyException($"Expected {expected}  but it's {actual}");
            }
        }
        public static void AreEqual(uint expected, uint actual)
        {
            if (expected != actual)
            {
                throw new MyException($"Expected {expected}  but it's {actual}");
            }
        }

        public static void AreEqual<TEnum>(object expected, object actual, TEnum errorCode) where TEnum : Enum
        {
            if (!EqualWithoutBoxing(expected, actual))
            {
                throw new MyException(errorCode);
            }
        }

        public static void AreNotEqual(object expected, object actual)
        {
            if (EqualWithoutBoxing(expected, actual))
            {
                throw new MyException($"Expected {expected}  but it's {actual}");
            }
        }


        public static void AreNotEqual<TEnum>(object expected, object actual, TEnum errorCode) where TEnum : Enum
        {
            if (EqualWithoutBoxing(expected, actual))
            {
                throw new MyException(errorCode);
            }
        }


        public static bool EqualWithoutBoxing(object o1, object o2)
        {
            if (o1 == null && o2 == null) return true;
            if (o1 == null || o2 == null) return false;

            if (o1.GetType().IsValueType)
            {
                throw new System.NotImplementedException();
            }
            else
            {
                return o1.Equals(o2);
            }
        }
        #endregion
    }
}
