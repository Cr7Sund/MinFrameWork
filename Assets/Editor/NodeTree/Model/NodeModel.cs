using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cr7Sund.FrameWork.Util;
using UnityEditor;
using UnityEditor.GraphView;
using UnityEngine;

namespace Cr7Sund.Editor.NodeTree
{
    [Serializable]
    public class NodeModel : IModel
    {
        [SerializeField]
        [HideInInspector]
        private float nodeX, nodeY;
        [SerializeField]
        public string name;
        [SerializeField]
        [HideInInspector]
        private string id;
        [SerializeField]
        [HideInInspector]
        private string typeName;
        [SerializeField]
        [HideInInspector]
        private string assemblyName;
        [HideInInspector]
        public List<PortInfo> portInfos = new();
        public PortListInfo portListInfo;

        [HideInInspector]
        public bool isUtilityNode;
        private static Dictionary<Type, NodeAttribute> nodeInfo = new Dictionary<Type, NodeAttribute>();

        public SerializedProperty serializedProperty { get; set; }
        public Type NodeType => Assembly.Load(assemblyName).GetType(typeName);


        public NodeModel()
        {
            id = Guid.NewGuid().ToString();
        }

        public void IterateChildNodes(Action<IModel, int> action)
        {
            for (int i = 0; i < portInfos.Count; i++)
            {
                action?.Invoke(portInfos[i], i);
            }

            if (ContainPortList(NodeType))
            {
                action?.Invoke(portListInfo, 0);
            }
        }

        public void MapSerializePorts(List<PortInfo> ports)
        {
            var portProp = serializedProperty.FindPropertyRelative("portInfos");
            portProp.ClearArray();
            for (int i = 0; i < ports.Count; i++)
            {
                portProp.InsertArrayElementAtIndex(i);
                var portListElementProp = portProp.GetArrayElementAtIndex(i);
                PortInfo.MapSerializePort(ports[i], portListElementProp);
            }
        }

        public static NodeAttribute GetNodeAttribute(Type type)
        {
            if (!nodeInfo.ContainsKey(type))
            {
                nodeInfo.Add(type, Attribute.GetCustomAttribute(type, typeof(NodeAttribute)) as NodeAttribute);
            }
            return nodeInfo[type];
        }

        public Vector2 GetPosition()
        {
            return new Vector2(nodeX, nodeY);
        }

        public void SetPosition(Vector2 newPos)
        {
            nodeX = newPos.x;
            nodeY = newPos.y;
        }

        private bool ContainPortList(Type nodeType)
        {
            var fields = nodeType.GetFields(BindingFlags.Public
                         | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in fields)
            {
                var outAttribute = item.GetCustomAttribute<OutPortAttribute>();
                if (outAttribute != null)
                {
                    if (ReflectUtil.IsCollection(item.FieldType))
                    {
                        // var elementType = ReflectUtil.GetElementType(item.FieldType);
                        return true;
                    }
                }
            }

            return false;
        }

        public static NodeModel CreateNodeModel(Type nodeType, Vector2 pos, bool isUtilityNode)
        {
            var nodeModel = new NodeModel();
            nodeModel.nodeX = pos.x;
            nodeModel.nodeY = pos.y;
            nodeModel.typeName = nodeType.FullName;
            nodeModel.assemblyName = nodeType.Assembly.FullName;
            nodeModel.name = nodeType.Name;
            nodeModel.isUtilityNode = isUtilityNode;

            var fields = nodeType.GetFields(BindingFlags.Public
            | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in fields)
            {
                var port = new PortInfo();

                var inAttribute = item.GetCustomAttribute<InPortAttribute>();
                if (inAttribute != null)
                {
                    port.portName = item.Name;
                    if (!string.IsNullOrEmpty(inAttribute.DisplayName))
                    {
                        port.portName = inAttribute.DisplayName;
                    }
                    port.direction = Direction.Input;
                    port.id = nodeModel.portInfos.Where(p => p.direction == Direction.Input).Count();
                    port.fullTypeName = item.FieldType.FullName;
                    port.assemblyName = item.FieldType.Assembly.FullName;

                    nodeModel.portInfos.Add(port);

                }

                var outAttribute = item.GetCustomAttribute<OutPortAttribute>();
                if (outAttribute != null)
                {
                    port.portName = item.Name;
                    if (!string.IsNullOrEmpty(outAttribute.DisplayName))
                    {
                        port.portName = outAttribute.DisplayName;
                    }
                    port.direction = Direction.Output;
                    port.id = nodeModel.portInfos.Where(p => p.direction == Direction.Output).Count();
                    port.fullTypeName = item.FieldType.FullName;
                    port.assemblyName = item.FieldType.Assembly.FullName;

                    nodeModel.portInfos.Add(port);

                    // port list, output
                    if (ReflectUtil.IsCollection(item.FieldType))
                    {
                        var elementType = ReflectUtil.GetElementType(item.FieldType);
                        nodeModel.portListInfo = new PortListInfo(port.portName, elementType.FullName, elementType.Assembly.FullName, port.direction);
                    }
                }
            }
            return nodeModel;
        }

        /// <summary>
        /// equal with unique guid
        /// since unity serialize reference will lost reference accidentally
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is NodeModel nodeModel)
            {
                return id.Equals(nodeModel.id);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public SerializedProperty OnBindSerializedProperty(SerializedProperty parentSerializedProperty, int index)
        {
            SerializedProperty listProperty = parentSerializedProperty.FindPropertyRelative("utilityNodes");
            if (!isUtilityNode)
            {
                listProperty = parentSerializedProperty.FindPropertyRelative("nodes");
            }

            return listProperty.GetArrayElementAtIndex(index);
        }

        public PortInfo GetPortByIndex(int id, Direction direction)
        {
            var portInfo = getPortByIndex(portInfos, id, direction);
            if (portInfo == null)
            {
                return getPortByIndex(portListInfo.portInfos, id, direction);
            }
            return portInfo;
        }

        private static PortInfo getPortByIndex(List<PortInfo> portInfos, int id, Direction direction)
        {
            foreach (var portInfo in portInfos)
            {
                if (portInfo.direction == direction
                    && portInfo.id == id)
                {
                    return portInfo;
                }
            }

            return null;
        }
    }
}