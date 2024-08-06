using System;

namespace Cr7Sund.Editor.NodeGraph
{
    public class NodeParamsController : EditorNode
    {
        public NodeParamsView ParamsView => View as NodeParamsView;


        public NodeParamsController(IModel model) : base(model)
        {
        }

        protected override IView CreateView()
        {
            if (_manifest.TryGetValue(nameof(NodeParamsController), out var outPut))
            {
                return Activator.CreateInstance(outPut.ViewType, modelData) as IView;
            }
            return base.CreateView();
        }
    }
}