using System;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class InjectionBinding : Binding, IInjectionBinding
    {
        private InjectionBindingType _type = InjectionBindingType.DEFAULT;
        private bool _toInject = true;
        private bool _isCrossContext = false;


        public InjectionBinding(Binder.BindingResolver resolver) : base(resolver)
        {
            KeyConstraint = BindingConstraintType.MANY;
        }


        #region IInjectionBinding implementation

        public bool IsCrossContext => _isCrossContext;

        public bool IsToInject => _toInject;

        public InjectionBindingType Type { get => _type; set => _type = value; }

        public IInjectionBinding AsDefault()
        {
            //If already a value, this mapping is redundant
            if (Type == InjectionBindingType.VALUE)
            {
                return this;
            }

            _type = InjectionBindingType.DEFAULT;
            if (resolver != null)
            {
                resolver(this);
            }
            return this;
        }

        public IInjectionBinding AsPool()
        {
            //If already a value, this mapping is redundant
            if (Type == InjectionBindingType.VALUE)
            {
                return this;
            }

            _type = InjectionBindingType.POOL;
            if (resolver != null)
            {
                resolver(this);
            }
            return this;
        }

        public IInjectionBinding AsSingleton()
        {
            //If already a value, this mapping is redundant
            if (Type == InjectionBindingType.VALUE)
            {
                return this;
            }

            _type = InjectionBindingType.SINGLETON;
            if (resolver != null)
            {
                resolver(this);
            }

            return this;
        }

        public IInjectionBinding CrossContext()
        {
            _isCrossContext = true;
            if (resolver != null)
            {
                resolver(this);
            }
            return this;
        }

        public IInjectionBinding ToInject(bool value)
        {
            _toInject = value;
            return this;
        }

        #endregion

        #region  IBinding implementation 


        public new IInjectionBinding Bind<T>()
        {
            return base.Bind<T>() as IInjectionBinding;
        }

        public IInjectionBinding Bind(Type type)
        {
            return base.Bind(type) as IInjectionBinding;
        }

        public IInjectionBinding ToValue(object o)
        {
            this.Type = InjectionBindingType.VALUE;
            this.SetValue(o);
            return this;
        }

        public IInjectionBinding SetValue(object o)
        {
            var objType = o.GetType();
            ValidBindingType(objType);

            base.To(o);
            return this;
        }

        public new IInjectionBinding To<T>()
        {
            ValidBindingType(typeof(T));
            return base.To<T>() as IInjectionBinding;
        }

        public IInjectionBinding To(Type type)
        {
            ValidBindingType(type);
            return base.To(type) as IInjectionBinding;
        }

        public new IInjectionBinding ToName(object name)
        {
            return base.ToName(name) as IInjectionBinding;
        }

        public new IInjectionBinding Weak()
        {
            return base.Weak() as IInjectionBinding;
        }

        #endregion

        private void ValidBindingType(Type objType)
        {
            if (this.KeyConstraint.Equals(BindingConstraintType.ONE))
            {
                var keyType = Key as Type;
                if (keyType.IsAssignableFrom(objType) == false)
                {
                    throw new InjectionException("Injection cannot bind a value that does not extend or implement the binding type.", InjectionExceptionType.ILLEGAL_BINDING_VALUE);
                }
            }
            else
            {
                object[] keys = Key as object[];
                for (int i = 0; i < keys.Length; i++)
                {
                    var keyType = keys[i] as Type;
                    if (keyType.IsAssignableFrom(objType) == false)
                    {
                        throw new InjectionException("Injection cannot bind a value that does not extend or implement the binding type.", InjectionExceptionType.ILLEGAL_BINDING_VALUE);
                    }
                }

            }
        }

    }
}