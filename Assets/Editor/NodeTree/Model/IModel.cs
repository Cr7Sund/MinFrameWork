using System;
using UnityEditor;

namespace Cr7Sund.Editor.NodeTree
{
    public interface IModel
    {
        SerializedProperty serializedProperty { get; set; }
        void IterateChildNodes(Action<IModel, int> action);
        SerializedProperty OnBindSerializedProperty(SerializedProperty parentSerializedProperty, int index);
    }


}