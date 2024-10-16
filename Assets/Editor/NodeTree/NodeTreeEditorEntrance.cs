using System;
using Cr7Sund.Editor.NodeGraph;
using UnityEditor;

namespace Cr7Sund.Editor.NodeTree
{
    public class NodeTreeEditorEntrance : IPlayModeChange
    {
        private const string GUID = "1c01fc9f6d337fa41a9b544fa28320ff";
        public bool IsActive
        {
            get;
            set;
        }


        public void Enable()
        {
            if (IsActive)
            {
                return;
            }
            IsActive = true;

            ClearGraph();
            OpenGraph();
            return;
        }

        public void Disable()
        {
            if (!IsActive)
            {
                return;
            }
            IsActive = false;

            ClearGraph();
            return;
        }

        private void OpenGraph()
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(GUID);
            var graphModel = AssetDatabase.LoadAssetAtPath<ScriptableGraphModel>(assetPath);
            var graphKey = new EditorKeys(graphModel.graphModelBase, assetPath);
            graphModel.Init(new SerializedObject(graphModel));
            NodeGraphWindow.OpenGraph<NodeTreeManifest>(graphKey);
        }

        private void ClearGraph()
        {
            NodeGraphWindow.ClearGraph();
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
