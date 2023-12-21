using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using System.Collections.Generic;
namespace Cr7Sund.Framework.Impl
{
    public class Binder : IBinder
    {

        /// A handler for resolving the nature of a binding during chained commands
        public delegate void BindingResolver(IBinding binding, object oldName = null);
        protected Dictionary<object, List<IBinding>> _bindings; // object is implicitly equal to type
        protected BindingResolver _bindingResolverHandler;


        public Binder()
        {
            _bindings = new Dictionary<object, List<IBinding>>();
            _bindingResolverHandler = Resolver;
        }

        #region IBinder implementation
        public IBinding Bind<T>()
        {
            return Bind(typeof(T));
        }

        public IBinding Bind(object key)
        {
            // if (MacroDefine.IsDebug)
            // {
            //     if (key.GetType().IsValueType)
            //     {
            //         throw new MyException($"{key} is not referenceType", BinderExceptionType.EXIST_BOXING);
            //     }
            // }

            IBinding binding;
            binding = GetRawBinding();
            binding.Bind(key);
            return binding;
        }

        protected virtual IBinding GetRawBinding()
        {
            return new Binding(_bindingResolverHandler);
        }

        private void Resolver(IBinding binding, object oldName = null)
        {
            var key = binding.Key;
            if (binding.KeyConstraint == BindingConstraintType.ONE)
            {
                ResolveBinding(binding, binding.Key.SingleValue);
            }
            else
            {
                for (int a = 0; a < key.Count; a++)
                {
                    ResolveBinding(binding, key[a]);
                }
            }
        }

        public IBinding GetBinding<T>(object name = null)
        {
            var key = typeof(T);
            return GetBinding(key, name);
        }

        public IBinding GetBinding(object key)
        {
            return GetBinding(key, null);
        }

        public IBinding GetBinding(object key, object name)
        {
            if (_bindings.TryGetValue(key, out var list))
            {
                name = name == null ? BindingConst.NULLOIDNAME : name;

                for (int i = 0; i < list.Count; i++)
                {
                    IBinding item = list[i];
                    if (item.Name.Equals(name))
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        public void Unbind(object key)
        {
            Unbind(key, null);
        }

        public void Unbind<T>(object name = null)
        {
            Unbind(typeof(T), name);
        }

        public void Unbind(IBinding binding)
        {
            if (binding == null)
            {
                return;
            }
            for (int i = 0; i < binding.Key.Count; i++)
            {
                Unbind(binding.Key[i], binding.Name);
            }
        }

        public void Unbind(object key, object name)
        {
            if (_bindings.TryGetValue(key, out var list))
            {
                name = name == null ? BindingConst.NULLOIDNAME : name;

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    IBinding item = list[i];
                    if (item.Name.Equals(name))
                    {
                        OnUnbind(item);

                        list.RemoveAt(i);

                        if (list.Count == 0)
                        {
                            _bindings.Remove(key);
                        }
                        break;
                    }
                }
            }
        }

        public void RemoveValue(IBinding binding, object value)
        {
            if (binding == null || value == null)
            {
                return;
            }
            object key = binding.Key.SingleValue;
            if (_bindings.TryGetValue(key, out var dict))
            {
                for (int i = dict.Count - 1; i >= 0; i--)
                {
                    UpdateBindingAndCleanup(binding, value, dict, i);
                }
            }

            void UpdateBindingAndCleanup(IBinding binding, object value, List<IBinding> dict, int i)
            {
                IBinding useBinding = dict[i];
                if (useBinding.Name.Equals(binding.Name))
                {
                    useBinding.RemoveValue(value);

                    //If result is empty, clean it out
                    if (useBinding.Value.Count == 0)
                    {
                        dict.RemoveAt(i);
                    }
                }
            }
        }

        public virtual void ResolveBinding(IBinding binding, object key, object oldName = null)
        {
            object bindingName = binding.Name;
            object removeName = oldName == null ? BindingConst.NULLOIDNAME : oldName;

            if (_bindings.TryGetValue(key, out var list))
            {
                RemoveConflictingBinding(binding, bindingName, list);
            }
            else
            {
                list = new List<IBinding>();
                _bindings[key] = list;
            }

            RemoveBindingByOldName(binding, removeName, list);
            AddBindingIfNotExists(binding, list, bindingName);

            void RemoveConflictingBinding(IBinding binding, object bindingName, List<IBinding> list)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    IBinding existingBinding = list[i];
                    if (existingBinding.Name != bindingName) continue;
                    if (existingBinding != binding)
                    {
                        if (!existingBinding.IsWeak)
                        {
                            throw new MyException($"there exist same binding key: {binding.Key.SingleValue} name: {binding.Name} ", BinderExceptionType.CONFLICT_IN_BINDER);
                        }

                        list.RemoveAt(i);
                        break;
                    }
                }
            }

            void RemoveBindingByOldName(IBinding binding, object removeName, List<IBinding> list)
            {
                //Remove nulled bindings
                // e.g. when we toname to change the binding name but the binderDict still exist the oldName
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    IBinding existingBinding = list[i];
                    if (existingBinding.Name.Equals(removeName) && existingBinding == binding)
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }

            void AddBindingIfNotExists(IBinding binding, List<IBinding> list, object bindingName)
            {
                bool shouldAdd = true;
                for (int i = 0; i < list.Count; i++)
                {
                    IBinding existingBinding = list[i];
                    if (existingBinding.Name.Equals(bindingName))
                    {
                        shouldAdd = false;
                        break;
                    }
                }
                if (shouldAdd)
                {
                    list.Add(binding);
                }
            }
        }

        public virtual void OnRemove()
        {
        }

        protected virtual void OnUnbind(IBinding binding)
        {
        }

        public virtual void Dispose()
        {

        }
        #endregion
    }
}
