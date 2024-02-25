/**
 * @class strange.extensions.reflector.impl.ReflectionBinder
 *
 * Uses System.Reflection to create `ReflectedClass` instances.
 *
 * Reflection is a slow process. This binder isolates the calls to System.Reflector
 * and caches the result, meaning that Reflection is performed only once per class.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Cr7Sund.Package.Api;
using Cr7Sund.FrameWork.Util;
using System.Collections;
namespace Cr7Sund.Package.Impl
{
    public class InjectorReflectionBinder : Binder, IReflectionBinder
    {
        public IReflectedClass Get<T>()
        {
            return Get(typeof(T));
        }

        public IReflectedClass Get(Type type)
        {
            var binding = GetBinding(type);
            IReflectedClass retVal = null;

            if (binding == null)
            {
                binding = GetRawBinding();
                var reflected = new ReflectedClass();
                MapPreferredConstructor(reflected, binding, type);
                MapPostConstructors(reflected, binding, type);
                MapFields(reflected, binding, type);
                binding.Bind(type).To(reflected);

                reflected.PreGenerated = false;
                retVal = reflected;
            }
            else
            {
                retVal = binding.Value.SingleValue as IReflectedClass;
                ((ReflectedClass)retVal).PreGenerated = true;
            }

            return retVal;
        }

        private void MapPreferredConstructor(IReflectedClass reflected, IBinding binding, Type type)
        {
            var constructor = FindPreferredConstructor(type);
            if (constructor == null)
            {
                throw new MyException("The reflector requires concrete classes.\nType " + type.Name + " has no constructor. Is it an interface?", ReflectionExceptionType.CANNOT_REFLECT_INTERFACE);
            }

            reflected.Constructor = constructor;
            reflected.ConstructorParameterCount = constructor.GetParameters().Length;
        }

        private void MapPostConstructors(IReflectedClass reflected, IBinding binding, Type type)
        {
            var methods = type.GetMethods(BindingFlags.FlattenHierarchy |
                                          BindingFlags.Public |
                                          BindingFlags.NonPublic |
                                          BindingFlags.Instance |
                                          BindingFlags.InvokeMethod);
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                object[] tagged = method.GetCustomAttributes(typeof(PostConstruct), true);
                if (tagged.Length > 0)
                {
                    if (reflected.PostConstructor == null)
                    {
                        reflected.PostConstructor = method;
                    }
                    else
                    {
                        throw new MyException("The reflector class.\nType " + type + " has more than one post constructors", ReflectionExceptionType.CANNOT_POST_CONSTRUCTS);
                    }
                }
            }
        }

        private void MapFields(IReflectedClass reflected, IBinding binding, Type type)
        {
            var pairs = new List<Tuple<Type, object, FieldInfo>>();

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            FillFields(pairs, fields);
            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                // try to use more protected or public field instead private fields
                var parentFields = type.BaseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                FillFieldsOfBaseType(pairs, parentFields);
            }

            reflected.Fields = pairs.ToArray();
        }

        private static void FillFieldsOfBaseType(List<Tuple<Type, object, FieldInfo>> pairs, FieldInfo[] parentFields)
        {
            for (int i = 0; i < parentFields.Length; i++)
            {
                FieldInfo field = parentFields[i];
                if (!field.IsPrivate)
                {
                    continue;
                }

                if (field.FieldType.IsArray)
                {
                    continue;
                }

                if (field.FieldType.IsGenericType &&
                 typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                {
                    continue;
                }

                object[] injections = field.GetCustomAttributes(typeof(Inject), true);

                if (injections.Length > 0)
                {
                    var attr = injections[0] as Inject;
                    var pointType = field.FieldType;
                    object bindingName = attr.Name;

                    var pair = new Tuple<Type, object, FieldInfo>(pointType, bindingName, field);
                    pairs.Add(pair);
                }
            }
        }

        private static void FillFields(List<Tuple<Type, object, FieldInfo>> pairs, FieldInfo[] parentFields)
        {
            for (int i = 0; i < parentFields.Length; i++)
            {
                FieldInfo field = parentFields[i];
                if (field.FieldType.IsArray)
                {
                    continue;
                }

                if (field.FieldType.IsGenericType &&
                 typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                {
                    continue;
                }

                object[] injections = field.GetCustomAttributes(typeof(Inject), true);

                if (injections.Length > 0)
                {
                    var attr = injections[0] as Inject;
                    var pointType = field.FieldType;
                    object bindingName = attr.Name;

                    var pair = new Tuple<Type, object, FieldInfo>(pointType, bindingName, field);
                    pairs.Add(pair);
                }
            }
        }


        //Look for a constructor in the order:
        //1. Only one (just return it, since it's our only option)
        //3. The constructor with the fewest parameters 
        private ConstructorInfo FindPreferredConstructor(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.FlattenHierarchy |
                                                    BindingFlags.Public |
                                                    BindingFlags.Instance |
                                                    BindingFlags.InvokeMethod);

            if (constructors.Length == 1)
            {
                return constructors[0];
            }

            int shortestLen = int.MaxValue;
            ConstructorInfo shortestConstructor = null;
            for (int i = 0; i < constructors.Length; i++)
            {
                var constructor = constructors[i];
                int len = constructor.GetParameters().Length;
                if (len < shortestLen)
                {
                    shortestLen = len;
                    shortestConstructor = constructor;
                }
            }

            return shortestConstructor;
        }
    }
}
