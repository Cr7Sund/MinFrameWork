using System;
using System.Reflection;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class ReflectedClass : IReflectedClass
    {
        public ConstructorInfo Constructor { get; set; }
        public int ConstructorParameterCount { get; set; }
        public bool PreGenerated { get; set; }
        public MethodInfo PostConstructor { get; set; }
        public Tuple<Type, object, FieldInfo>[]  Fields { get; set; }
    }
}