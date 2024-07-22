using System;
using System.Collections.Generic;
using UnityEditor;

namespace Cr7Sund.Editor.NodeGraph
{
    [System.Serializable]
    public class GraphModel : BaseModel
    {
        public List<NodeModel> nodes = new List<NodeModel>();
        public List<NodeModel> utilityNodes = new List<NodeModel>();
        public List<EdgeModel> edges = new List<EdgeModel>();
        public ContextInfo contextInfo;
        public InspectorInfo inspectorInfo;

        public SerializedObject serializedObject { get; private set; }


        public GraphModel() : base(null)
        {
            contextInfo = new ContextInfo(this);
            inspectorInfo = new InspectorInfo(this);
        }

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

            string propListName = "utilityNodes";
            if (!nodeModel.isUtilityNode)
            {
                propListName = "nodes";
            }
            this.AddChildModel(nodeModel, propListName);

            return nodeModel;
        }

        public void AddEdge(EdgeModel edgeModel)
        {
            if (!edges.Contains(edgeModel))
            {
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

        public override void IterateChildNodes(Action<IModel, int> action)
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

        public void ClearAll()
        {
            nodes.Clear();
            utilityNodes.Clear();
            edges.Clear();
        }

        public bool HasCopyNodes()
        {
            return false;
        }

        public override SerializedProperty OnBindSerializedProperty(IModel model, SerializedProperty parentSerializedProperty, int index)
        {
            return null;
        }

    }

}