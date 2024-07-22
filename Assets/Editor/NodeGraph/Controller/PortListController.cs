using NUnit.Framework;
using UnityEditor.GraphView;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{
    public class PortListController : EditorNode
    {
        public PortListView portListView => view as PortListView;
        public PortListInfo portListInfo => modelData as PortListInfo;

        [Inject] private InspectorInfo _inspectorInfo;


        public PortListController(IModel model) : base(model)
        {
        }


        protected override IView CreateView()
        {
            return new PortListView();
        }

        protected override EditorNode CreateChildNode(IModel model)
        {
            if (model is PortInfo portInfo)
            {
                return new PortController(model);
            }

            throw new System.NotImplementedException();
        }

        protected override void OnBindUI()
        {
            Assert.NotNull(portListView);
            Assert.NotNull(portListInfo);

            portListView.UpdateHeader(portListInfo.portListName);
            portListView.bindItem = BindItem;
            portListView.itemIndexChanged += portListInfo.Swap;
            portListView.itemsSource = portListInfo.portInfos;
        }

        private void BindItem(VisualElement rowItem, int index)
        {
            var portView = rowItem.Q<PortView>();

            Assert.NotNull(portView);

            int id = 1000 + index;
            var parentNodeModel = _parent.modelData as NodeModel;
            var portInfo = portListInfo.TryAddPort(parentNodeModel, id);
            portView.UpdatePort(portInfo, _inspectorInfo.Orientation);

            // late view binding
            if (ChildNodes.Count <= index)
            {
                PortController childNode = new PortController(portListInfo.portInfos[index])
                {
                    view = portListView.portViews[index]
                };
                AddChildAsync(childNode);
            }
            else
            {
                var childNode = ChildNodes[index] as PortController;
                childNode.view = portListView.portViews[index];

                var parentNode = _parent as NodeController;
                var eventData = new AddPortListEvent();
                eventData.edgeView = portListView.portViews[index];
                eventData.portInfo = childNode.portInfo;
                eventData.containerNode = parentNode.nodeModel;
                _eventBus.Dispatch(eventData);
            }

        }
    }
}