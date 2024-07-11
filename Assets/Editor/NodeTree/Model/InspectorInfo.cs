using System;
using UnityEditor;

namespace Cr7Sund.Editor.NodeTree
{
    [System.Serializable]
    public class InspectorInfo : IModel
    {
        public Type fieldType;
        public string fieldName = null;

        public SerializedProperty serializedProperty { get; set; }

        public void IterateChildNodes(Action<IModel, int> action)
        {
        }

        public SerializedProperty OnBindSerializedProperty(SerializedProperty parentSerializedProperty, int index)
        {
            return parentSerializedProperty.FindPropertyRelative("inspectorInfo");
        }
    }
}