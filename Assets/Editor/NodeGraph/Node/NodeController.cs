using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.GraphView;
using UnityEngine;

namespace Cr7Sund.Editor.NodeGraph
{

    public class NodeController : EditorNode
    {
        public NodeView nodeView => View as NodeView;
        public NodeModel nodeModel => modelData as NodeModel;

        public NodeController(NodeModel nodeModel) : base(nodeModel)
        {
        }

        protected override EditorNode CreateChildNode(IModel model)
        {
            if (model is PortInfo)
            {
                if (_manifest.TryGetValue(nameof(PortNode), out var outputInfo))
                {
                    return Activator.CreateInstance(outputInfo.NodeType, model) as EditorNode;
                }
                // return new PortController(model);
            }
            else if (model is PortListInfo portListInfo)
            {
                if (_manifest.TryGetValue(nameof(PortListNode), out var outputInfo))
                {
                    return Activator.CreateInstance(outputInfo.NodeType, model) as EditorNode;
                }
                // return new PortListController(model);
            }
            else if (model is NodeParamsInfo nodeInfo)
            {
                if (_manifest.TryGetValue(nameof(NodeParamsController), out var outputInfo))
                {
                    return Activator.CreateInstance(outputInfo.NodeType, model) as EditorNode;
                }
                // return new NodeParamsController(model);
            }


            throw new System.Exception($"can not create child node of {model.GetType()}");
        }

        protected override IView CreateView()
        {
            if (_manifest.TryGetValue(nameof(NodeController), out var outPut))
            {
                return Activator.CreateInstance(outPut.ViewType, this) as IView;
            }
            return base.CreateView();
        }

        protected override void OnBindUI()
        {
            var nodeView = View as NodeView;

            var nodeModel = modelData as NodeModel;
            nodeView.SetPosition(nodeModel.GetPosition());
        }

        public void SetPosition(Vector2 newPosition)
        {
            var nodeModel = modelData as NodeModel;
            nodeModel.SetPosition(newPosition);
            nodeView.SetPosition(newPosition);
        }

        public PortNode GetPortByView(BasePort targetPort)
        {
            return ChildNodes
                .SelectMany(childNode => GetPortControllers(childNode))
                .FirstOrDefault(portController => portController.View == targetPort);
        }

        public PortNode GetPortByIndex(Direction direction, int id)
        {
            return ChildNodes
                 .SelectMany(GetPortControllers)
                 .FirstOrDefault(portController =>
                     portController.modelData is PortInfo portInfo &&
                     portInfo.direction == direction &&
                     portInfo.id == id);
        }

        public PortNode AddPort(PortInfo portInfo)
        {
            var newPort = new PortNode(portInfo);
            nodeModel.AddPort(portInfo);
            AddChildAsync(newPort);
            return newPort;
        }

        public void TryAddNodeParams(object value, Type fieldType, string name, string disPlayName)
        {
            bool containsValue = nodeModel.TryAddParams(value, fieldType, name, disPlayName, out var serialProp);
            if (!containsValue)
            {
                ChildNodes.OfType<NodeParamsController>()
                         .ToList()
                         .ForEach(nodeParamCtrl => nodeParamCtrl.ParamsView.AddParamsView(nodeModel.Name, serialProp));
            }
        }

        public override string ToString()
        {
            return "NodeController";
        }

        private IEnumerable<PortNode> GetPortControllers(EditorNode childNode)
        {
            if (childNode is PortNode portController)
            {
                yield return portController;
            }

            if (childNode is PortListNode portListController)
            {
                IEnumerable<PortNode> childPorts = portListController.ChildNodes.OfType<PortNode>();
                foreach (var childPort in childPorts)
                {
                    yield return childPort;
                }
            }
        }
    }
}