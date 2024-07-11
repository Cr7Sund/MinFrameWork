using System.Collections.Generic;
using System.Linq;
using UnityEditor.GraphView;
using UnityEngine;

namespace Cr7Sund.Editor.NodeTree
{
    public class NodeController : EditorNode
    {
        public NodeView nodeView => view as NodeView;
        public NodeModel nodeModel => modelData as NodeModel;


        public NodeController(NodeModel nodeModel) : base(nodeModel)
        {
        }

        protected override EditorNode CreateChildNode(IModel model)
        {
            if (model is PortInfo)
            {
                return new PortController(model);
            }
            else if (model is PortListInfo portListInfo)
            {
                return new PortListController(model);
            }

            throw new System.NotImplementedException();
        }

        protected override IView CreateView()
        {
            NodeView nodeView = new NodeView(this);
            return nodeView;
        }

        protected override void OnBindUI()
        {
            var nodeView = view as NodeView;

            var nodeModel = modelData as NodeModel;
            nodeView.SetPosition(nodeModel.GetPosition());
        }

        public void SetPosition(Vector2 newPosition)
        {
            var nodeModel = modelData as NodeModel;
            nodeModel.SetPosition(newPosition);
        }

        public PortController GetPortByView(BasePort targetPort)
        {
            return ChildNodes
                .SelectMany(childNode => GetPortControllers(childNode))
                .FirstOrDefault(portController => portController.view == targetPort);
        }

        public PortController GetPortByIndex(Direction direction, int id)
        {
            return ChildNodes
                 .SelectMany(GetPortControllers)
                 .FirstOrDefault(portController =>
                     portController.modelData is PortInfo portInfo &&
                     portInfo.direction == direction &&
                     portInfo.id == id);
        }

        public override string ToString()
        {
            return "NodeController";
        }

        private IEnumerable<PortController> GetPortControllers(EditorNode childNode)
        {
            if (childNode is PortController portController)
            {
                yield return portController;
            }

            if (childNode is PortListController portListController)
            {
                foreach (var childPort in portListController.ChildNodes.OfType<PortController>())
                {
                    yield return childPort;
                }
            }
        }
    }
}