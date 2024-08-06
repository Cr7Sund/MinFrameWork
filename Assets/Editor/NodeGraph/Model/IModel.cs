using System;
using UnityEditor;

namespace Cr7Sund.Editor.NodeGraph
{
    public interface IModel
    {
        SerializedProperty serializedProperty { get; set; }
        string Name { get; set; }

        void IterateChildNodes(Action<IModel, int> action);
        SerializedProperty OnBindSerializedProperty(IModel model, SerializedProperty parentSerializedProperty, int index);
    }

    public abstract class BaseModel : IModel
    {
        protected BaseModel _parentModel;
        public virtual string Name { get; set; }
        public SerializedProperty serializedProperty { get; set; }

        public BaseModel(BaseModel parentModel)
        {
            _parentModel = parentModel;
        }
        
        public virtual void IterateChildNodes(Action<IModel, int> action)
        {

        }

        public abstract SerializedProperty OnBindSerializedProperty(IModel model, SerializedProperty parentSerializedProperty, int index);

        protected void ApplyModification()
        {
            SerializedPropertyHelper.ReflectProp(this, serializedProperty);
        }

        internal SerializedProperty AddChildModel(IModel model, string arrayName)
        {
            var listProperty = serializedProperty.FindPropertyRelative(arrayName);
            listProperty.InsertArrayElementAtIndex(listProperty.arraySize);
            var elementProp = listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1);
            SerializedPropertyHelper.ReflectProp(model, elementProp);
            model.serializedProperty = elementProp;
            return elementProp;
        }
    }

}