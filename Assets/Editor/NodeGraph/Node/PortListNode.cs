using System;
using NUnit.Framework;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{
    public class PortListNode : EditorNode
    {
        public PortListView portListView => View as PortListView;
        public PortListInfo portListInfo => modelData as PortListInfo;

        [Inject] private BlackboardInfo _blackboardInfo;


        public PortListNode(IModel model) : base(model)
        {
        }

        protected override EditorNode CreateChildNode(IModel model)
        {
            if (model is PortInfo portInfo)
            {
                // return new PortController(model);
                if (_manifest.TryGetValue(nameof(PortNode), out var outputInfo))
                {
                    return Activator.CreateInstance(outputInfo.NodeType, model) as EditorNode;
                }
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
            portView.UpdatePort(portInfo, _blackboardInfo.Orientation);

            // late view binding
            if (ChildNodes.Count <= index)
            {
                PortNode childNode = new PortNode(portListInfo.portInfos[index])
                {
                    View = portListView.portViews[index]
                };
                AddChildAsync(childNode);
            }
            else
            {
                var childNode = ChildNodes[index] as PortNode;
                childNode.View = portListView.portViews[index];

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