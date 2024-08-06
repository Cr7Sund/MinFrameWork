using UnityEditor.GraphView;
using UnityEngine;

namespace Cr7Sund.Editor.NodeGraph
{
    public class ContextMenu : GraphSearchWindowProvider, IView
    {
        public GraphView graphView;

        public void StartView(IView parentView)
        {
            if (parentView is GraphView graphView)
            {
                this.graphView = graphView;
                Initialize(graphView.shortcutHandler);
            }
        }

        public void StopView(IView parentView)
        {
        }

    }
}