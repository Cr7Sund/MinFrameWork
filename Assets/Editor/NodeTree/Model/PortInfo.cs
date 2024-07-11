using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.GraphView;
using UnityEngine.Serialization;

namespace Cr7Sund.Editor.NodeTree
{
    [Serializable]
    public class PortInfo : IModel
    {
        public string portName;
        public string fullTypeName;
        public string assemblyName;
        [FormerlySerializedAs("index")]
        public int id;
        public Orientation orientation;
        public Direction direction;
        public PortCapacity capacity;

        public SerializedProperty serializedProperty { get; set; }


        public SerializedProperty OnBindSerializedProperty(SerializedProperty parentSerializedProperty, int index)
        {
            return parentSerializedProperty.FindPropertyRelative("portInfos").GetArrayElementAtIndex(index);
        }

        public void IterateChildNodes(Action<IModel, int> action)
        {
        }

        public Type GetPortType()
        {
            Type type = typeof(int).Assembly.GetType(fullTypeName);
            if (type != null)
            {
                return type;
            }
            var assembly = Assembly.Load(assemblyName);
            return assembly.GetType(fullTypeName);
        }
        
        public static void MapSerializePort(PortInfo port, SerializedProperty serializedProperty)
        {
            SerializedPropertyHelper.ReflectProp(port, serializedProperty.FindPropertyRelative(nameof(portName)));
        }

    }
}