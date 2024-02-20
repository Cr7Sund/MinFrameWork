using System;
using System.Runtime.CompilerServices;
using Cr7Sund.Package.Api;
using Cr7Sund.FrameWork.Util;
namespace Cr7Sund.Package.Impl
{
    public class SemiBinding : ISemiBinding
    {
        protected object[] _objectValue;
        private int _size;
        private int _capacity;
        private const int DefaultCapacity = 1;
        private const int DefaultDoubleCapacity = 4;
        private const int MaxLength = 1024;
        private static readonly object[] _emptyArray = new object[0];

        public BindingConstraintType Constraint { get; set; }
        public bool UniqueValue { get; set; }

        public virtual object Value
        {
            get
            {
                if (Constraint == BindingConstraintType.ONE)
                {
                    return _objectValue?[0];
                }
                return _objectValue;
            }
        }
        public int Count => _size;

        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                    throw new IndexOutOfRangeException();
                return _objectValue[index];
            }
            set
            {
                if (index < 0 || index >= _size)
                    throw new IndexOutOfRangeException();
                _objectValue[index] = value;
            }
        }
        public object SingleValue
        {
            get
            {
                return _objectValue == null || _objectValue.Length == 0 ? null : _objectValue[0];
            }
        }
        public PoolInflationType InflationType { get; set; }
        public int MaxSize { get; set; }
        public int Capacity => _capacity;

        public SemiBinding()
        {
            Constraint = BindingConstraintType.ONE;
            InflationType = PoolInflationType.INCREMENT;
            MaxSize = MaxLength;
            UniqueValue = true;
            _objectValue = _emptyArray;
        }

        #region IManagedList implementation
        public IManagedList Add(object o)
        {

            if (Constraint == BindingConstraintType.ONE)
            {
                if (_objectValue.Length == 0)
                {
                    _objectValue = new object[1];
                    _size = 1;
                }
                _objectValue[0] = o;
            }
            else
            {
                if(Contains(o)) return this;

                if (_size >= _objectValue.Length)
                {
                    AddWithResize();
                }

                _objectValue[_size++] = o;

            }

            return this;
        }

        public IManagedList Add(object[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Add(list[i]);
            }
            return this;
        }

        public IManagedList Remove(object o)
        {
            if (o.Equals(_objectValue) || _objectValue == null)
            {
                _objectValue = null;
                return this;
            }

            int matchIndex = Array.IndexOf(_objectValue, o);
            if (matchIndex != ArrayExt.UNMATCHINDEX)
            {
                _objectValue.SpliceValueAt(matchIndex, ref _size);
            }

            return this;

        }

        public IManagedList Remove(object[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Remove(list[i]);
            }
            return this;
        }

        public void Dispose()
        {
            _size = 0;
            _capacity = 0;
            _objectValue = _emptyArray;
        }

        public bool Contains(object o)
        {
            if (UniqueValue)
            {
                return _objectValue.Contains(o);
            }
            else
            {
                return false;
            }
        }

        private void AddWithResize()
        {
            _capacity = GetNewCapacity();

            if (_capacity != _objectValue.Length)
            {
                var newItems = new object[_capacity];
                Array.Copy(_objectValue, newItems, _size);
                _objectValue = newItems;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetNewCapacity()
        {
            int newCapacity = _objectValue.Length == 0 ?
                      (InflationType == PoolInflationType.DOUBLE ? DefaultDoubleCapacity : DefaultCapacity) :
                      (InflationType == PoolInflationType.DOUBLE ? 2 * _objectValue.Length : _objectValue.Length + 1);

            if (newCapacity > MaxSize) throw new MyException(BinderExceptionType.BINDING_LIMIT);
            return newCapacity;
        }

        #endregion


        #region ISemiBinding implementation

        public object[] Clone()
        {
            object[] resultArray = new object[_size];
            Array.Copy(_objectValue, resultArray, _size);
            return resultArray;
        }
        #endregion
    }
}
