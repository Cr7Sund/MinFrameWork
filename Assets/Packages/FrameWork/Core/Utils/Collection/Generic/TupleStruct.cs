using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections;
namespace Cr7Sund.Collection
{
    public struct TupleStruct<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public TupleStruct(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
}