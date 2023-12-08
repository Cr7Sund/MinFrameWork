using Cr7Sund.Framework.Api;
using System;
using System.Reflection;
namespace Cr7Sund.Framework.Impl
{
    public class ReflectedClass : IReflectedClass
    {
        public bool PreGenerated { get; set; }
        public ConstructorInfo Constructor { get; set; }
        public int ConstructorParameterCount { get; set; }
        public MethodInfo PostConstructor { get; set; }
        public Tuple<Type, object, FieldInfo>[] Fields { get; set; }
    }
}
