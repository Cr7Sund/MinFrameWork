using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;

namespace Cr7Sund.NodeTree.Editor
{
    public class NodeTreeGraphWindow : BaseGraphWindow
    {
        BaseGraph tmpGraph;

        [MenuItem("Window/NodeTreeGraph")]
        public static BaseGraphWindow OpenGraph()
        {
            var graphWindow = CreateWindow<NodeTreeGraphWindow>();

            // When the graph is opened from the window, we don't save the graph to disk
            var graphInstance = ScriptableObject.CreateInstance<BaseGraph>();
            graphWindow.tmpGraph = graphInstance;
            graphWindow.tmpGraph.hideFlags = HideFlags.HideAndDontSave;
            graphWindow.InitializeGraph(graphWindow.tmpGraph);
            graphWindow.Show();
            return graphWindow;
        }

        protected override void OnDestroy()
        {
            graphView?.Dispose();
            DestroyImmediate(tmpGraph);
        }

        protected override void InitializeWindow(BaseGraph graph)
        {
            titleContent = new GUIContent("Default Graph");

            if (graphView == null)
                graphView = new BaseGraphView(this);

            rootView.Add(graphView);
        }
    }
}