using System;
using System.Linq;
using UnityEditor.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{
    public class GraphView : UnityEditor.GraphView.GraphView, IView
    {
        private Action<Actions, object> OnAction;

        public int OpenContextMenu { get; }

        public GraphView(Action<Actions, object> OnAction) : base()
        {
            this.OnAction = OnAction;
        }


        public void StartView(IView parentView)
        {
            OnViewTransformChanged += OnViewportChanged;
        }

        public void StopView(IView parentView)
        {
            OnViewTransformChanged -= OnViewportChanged;
        }

        internal int GetSelectedNodeCount()
        {
            return ContentContainer.NodesSelected.Count();
        }

        internal bool HasSelectedEdges()
        {
            return ContentContainer.EdgesSelected.Count() > 0 ? true : false;
        }

        public void ForEachNodeDo(Action<BaseNode> callback)
        {
            foreach (BaseNode node in ContentContainer.Nodes)
            {
                callback(node);
            }
        }

        public void ForEachSelectedNodeDo(Action<BaseNode> callback)
        {
            foreach (BaseNode node in ContentContainer.NodesSelected)
            {
                callback(node);
            }
        }

        public void ForEachSelectedEdgeDo(Action<Edge> callback)
        {
            foreach (BaseEdge edge in ContentContainer.EdgesSelected)
            {
                callback(edge as Edge);
            }
        }

        public Vector2 LocalToViewTransformPosition(Vector2 localMousePosition)
        {
            return new Vector2((localMousePosition.x - ContentContainer.transform.position.x) / ContentContainer.transform.scale.x, (localMousePosition.y - ContentContainer.transform.position.y) / ContentContainer.transform.scale.y);
        }

        public Vector2 GetCurrentScale()
        {
            return ContentContainer.transform.scale;
        }

        public Vector2 GetMouseViewPosition()
        {
            return LocalToViewTransformPosition(this.WorldToLocal(mousePosition));
        }

        public override void OnActionExecuted(Actions actionType, object data = null)
        {
            OnAction?.Invoke(actionType, data);
        }

        private void OnViewportChanged(GraphElementContainer contentContainer)
        {
        }



    }
}