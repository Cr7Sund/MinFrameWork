using System;

namespace Cr7Sund.Editor
{
    /// <summary>
    /// Set a custom drawer for a field. It can then be created using the FieldFactory
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FieldDrawerAttribute : Attribute
    {
        public Type fieldType;

        /// <summary>
        /// Register a custom view for a type in the FieldFactory class
        /// </summary>
        /// <param name="fieldType"></param>
        public FieldDrawerAttribute(Type fieldType)
        {
            this.fieldType = fieldType;
        }
    }
}