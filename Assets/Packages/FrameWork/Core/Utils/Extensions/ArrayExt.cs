using System;
namespace Cr7Sund.PackageTest.Util
{
    public static class ArrayExt
    {
        public const int UNMATCHINDEX = -1;
        public static void SpliceValueAt<T>(this T[] instance, int index, ref int size)
        {
            if (index >= instance.Length)
            {
                throw new IndexOutOfRangeException();
            }
            size--;
            if (index < size)
            {
                Array.Copy(instance, index + 1, instance, index, size - index);
            }
            if (!typeof(T).IsValueType)
            {
                instance[size] = default!;
            }
        }

        public static bool Contains<T>(this T[] instance, T o)
        {
            return instance != null && Array.IndexOf(instance, o) != UNMATCHINDEX;
        }

        public static int FindMatchIndex<T>(this T[] instance, T o)
        {
            if (instance == null) return UNMATCHINDEX;

            for (int i = 0; i < instance.Length; i++)
            {
                var curVal = instance[i];
                if (o.Equals(curVal))
                {
                    return i;
                }
            }

            return UNMATCHINDEX;
        }
    }
}
