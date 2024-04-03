using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections;
namespace Cr7Sund.Collection.Generic
{
    public class UnsafeUnOrderList<T> : IEnumerable<T>
    {
        private const int DefaultCapacity = 4;
        private const int MaxSize = Int32.MaxValue;
        private static readonly T[] _emptyArray = new T[0];

        internal T[] _items;
        internal int _size;

        public int Count => _size;

        public UnsafeUnOrderList()
        {
            _items = _emptyArray;
            _size = 0;
        }

        public UnsafeUnOrderList(int count)
        {
            _items = new T[count];
            _size = count;
        }


        public void AddLast(T value)
        {
            if (_items.Length <= _size)
            {
                AddWithResize();
            }

            _items[_size++] = value;
        }

        private void AddWithResize()
        {
            var capacity = GetNewCapacity();

            if (capacity != _items.Length)
            {
                var newItems = new T[capacity];
                Array.Copy(_items, newItems, _size);
                _items = newItems;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetNewCapacity()
        {
            int newCapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;

            if (newCapacity > MaxSize) throw new Exception("beyond limit");
            return newCapacity;
        }

        public void Remove(T value)
        {
            int matchIndex = Array.IndexOf(_items, value);
            _items[matchIndex] = _items[_size - 1];
            _items[--_size] = default;
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Count == 0 ? SZGenericArrayEnumerator<T>.Empty :
                       new Enumerator(this);
        }

        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size); // Clear the elements so that the gc can reclaim the references.
            }
            _size = 0;
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            internal static IEnumerator<T> s_emptyEnumerator;

            private readonly UnsafeUnOrderList<T> _list;
            private int _index;
            private T _current;

            internal Enumerator(UnsafeUnOrderList<T> list)
            {
                _list = list;
                _index = 0;
                _current = default;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                UnsafeUnOrderList<T> localList = _list;

                if ((uint)_index < (uint)localList._size)
                {
                    _current = localList._items[_index];
                    _index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                _index = _list._size + 1;
                _current = default;
                return false;
            }

            public T Current => _current!;

            object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _list._size + 1)
                    {
                        throw new InvalidOperationException("InvalidOperation_EnumFailed");
                    }
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                _index = 0;
                _current = default;
            }
        }

    }

}
