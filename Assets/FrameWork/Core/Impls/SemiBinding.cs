using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;

namespace Cr7Sund.Framework.Impl
{
    public class SemiBinding : ISemiBinding
    {
        protected object[] objectValue;
        public BindingConstraintType Constraint { get; set; }
        public bool UniqueValue { get; set; }
        public SemiBinding()
        {
            Constraint = BindingConstraintType.ONE;
            UniqueValue = true;
        }

        #region ISemiBinding implementation

        public virtual object Value
        {
            get
            {
                if (Constraint.Equals(BindingConstraintType.ONE))
                {
                    return (objectValue == null) ? null : objectValue[0];
                }
                return objectValue;
            }
        }

        public int Count => objectValue == null ? 0 : objectValue.Length;

        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= objectValue.Length)
                    throw new IndexOutOfRangeException();
                return objectValue[index];
            }
            set
            {
                if (index < 0 || index >= objectValue.Length)
                    throw new IndexOutOfRangeException();
                objectValue[index] = value;
            }
        }

        #endregion

        #region IManagedList implementation
        public IManagedList Add(object o)
        {
            if (objectValue == null || Constraint == BindingConstraintType.ONE)
            {
                objectValue = new object[1];
            }
            else
            {
                if (UniqueValue)
                {
                    if (this.Contains(o)) return this;
                }

                var newItems = new object[objectValue.Length + 1];
                Array.Copy(objectValue, newItems, objectValue.Length);
                objectValue = newItems;
            }

            objectValue[objectValue.Length - 1] = o;

            return this;
        }

        public IManagedList Add(object[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                this.Add(list[i]);
            }
            return this;
        }

        public IManagedList Remove(object o)
        {
            if (o.Equals(objectValue) || objectValue == null)
            {
                objectValue = null;
                return this;
            }

            int matchIndex = Array.IndexOf(objectValue, o);
            if (matchIndex != CollectionUtil.UNMATCHINDEX)
            {
                this.objectValue = this.objectValue.SpliceValueAt(matchIndex);
            }

            return this;

        }

        public IManagedList Remove(object[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                this.Remove(list[i]);
            }
            return this;
        }

        public bool Contains(object o)
        {
            return this.objectValue.Contains(o);
        }

        #endregion
    }


}