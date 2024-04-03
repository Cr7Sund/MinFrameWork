using System;
using Cr7Sund.FrameWork.Util;
using System.Collections.Generic;
using System.Collections;
namespace Cr7Sund.Collection
{
    internal abstract class SZGenericArrayEnumeratorBase : IDisposable
    {
        protected int _index;
        protected readonly int _endIndex;

        protected SZGenericArrayEnumeratorBase(int endIndex)
        {
            _index = -1;
            _endIndex = endIndex;
        }

        public bool MoveNext()
        {
            int index = _index + 1;
            if ((uint)index < (uint)_endIndex)
            {
                _index = index;
                return true;
            }
            _index = _endIndex;
            return false;
        }

        public void Reset() => _index = -1;

        public void Dispose()
        {
        }
    }

    internal sealed class SZGenericArrayEnumerator<T> : SZGenericArrayEnumeratorBase, IEnumerator<T>
    {
        private readonly T[] _array;

        /// <summary>Provides an empty enumerator singleton.</summary>
        /// <remarks>
        /// If the consumer is using SZGenericArrayEnumerator elsewhere or is otherwise likely
        /// to be using T[] elsewhere, this singleton should be used.  Otherwise, GenericEmptyEnumerator's
        /// singleton should be used instead, as it doesn't reference T[] in order to reduce footprint.
        /// </remarks>
        internal static readonly SZGenericArrayEnumerator<T> Empty = new SZGenericArrayEnumerator<T>(null, 0);

        internal SZGenericArrayEnumerator(T[] array, int endIndex)
            : base(endIndex)
        {
            AssertUtil.IsTrue(array == null || endIndex == array.Length);

            _array = array;
        }

        public T Current
        {
            get
            {
                if ((uint)_index >= (uint)_endIndex)
                {
                    throw new InvalidOperationException("InvalidOperation_EnumCurrent");
                }
                return _array![_index];
            }
        }

        object IEnumerator.Current => Current;
    }

}
