using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class InjectionBinder : Binder, IInjectionBinder
    {
        public IInjector Injector { get; set; }

        public InjectionBinder()
        {


            Injector = new Injector
            {
                Binder = this,
            };
        }

        #region  IInstanceProvider implementation

        public object GetInstance(Type key)
        {
            return GetInstance(key, null);
        }

        public T GetInstance<T>(object name)
        {
            object instance = GetInstance(typeof(T), name);
            T retVal = (T)instance;
            return retVal;
        }

        public T GetInstance<T>()
        {
            object instance = GetInstance(typeof(T), null);
            T retVal = (T)instance;
            return retVal;
        }

        public virtual object GetInstance(Type key, object name)
        {
            var binding = GetBinding(key, name) as IInjectionBinding;
            if (binding == null)
            {
                throw new InjectionException("InjectionBinder has no binding for:\n\tkey: " + key + "\nname: " + name, InjectionExceptionType.NULL_BINDING);
            }

            var instance = GetInjectorForBinding(binding).Instantiate(binding);
            return instance;
        }

        public void ReleaseInstance(object instance, object name = null)
        {
            if (instance != null)
            {
                var binding = GetBinding(instance.GetType(), name) as IInjectionBinding;
                if (binding == null)
                {
                    throw new InjectionException("InjectionBinder has no binding for:\n\tkey: " + instance.GetType() + "\nname: " + name, InjectionExceptionType.NULL_BINDING);
                }

                GetInjectorForBinding(binding).Destroy(instance);
            }
            else
            {
                // e.g.  
                // private void AssignNull(ref GuaranteedUniqueInstances instances)
                // {
                //     instances = null;
                // }
                // AssignNull(ref instance1);
                // binder.ReleaseInstance(instance1, SomeEnum.ONE);
                throw new InjectionException("try to release an null instance", InjectionExceptionType.NULL_BINDING);
            }
        }

        protected virtual IInjector GetInjectorForBinding(IInjectionBinding binding)
        {
            return Injector;
        }

        #endregion

        #region  IBinder implementation

        protected override IBinding GetRawBinding()
        {
            return new InjectionBinding(Resolver);
        }

        public new virtual IInjectionBinding Bind<T>()
        {
            return base.Bind<T>() as IInjectionBinding;
        }

        public IInjectionBinding GetBinding(Type type)
        {
            return this.GetBinding(type, null) as IInjectionBinding;
        }

        public new IInjectionBinding GetBinding<T>(object name = null)
        {
            return this.GetBinding(typeof(T), name) as IInjectionBinding;
        }

        public virtual IInjectionBinding GetBinding(Type type, object name)
        {
            return base.GetBinding(type, name) as IInjectionBinding;
        }

        #endregion

        #region  IInjectionBinder Implementation
        public int ReflectAll()
        {
            var set = new HashSet<Type>();
            var list = new List<Type>();
            foreach (var pair in bindings)
            {
                foreach (var bPair in pair.Value)
                {
                    var binding = bPair.Value;

                    if (binding.Value is Type t)
                    {
                        if (!set.Contains(t))
                        {
                            list.Add(t);
                            set.Add(t);
                        }
                    }
                }
            }
            return Reflect(list);
        }

        public int Reflect(List<Type> list)
        {
            int count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[i];

                //Reflector won't permit primitive types, so skip them
                if (t.IsPrimitive || t == typeof(string) || t == typeof(decimal))
                {
                    continue;
                }

                count++;
                Injector.Reflector.Get(t);
            }
            return count;
        }

        #endregion


    }
}