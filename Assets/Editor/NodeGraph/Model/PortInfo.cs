using System;
using UnityEditor;
using UnityEditor.GraphView;
using UnityEngine.Serialization;

namespace Cr7Sund.Editor.NodeGraph
{
    [Serializable]
    public class PortInfo : BaseModel
    {
        public string portName;
        public SerializeType serializeType;
        [FormerlySerializedAs("index")]
        public int id;
        public Direction direction;
        public PortCapacity capacity;

        public PortInfo(BaseModel parentModel) : base(parentModel)
        {

        }
        public override SerializedProperty OnBindSerializedProperty(IModel model, SerializedProperty parentSerializedProperty, int index)
        {
            var listProperty = parentSerializedProperty.FindPropertyRelative("portInfos");
            if (listProperty.arraySize <= index)
            {
                return _parentModel.AddChildModel(model, "portInfos");
            }
            else
            {
                return listProperty.GetArrayElementAtIndex(index);
            }
        }


        public Type GetPortType()
        {
            return serializeType.GetSerialType();
        }

        public static void MapSerializePort(PortInfo port, SerializedProperty serializedProperty)
        {
            SerializedPropertyHelper.ReflectProp(port, serializedProperty);
        }

    }
}