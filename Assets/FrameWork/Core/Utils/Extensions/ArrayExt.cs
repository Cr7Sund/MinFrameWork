using System;
namespace Cr7Sund.Framework.Util
{
    public static class ArrayExt
    {
        public const int UNMATCHINDEX = -1;
        public static T[] SpliceValueAt<T>(this T[] instance, int splicePos)
        {
            var newList = new T[instance.Length - 1];
            int mod = 0;
            for (int i = 0; i < instance.Length; i++)
            {
                if (i == splicePos)
                {
                    mod = -1;
                    continue;
                }
                newList[i + mod] = instance[i];
            }
            return newList.Length == 0 ? null : newList;
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
