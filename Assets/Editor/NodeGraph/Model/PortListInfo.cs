using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.GraphView;
using UnityEngine;

namespace Cr7Sund.Editor.NodeGraph
{
    [Serializable]
    public class PortListInfo : BaseModel
    {
        public List<PortInfo> portInfos = new();
        [HideInInspector] public string portListName;
        public string portName;
        public SerializeType serializeType;
        public Direction direction;

        public PortListInfo(BaseModel parentModel, string portListName, Type type, Direction direction) : base(parentModel)
        {
            this.portListName = portListName;
            this.serializeType = new SerializeType(type);
            this.direction = direction;
        }

        public override void IterateChildNodes(Action<IModel, int> action)
        {
            if (portInfos == null)
            {
                return;
            }

            for (int i = 0; i < portInfos.Count; i++)
            {
                action?.Invoke(portInfos[i], i);
            }
        }

        public override SerializedProperty OnBindSerializedProperty(IModel model, SerializedProperty parentSerializedProperty, int index)
        {
            return parentSerializedProperty.FindPropertyRelative("portListInfo");
        }

        public void Swap(int draggedIndex, int dropIndex)
        {
            var tmp = portInfos[draggedIndex];
            portInfos[draggedIndex] = portInfos[dropIndex];
            portInfos[dropIndex] = tmp;
        }

        public PortInfo TryAddPort(BaseModel parentModel, int id)
        {
            portInfos[portInfos.Count - 1] = new PortInfo(parentModel)
            {
                portName = $"Element_{id}",
                serializeType = new SerializeType(serializeType),
                id = id,
                direction = direction
            };

            return portInfos[portInfos.Count - 1];
        }
    }
}