using System;

namespace Cr7Sund.Editor.NodeGraph
{
    public class PortNode : EditorNode
    {
        public PortInfo portInfo => modelData as PortInfo;
        public PortView portView => View as PortView;

        [Inject] private BlackboardInfo _blackboardInfo;

        public PortNode(IModel model) : base(model)
        {
        }

        protected override IView CreateView()
        {
            if (_parent is PortListNode portListController)
            {
                // skip it since the view "will" created by list
                return null;
            }
            if (_manifest.TryGetValue(nameof(PortNode), out var outPut))
            {
                return Activator.CreateInstance(outPut.ViewType, portInfo, _blackboardInfo.Orientation) as IView;
            }
            return base.CreateView();
        }

    }
}