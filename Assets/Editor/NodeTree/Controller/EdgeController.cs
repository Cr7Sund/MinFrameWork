using System;
using UnityEditor.GraphView;

namespace Cr7Sund.Editor.NodeTree
{
    public class EdgeController : EditorNode
    {
        public readonly EdgeView edgeView;
        public EdgeModel edgeModel => modelData as EdgeModel;


        public EdgeController(IModel model, Edge baseEdge) : base(model)
        {
            edgeView = new EdgeView(baseEdge, model as EdgeModel);
        }

        protected override EditorNode CreateChildNode(IModel model)
        {
            throw new NotImplementedException();
        }

        protected override IView CreateView()
        {
            return edgeView;
        }
    }
}