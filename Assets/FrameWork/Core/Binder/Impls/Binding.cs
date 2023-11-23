using Cr7Sund.Framework.Api;
namespace Cr7Sund.Framework.Impl
{
    public class Binding : IBinding
    {
        protected bool _isWeak;
        protected ISemiBinding _key;
        protected object _name;
        protected ISemiBinding _value;
        public Binder.BindingResolver resolver;

        public Binding(Binder.BindingResolver resolver)
        {
            this.resolver = resolver;

            _value = new SemiBinding();
            _key = new SemiBinding();

            ValueConstraint = BindingConstraintType.ONE;
            KeyConstraint = BindingConstraintType.ONE;

            Unique = true;
        }

        // only for unit-test binding
        public Binding() : this(null)
        {

        }


        #region IBinding implementation
        public BindingConstraintType ValueConstraint
        {
            get
            {
                return _value.Constraint;
            }
            set
            {
                _value.Constraint = value;
            }
        }

        public BindingConstraintType KeyConstraint
        {
            get
            {
                return _key.Constraint;
            }
            set
            {
                _key.Constraint = value;
            }
        }

        public bool Unique
        {
            get
            {
                return _value.UniqueValue;
            }
            set
            {
                _value.UniqueValue = value;
            }
        }

        public object Key
        {
            get
            {
                return _key.Value;
            }
        }
        public object Value
        {
            get
            {
                return _value.Value;
            }
        }

        public object Name
        {
            get
            {
                return _name == null ? BindingConst.NULLOIDNAME : _name;
            }
        }

        public bool IsWeak
        {
            get
            {
                return _isWeak;
            }
        }

        public IBinding Bind<T>()
        {
            return Bind(typeof(T));
        }

        public virtual IBinding Bind(object key)
        {
            if (BindingConst.FORBID_BOXING)
            {
                if (key.GetType().IsValueType)
                    throw new BinderException($"{key} is not referenceType", BinderExceptionType.EXIST_BOXING);
            }

            _key.Add(key);
            return this;
        }

        public IBinding To<T>()
        {
            return To(typeof(T));
        }

        public IBinding To(object value)
        {
            if (BindingConst.FORBID_BOXING)
            {
                if (value.GetType().IsValueType)
                    throw new BinderException($"{value} is not referenceType", BinderExceptionType.EXIST_BOXING);
            }

            _value.Add(value);
            if (resolver != null)
                resolver(this);
            return this;
        }

        public IBinding ToName(object o)
        {
            object toName = o == null ? BindingConst.NULLOIDNAME : o;
            object oldName = _name;

            _name = toName;
            if (resolver != null)
                resolver(this, oldName);
            return this;
        }


        public virtual void RemoveValue(object o)
        {
            _value.Remove(o);
        }


        public IBinding Weak()
        {
            _isWeak = true;
            return this;
        }
        #endregion
    }

    public class BindingConst
    {
        /// Null is an acceptable binding, but dictionaries choke on it, so we map null to this instead.
        // the reason why not use enum is boxing operation cost
        public const string NULLOIDNAME = "NULLOIDNAME";

        // if you dont want to avoid value type boxing cost
        // can switch it to true 
        public static bool FORBID_BOXING = false;
    }
}
