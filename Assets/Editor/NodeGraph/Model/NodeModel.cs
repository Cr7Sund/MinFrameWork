using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cr7Sund.FrameWork.Util;
using UnityEditor;
using UnityEditor.GraphView;
using UnityEngine;

namespace Cr7Sund.Editor.NodeGraph
{
    [Serializable]
    public class NodeModel : BaseModel
    {
        [SerializeField]
        // [HideInInspector]
        private float nodeX, nodeY;
        [SerializeField]
        private string nodeName;
        [SerializeField]
        private string modelID;
        [SerializeField]
        [HideInInspector]
        private SerializeType serializeType;
        public List<PortInfo> portInfos = new();
        [HideInInspector]
        public PortListInfo portListInfo;
        public NodeParameters nodeParameter;

        [HideInInspector]
        public bool isUtilityNode;
        private static Dictionary<Type, NodeAttribute> nodeInfos = new Dictionary<Type, NodeAttribute>();

        public Type NodeType => serializeType.GetSerialType();
        public override string Name { get => nodeName; set => nodeName = value; }

        public NodeModel(BaseModel parentModel) : base(parentModel)
        {
            modelID = Guid.NewGuid().ToString();
        }

        public override void IterateChildNodes(Action<IModel, int> action)
        {
            for (int i = 0; i < portInfos.Count; i++)
            {
                action?.Invoke(portInfos[i], i);
            }

            if (ContainPortList(NodeType))
            {
                action?.Invoke(portListInfo, 0);
            }
            action?.Invoke(nodeParameter, 0);
        }

        public static NodeAttribute GetNodeAttribute(Type type)
        {
            if (!nodeInfos.ContainsKey(type))
            {
                nodeInfos.Add(type, Attribute.GetCustomAttribute(type, typeof(NodeAttribute)) as NodeAttribute);
            }
            return nodeInfos[type];
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
            if (nodeType == null) return false;

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

        public static NodeModel CreateNodeModel(BaseModel parentModel, string name, Type nodeType, Vector2 pos, bool isUtilityNode = false)
        {
            var nodeModel = new NodeModel(parentModel)
            {
                nodeX = pos.x,
                nodeY = pos.y,
                serializeType = new SerializeType(nodeType),
                nodeName = name,
                isUtilityNode = isUtilityNode
            };
            if (nodeModel.nodeParameter == null)
            {
                nodeModel.nodeParameter = new NodeParameters(parentModel);
            }

            object instance = null;
            try
            {
                instance = Activator.CreateInstance(nodeType);
            }
            catch
            {
            }

            var fields = nodeType.GetFields(BindingFlags.Public
            | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fields)
            {
                var port = new PortInfo(nodeModel);

                var inAttribute = fieldInfo.GetCustomAttribute<InPortAttribute>();
                if (inAttribute != null)
                {
                    port.portName = fieldInfo.Name;
                    if (!string.IsNullOrEmpty(inAttribute.DisplayName))
                    {
                        port.portName = inAttribute.DisplayName;
                    }
                    port.direction = Direction.Input;
                    port.id = nodeModel.portInfos.Where(p => p.direction == Direction.Input).Count();
                    port.serializeType = new SerializeType(fieldInfo.FieldType);

                    nodeModel.portInfos.Add(port);
                }

                var outAttribute = fieldInfo.GetCustomAttribute<OutPortAttribute>();
                if (outAttribute != null)
                {
                    port.portName = fieldInfo.Name;
                    if (!string.IsNullOrEmpty(outAttribute.DisplayName))
                    {
                        port.portName = outAttribute.DisplayName;
                    }
                    port.direction = Direction.Output;
                    port.id = nodeModel.portInfos.Where(p => p.direction == Direction.Output).Count();
                    port.serializeType = new SerializeType(fieldInfo.FieldType);

                    nodeModel.portInfos.Add(port);

                    // port list, output
                    if (ReflectUtil.IsCollection(fieldInfo.FieldType))
                    {
                        var elementType = ReflectUtil.GetElementType(fieldInfo.FieldType);
                        nodeModel.portListInfo = new PortListInfo(nodeModel, port.portName, elementType, port.direction);
                    }
                }

                var nodeInfoAttribute = fieldInfo.GetCustomAttribute<NodeParamsAttribute>();
                if (nodeInfoAttribute != null)
                {
                    var value = fieldInfo.GetValue(instance);
                    nodeModel.nodeParameter.TryAddValue(value, fieldInfo.FieldType, fieldInfo.Name, nodeType.Name, fieldInfo.Name, out var resultParams);
                }
            }

            return nodeModel;
        }

        public override SerializedProperty OnBindSerializedProperty(IModel model, SerializedProperty parentSerializedProperty, int index)
        {
            string propListName = "utilityNodes";
            if (!isUtilityNode)
            {
                propListName = "nodes";
            }

            SerializedProperty listProperty = parentSerializedProperty.FindPropertyRelative(propListName);
            if (listProperty.arraySize <= index)
            {
                return _parentModel.AddChildModel(model, "nodes");
            }
            else
            {
                return listProperty.GetArrayElementAtIndex(index);
            }
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

        public void AddPort(PortInfo newModel)
        {
            if (!portInfos.Contains(newModel))
            {
                portInfos.Add(newModel);
            }
        }

        public bool TryAddParams(object value, Type fieldType, string name, string disPlayName, out SerializedProperty serializedProperty)
        {
            serializedProperty = null;
            bool containsValue = nodeParameter.TryAddValue(value, fieldType, name, nodeName, disPlayName, out var valueParams);
            if (!containsValue)
            {
                serializedProperty = nodeParameter.AddSerialization(value, valueParams);
            }


            return containsValue;
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
                return modelID.Equals(nodeModel.modelID);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return modelID.GetHashCode();
        }

    }
}