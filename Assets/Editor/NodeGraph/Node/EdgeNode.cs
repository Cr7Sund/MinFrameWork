using System;
using UnityEditor.GraphView;

namespace Cr7Sund.Editor.NodeGraph
{
    public class EdgeNode : EditorNode
    {
        public readonly EdgeView edgeView;
        public EdgeModel edgeModel => modelData as EdgeModel;


        public EdgeNode(IModel model, Edge baseEdge) : base(model)
        {
            edgeView = new EdgeView(baseEdge, model as EdgeModel);
        }

        protected override IView CreateView()
        {
            return edgeView;
        }
    }
}