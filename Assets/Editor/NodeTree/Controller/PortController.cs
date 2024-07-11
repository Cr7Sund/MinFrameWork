namespace Cr7Sund.Editor.NodeTree
{
    public class PortController : EditorNode
    {
        public PortInfo portInfo => modelData as PortInfo;
        public PortView portView => view as PortView;


        public PortController(IModel model) : base(model)
        {
        }


        protected override IView CreateView()
        {
            if (_parent is PortListController portListController)
            {
                // skip it since the view "will" created by list
                return null;
            }
            return new PortView(portInfo);
        }

    }
}