namespace Cr7Sund.Editor.NodeGraph
{
    public class NodeParamsController : EditorNode
    {
        public NodeParamsView ParamsView => view as NodeParamsView;

        
        public NodeParamsController(IModel model) : base(model)
        {
        }

        protected override IView CreateView()
        {
            return new NodeParamsView(modelData);
        }
    }
}