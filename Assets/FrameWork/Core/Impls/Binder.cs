using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class Binder : IBinder
    {
        protected Dictionary<object, Dictionary<object, IBinding>> bindings; // object is implicitly equal to type
        public Binder()
        {
            bindings = new Dictionary<object, Dictionary<object, IBinding>>();
        }
        #region IBinder implementation

        public IBinding Bind<T>()
        {
            return Bind(typeof(T));
        }

        public virtual IBinding Bind(object key)
        {
            if (BindingConst.FORBIDBOXING)
            {
                if (key.GetType().IsValueType)
                    throw new BinderException($"{key} is not referenceType", BinderExceptionType.EXIST_BOXING);
            }

            IBinding binding;
            binding = GetRawBinding();
            binding.Bind(key);
            return binding;
        }

        protected virtual IBinding GetRawBinding()
        {
            return new Binding(resolver);
        }

        protected void resolver(IBinding binding, object oldName = null)
        {
            object key = binding.Key;
            if (binding.KeyConstraint.Equals(BindingConstraintType.ONE))
            {
                ResolveBinding(binding, key);
            }
            else
            {
                object[] keys = key as object[];
                int aa = keys.Length;
                for (int a = 0; a < aa; a++)
                {
                    ResolveBinding(binding, keys[a]);
                }
            }
        }

        public IBinding GetBinding<T>()
        {
            var key = typeof(T);
            return this.GetBinding(key, null);
        }

        public IBinding GetBinding<T>(object name)
        {
            var key = typeof(T);
            return this.GetBinding(key, name);
        }

        public IBinding GetBinding(object key)
        {
            return this.GetBinding(key, null);
        }

        public IBinding GetBinding(object key, object name)
        {
            if (bindings.TryGetValue(key, out var dict))
            {
                name = (name == null) ? BindingConst.NULLOIDNAME : name;
                if (dict.ContainsKey(name))
                {
                    return dict[name];
                }
            }

            return null;
        }

        public void Unbind<T>()
        {
            Unbind(typeof(T), null);
        }

        public void Unbind(object key)
        {
            Unbind(key, null);
        }

        public void Unbind<T>(object name)
        {
            Unbind(typeof(T), name);
        }

        public void Unbind(object key, object name)
        {
            if (bindings.TryGetValue(key, out var dict))
            {
                name = (name == null) ? BindingConst.NULLOIDNAME : name;

                if (dict.ContainsKey(name))
                {
                    dict.Remove(name);
                }
            }
        }

        public void RemoveValue(IBinding binding, object value)
        {
            if (binding == null || value == null)
            {
                return;
            }
            object key = binding.Key;
            Dictionary<object, IBinding> dict;
            if ((bindings.ContainsKey(key)))
            {
                dict = bindings[key];
                if (dict.ContainsKey(binding.Name))
                {
                    IBinding useBinding = dict[binding.Name];
                    useBinding.RemoveValue(value);

                    //If result is empty, clean it out
                    object[] values = useBinding.Value as object[];
                    if (values == null || values.Length == 0)
                    {
                        dict.Remove(useBinding.Name);
                    }
                }
            }
        }

        public virtual void ResolveBinding(Api.IBinding binding, object key, object oldName = null)
        {
            object bindingName = binding.Name;

            if (bindings.TryGetValue(key, out var dict))
            {
                if (dict.TryGetValue(bindingName, out var existingBinding))
                {
                    if (existingBinding != binding)
                    {
                        if (existingBinding.IsWeak)
                        {
                            dict.Remove(bindingName);
                        }
                    }
                }
            }
            else
            {
                dict = new Dictionary<object, IBinding>();
                bindings[key] = dict;
            }

            //Remove nulloid bindings
            // e.g. when we toname to change the binding name but the binderDict still exist the oldName
            object removeName = oldName == null ? BindingConst.NULLOIDNAME : oldName;
            if (dict.ContainsKey(removeName) && dict[removeName] == binding)
            {
                dict.Remove(removeName);
            }

            if (!dict.ContainsKey(bindingName))
            {
                dict.Add(bindingName, binding);
            }
        }

        public virtual void OnRemove()
        {
        }

        #endregion


        /// A handler for resolving the nature of a binding during chained commands
        public delegate void BindingResolver(IBinding binding, object oldName = null);

    }
}