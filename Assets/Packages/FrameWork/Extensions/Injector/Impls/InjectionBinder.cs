using System;
using System.Collections.Generic;
using Cr7Sund.Package.Api;
using Cr7Sund.FrameWork.Util;
namespace Cr7Sund.Package.Impl
{
    public class InjectionBinder : Binder, IInjectionBinder
    {
        public IInjector Injector { get; set; }


        public InjectionBinder()
        {
            Injector = new Injector
            {
                Binder = this
            };
        }


        #region IInstanceProvider implementation
        public object GetInstance(Type key)
        {
            return GetInstance(key, null);
        }

        public T GetInstance<T>(object name)
        {
            object instance = GetInstance(typeof(T), name);
            var retVal = (T)instance;
            return retVal;
        }

        public T GetInstance<T>()
        {
            object instance = GetInstance(typeof(T), null);
            var retVal = (T)instance;
            return retVal;
        }

        public virtual object GetInstance(Type key, object name)
        {
            var binding = GetBinding(key, name);

            if (binding == null)
            {
                throw new MyException("InjectionBinder has no binding for:\n\tkey: " + key + "\nname: " + name, InjectionExceptionType.NULL_BINDING_CREATE);
            }

            object instance = GetInjectorForBinding(binding).Instantiate(binding);
            return instance;
        }

        public void ReleaseInstance(object instance, object name = null)
        {
            AssertUtil.NotNull(instance, InjectionExceptionType.RELEASE_NULL_BINDING);

            var binding = GetBinding(instance.GetType(), name);
            if (binding == null)
            {
                throw new MyException("InjectionBinder has no binding for:\n\tkey: " + instance.GetType() + "\nname: " + name, InjectionExceptionType.NULL_BINDING_RELEASE);
            }

            GetInjectorForBinding(binding).Destroy(instance);
        }

        protected virtual IInjector GetInjectorForBinding(IInjectionBinding binding)
        {
            return Injector;
        }
        #endregion

        #region IBinder implementation
        protected override IBinding GetRawBinding()
        {
            return new InjectionBinding(_bindingResolverHandler);
        }

        public new virtual IInjectionBinding Bind<T>()
        {
            return base.Bind<T>() as IInjectionBinding;
        }

        public IInjectionBinding GetBinding(Type type)
        {
            return GetBinding(type, null);
        }

        public new IInjectionBinding GetBinding<T>(object name = null)
        {
            return GetBinding(typeof(T), name);
        }

        public virtual IInjectionBinding GetBinding(Type type, object name)
        {
            return base.GetBinding(type, name) as IInjectionBinding;
        }

        protected override void OnUnbind(IBinding binding)
        {
            if (binding is IInjectionBinding injectionBinding)
            {
                if (injectionBinding.Type != InjectionBindingType.POOL)
                {
                    injectionBinding.Dispose();
                }
            }
        }
        #endregion

        #region IInjectionBinder Implementation
        public int ReflectAll()
        {
            var set = new HashSet<Type>();
            var list = new List<Type>();
            foreach (var pair in _bindings)
            {
                foreach (IBinding binding in pair.Value)
                {
                    AddUniqueTypeToSets(set, list, binding);
                }
            }
            return Reflect(list);

            void AddUniqueTypeToSets(HashSet<Type> set, List<Type> list, IBinding binding)
            {
                if (binding.Value.SingleValue is Type t)
                {
                    if (!set.Contains(t))
                    {
                        list.Add(t);
                        set.Add(t);
                    }
                }
            }
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
