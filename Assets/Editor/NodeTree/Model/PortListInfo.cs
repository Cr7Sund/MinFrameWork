using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.GraphView;
using UnityEngine;

namespace Cr7Sund.Editor.NodeTree
{
    [Serializable]
    public class PortListInfo : IModel
    {
        public List<PortInfo> portInfos = new();
        [HideInInspector] public string portListName;
        public string portName;
        public string typeName;
        public string assemblyName;
        public Direction direction;

        public SerializedProperty serializedProperty { get; set; }


        public PortListInfo(string portListName, string typeName, string assemblyName, Direction direction)
        {
            this.portListName = portListName;
            this.typeName = typeName;
            this.assemblyName = assemblyName;
            this.direction = direction;
        }

        public void IterateChildNodes(Action<IModel, int> action)
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

        public SerializedProperty OnBindSerializedProperty(SerializedProperty parentSerializedProperty, int index)
        {
            return parentSerializedProperty.FindPropertyRelative("portListInfo");
        }

        public void Swap(int draggedIndex, int dropIndex)
        {
            var tmp = portInfos[draggedIndex];
            portInfos[draggedIndex] = portInfos[dropIndex];
            portInfos[dropIndex] = tmp;
        }

        public PortInfo TryAddPort(int id)
        {
            portInfos[portInfos.Count - 1] = new PortInfo()
            {
                portName = $"Element_{id}",
                fullTypeName = typeName,
                assemblyName = assemblyName,
                id = id,
                direction = direction
            };

            return portInfos[portInfos.Count - 1];
        }
    }
}