using System;
using System.Collections.Generic;
using UnityEditor;

namespace Cr7Sund.Editor.NodeTree
{
    [System.Serializable]
    public class GraphModel : IModel
    {
        public List<NodeModel> nodes = new List<NodeModel>();
        public List<NodeModel> utilityNodes = new List<NodeModel>();
        public List<EdgeModel> edges = new List<EdgeModel>();
        public ContextInfo contextInfo = new ContextInfo();
        public InspectorInfo inspectorInfo = new InspectorInfo();

        public SerializedProperty serializedProperty { get; set; }
        public SerializedObject serializedObject { get; private set; }

        public void AssignSerializeObject(SerializedObject serializedObject)
        {
            this.serializedObject = serializedObject;
        }

        public NodeModel AddNode(NodeModel nodeModel)
        {
            if (!nodeModel.isUtilityNode)
            {
                nodes.Add(nodeModel);
            }
            else
            {
                utilityNodes.Add(nodeModel);
            }

            // we need to do list adding serialization manually first 
            var nodeListProp = serializedProperty.FindPropertyRelative("nodes");
            nodeListProp.InsertArrayElementAtIndex(nodeListProp.arraySize);

            var nodeProp = nodeListProp.GetArrayElementAtIndex(nodeListProp.arraySize - 1);
            nodeModel.serializedProperty = nodeProp;
            nodeModel.MapSerializePorts(nodeModel.portInfos);
            SerializedPropertyHelper.ReflectProp(nodeModel, nodeProp);

            return nodeModel;
        }

        public void AddEdge(EdgeModel edgeModel)
        {
            if (!edges.Contains(edgeModel))
            {
                var edgeProp = serializedProperty.FindPropertyRelative("edges");
                edgeProp.InsertArrayElementAtIndex(edgeProp.arraySize);

                var copyProp = edgeProp.GetArrayElementAtIndex(edgeProp.arraySize - 1);
                // copyProp.FindPropertyRelative("inputNode").managedReferenceValue = edgeModel.inputNode;
                // copyProp.FindPropertyRelative("outputNode").managedReferenceValue = edgeModel.outputNode;

                edges.Add(edgeModel);
            }
        }

        public void RemoveEdge(EdgeModel edgeModel)
        {
            if (edges.Contains(edgeModel))
            {
                edges.Remove(edgeModel);
            }
        }

        public void IterateChildNodes(Action<IModel, int> action)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                NodeModel item = nodes[i];
                action?.Invoke(item, i);
            }

            for (int i = 0; i < utilityNodes.Count; i++)
            {
                NodeModel item = utilityNodes[i];
                action?.Invoke(item, i);
            }

            for (int i = 0; i < edges.Count; i++)
            {
                EdgeModel edgeItem = edges[i];
                action?.Invoke(edgeItem, i);
            }

            action?.Invoke(contextInfo, 0);
            action?.Invoke(inspectorInfo, 0);
        }

        public void RemoveNode(NodeModel node)
        {
            if (!node.isUtilityNode)
            {
                nodes.Remove(node);

            }
            else
            {
                utilityNodes.Remove(node);
            }
        }

        public bool HasCopyNodes()
        {
            return false;
        }

        public SerializedProperty OnBindSerializedProperty(SerializedProperty parentSerializedProperty, int index)
        {
            return null;
        }

    }

}