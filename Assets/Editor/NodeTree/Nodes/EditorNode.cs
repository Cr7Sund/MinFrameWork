using System.Collections.Generic;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using GraphProcessor;
using UnityEditor.SearchService;

namespace Cr7Sund.NodeTree.Editor
{
    [System.Serializable]
    public class EditorNode : BaseNode
    {
        public INode runNode;

        [Input(name = "Parent")]
        public INode ParentNode;

        [Output(name = "Child")]
        public List<INode> ChildNodes = new();


        public EditorNode(Node node) : base()
        {
            runNode = node;
            ParentNode = node.Parent;
        }

        public void OnUpdateChild()
        {
            ParentNode = runNode.Parent;
            ChildNodes.Clear();
            for (int i = 0; i < runNode.ChildCount; i++)
            {
                var node = runNode.GetChild(i);
                ChildNodes.Add(node);
            }
        }

    }
}