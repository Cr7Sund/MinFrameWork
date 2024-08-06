using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Cr7Sund.Editor.NodeGraph
{
    public class Manifest
    {
        public class NodeBindInfo
        {
            public readonly Type NodeType;
            public readonly Type ViewType;
            public readonly Type ModelType;

            public NodeBindInfo(Type nodeType, Type viewType, Type modelType)
            {
                Assert.That(typeof(IView).IsAssignableFrom(viewType));
                Assert.That(typeof(IModel).IsAssignableFrom(modelType));
                if (nodeType != null)
                {
                    Assert.That(typeof(EditorNode).IsAssignableFrom(nodeType));
                }

                NodeType = nodeType;
                ViewType = viewType;
                ModelType = modelType;
            }
        }

        public Dictionary<string, NodeBindInfo> dict = new();

        public Manifest()
        {
            dict.Add(nameof(GraphNode), new NodeBindInfo(
                typeof(GraphNode),
                typeof(GraphView),
                typeof(GraphModel)
            ));

            dict.Add(nameof(NodeController), new NodeBindInfo(
                typeof(NodeController),
                typeof(NodeView),
                typeof(NodeModel)
            ));

            dict.Add(nameof(EdgeNode), new NodeBindInfo(
                typeof(EdgeNode),
                typeof(EdgeView),
                typeof(EdgeModel)
            ));


            dict.Add(nameof(PortNode), new NodeBindInfo(
                typeof(PortNode),
                typeof(PortView),
                typeof(PortInfo)
            ));

            dict.Add(nameof(PortListNode), new NodeBindInfo(
                typeof(PortListNode),
                typeof(PortListView),
                typeof(PortListInfo)
            ));

            dict.Add(nameof(ContextMenuNode), new NodeBindInfo(
                typeof(ContextMenuNode),
                typeof(ContextMenu),
                typeof(ContextInfo)
            ));

            dict.Add(nameof(BlackboardNode), new NodeBindInfo(
                typeof(BlackboardNode),
                typeof(BlackboardView),
                typeof(BlackboardInfo)
            ));

            dict.Add(nameof(NodeParamsController), new NodeBindInfo(
                typeof(NodeParamsController),
                typeof(NodeParamsView),
                typeof(NodeParamsInfo)
            ));
        }

        internal bool TryGetValue(string nodeName, out NodeBindInfo bindInfo)
        {
            return dict.TryGetValue(nodeName, out bindInfo);
        }
    }
}