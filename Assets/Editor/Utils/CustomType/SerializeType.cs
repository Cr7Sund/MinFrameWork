using System;
using System.Reflection;

namespace Cr7Sund.Editor
{
    [System.Serializable]
    public struct SerializeType
    {
        public string assemblyName;
        public string fullTypeName;
        public Type type;

        public Type GetSerialType()
        {
            if (type != null) return type;

            var assembly = Assembly.Load(assemblyName);
            type = assembly.GetType(fullTypeName);
            return type;
        }

        public SerializeType(Type type)
        {
            this.type = type;
            assemblyName = type.Assembly.FullName;
            fullTypeName = type.FullName;
        }

        public SerializeType(SerializeType serializeType) 
        {
            this.fullTypeName = serializeType.fullTypeName;
            assemblyName = serializeType.assemblyName;
            this.type = serializeType.type;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(assemblyName);
        }

        public void Clear()
        {
            assemblyName = string.Empty;
            fullTypeName = string.Empty;
        }

        public override int GetHashCode()
        {
            return assemblyName.GetHashCode() + fullTypeName.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is SerializeType otherSerialType)
            {
                return assemblyName.Equals(otherSerialType.assemblyName) && fullTypeName.Equals(otherSerialType.fullTypeName);
            }
            else
            {
                return base.Equals(obj);
            }
        }
    }

}