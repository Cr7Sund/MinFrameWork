using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using System;
namespace Cr7Sund.Framework.Impl
{
    public class SemiBinding : ISemiBinding
    {
        protected object[] _objectValue;
        public SemiBinding()
        {
            Constraint = BindingConstraintType.ONE;
            UniqueValue = true;
        }
        public BindingConstraintType Constraint { get; set; }
        public bool UniqueValue { get; set; }

        #region ISemiBinding implementation
        public virtual object Value
        {
            get
            {
                if (Constraint.Equals(BindingConstraintType.ONE))
                {
                    return _objectValue?[0];
                }
                return _objectValue;
            }
        }

        public int Count
        {
            get
            {
                return _objectValue?.Length ?? 0;
            }
        }

        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= _objectValue.Length)
                    throw new IndexOutOfRangeException();
                return _objectValue[index];
            }
            set
            {
                if (index < 0 || index >= _objectValue.Length)
                    throw new IndexOutOfRangeException();
                _objectValue[index] = value;
            }
        }
        #endregion

        #region IManagedList implementation
        public IManagedList Add(object o)
        {
            if (_objectValue == null || Constraint == BindingConstraintType.ONE)
            {
                _objectValue = new object[1];
            }
            else
            {
                if (UniqueValue)
                {
                    if (Contains(o)) return this;
                }

                object[] newItems = new object[_objectValue.Length + 1];
                Array.Copy(_objectValue, newItems, _objectValue.Length);
                _objectValue = newItems;
            }

            _objectValue[_objectValue.Length - 1] = o;

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
                _objectValue = _objectValue.SpliceValueAt(matchIndex);
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

        public bool Contains(object o)
        {
            return _objectValue.Contains(o);
        }
        #endregion
    }


}
