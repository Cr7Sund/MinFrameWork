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

        public T[] _items;
        public int _size;

        public int Count => _size;
        private int _version;


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
            _version++;
        }

        public void Remove(T value)
        {
            int matchIndex = Array.IndexOf(_items, value);
            if (matchIndex == -1) throw new InvalidOperationException("not value found");
            _items[matchIndex] = _items[_size - 1];
            _items[--_size] = default;
            _version++;
        }

        public bool Contains(T value)
        {
            return Array.IndexOf(_items, value) != -1;
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
            _version++;
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

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            // public static IEnumerator<T> s_emptyEnumerator;

            private readonly UnsafeUnOrderList<T> _list;
            private int _index;
            private T _current;
            private int _version;

            public Enumerator(UnsafeUnOrderList<T> list)
            {
                _list = list;
                _index = 0;
                _current = default;
                _version = list._version;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_version != _list._version)
                {
                    throw new InvalidOperationException("can not modified when enumerating");
                }

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
