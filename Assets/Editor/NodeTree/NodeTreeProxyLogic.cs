using System;
using Cr7Sund.Editor.NodeGraph;
using UnityEditor;

namespace Cr7Sund.Editor.NodeTree
{
    public class NodeTreeProxyLogic : BaseGraphLogic
    {
        private const string GUID = "1c01fc9f6d337fa41a9b544fa28320ff";

        public static void OpenGraph()
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(GUID);
            var graphModel = AssetDatabase.LoadAssetAtPath<ScriptableGraphModel>(assetPath);
            var graphKey = new EditorKeys(graphModel.graphModelBase, assetPath);
            graphModel.Init(new SerializedObject(graphModel));
            NodeGraphWindow.OpenGraph<NodeTreeProxyLogic, NodeTreeManifest>(graphKey);
        }

        public static void ClearGraph()
        {
            NodeGraphWindow.ClearGraph();
        }

        public override void Stop()
        {
            try
            {
                // we will handle the remove logic to nodeTree
                // so we can see the left node we don't clear
                // ------- -------
                // _graphWindowNode.Stop();
                _graphWindowNode = null;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
            }
        }
    }

    public class NodeTreeManifest : Manifest
    {
        public NodeTreeManifest() 
        {
            dict[nameof(GraphNode)] = new NodeBindInfo(
                typeof(NodeTreeGraphNode),
                typeof(GraphView),
                typeof(GraphModel)
            );
        }
    }
}
