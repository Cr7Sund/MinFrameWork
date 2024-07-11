using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.GraphView;
using UnityEditor;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeTree
{
    public class GraphController : EditorNode
    {
        public GraphView graphView => view as GraphView;
        public GraphModel graphModel => modelData as GraphModel;
        public VisualElement _rootVisualElement;
        private List<EdgeModel> appendingEdges = new();

        public GraphController(IModel model, VisualElement rootVisualElement) : base(model)
        {
            this._rootVisualElement = rootVisualElement;
        }

        public Dictionary<Actions, Tuple<Func<bool>, Action<object>>> CreateCommands()
        {
            Dictionary<Actions, Tuple<Func<bool>, Action<object>>> commands = new(); // tmp

            AddShortcutEntry(commands, Actions.Frame, SearchTreeEntry.AlwaysEnabled, OnFrameGraph);
            AddShortcutEntry(commands, Actions.Rename, () => graphView.GetSelectedNodeCount() == 1, OnRename);
            AddShortcutEntry(commands, Actions.Cut, DefaultEnabledCheck, OnCut);
            AddShortcutEntry(commands, Actions.Copy, DefaultEnabledCheck, OnCopy);
            AddShortcutEntry(commands, Actions.Paste, () => graphModel.HasCopyNodes(), OnPaste);
            AddShortcutEntry(commands, Actions.Duplicate, DefaultEnabledCheck, OnDuplicate);
            AddShortcutEntry(commands, Actions.Delete, () => graphView.HasSelectedEdges() || DefaultEnabledCheck(), OnDelete);

            return commands;
        }

        public override string ToString()
        {
            return "GraphController";
        }
        
        protected override EditorNode CreateChildNode(IModel model)
        {
            if (model is NodeModel nodeModel)
            {
                return new NodeController(nodeModel);
            }
            else if (model is EdgeModel edgeModel)
            {
                var outputEdge = GetPortByEdge(edgeModel.outputNode, edgeModel.outputId, Direction.Output).portView;
                var inputEdge = GetPortByEdge(edgeModel.inputNode, edgeModel.inputId, Direction.Input).portView;

                if (outputEdge == null)
                {
                    appendingEdges.Add(edgeModel);
                    return null;
                }
                else
                {
                    return new EdgeController(edgeModel, EdgeView.CreateEdge(inputEdge, outputEdge));
                }
            }
            else if (model is ContextInfo contextInfo)
            {
                return new ContextMenuController(contextInfo);
            }
            else if (model is InspectorInfo inspectorInfo)
            {
                return new BlackboardNode(inspectorInfo, _rootVisualElement);
            }

            throw new NotImplementedException();
        }

        protected override IView CreateView()
        {
            return new GraphView(OnGraphViewActionExecuted);
        }

        protected override void OnListen()
        {
            _eventBus.AddObserver<AddNodeEvent>(OnAddNode);
            _eventBus.AddObserver<AddPortListEvent>(OnAddPortList);
        }

        private void OnAddPortList(AddPortListEvent eventData)
        {
            EdgeModel edgeModel = null;
            for (int i = appendingEdges.Count - 1; i >= 0; i--)
            {
                EdgeModel appendEdge = appendingEdges[i];
                if (appendEdge.outputId == eventData.portInfo.id &&
                    appendEdge.outputNode.Equals(eventData.containerNode))
                {
                    edgeModel = appendEdge;
                    break;
                }
            }
            if (edgeModel == null)
            {
                return;
            }
            
            var outputEdge = eventData.edgeView;
            var inputEdge = GetPortByEdge(edgeModel.inputNode, edgeModel.inputId, Direction.Input).portView;
            var nodeCtrl = new EdgeController(edgeModel, EdgeView.CreateEdge(inputEdge, outputEdge));

            // nodeCtrl.modelData.serializedProperty = nodeCtrl.modelData.OnBindSerializedProperty(modelData.serializedProperty, index);
            AddChildAsync(nodeCtrl);
        }

        private void OnGraphViewActionExecuted(Actions actions, object data)
        {
            switch (actions)
            {
                case Actions.EdgeCreate:
                    OnEdgeCreate(data);
                    break;
                case Actions.EdgeDelete:
                    OnEdgeDelete(data);
                    break;
                case Actions.Delete:
                    OnDelete(data);
                    break;
                case Actions.SelectionChanged:
                    OnSelectionChange(data);
                    break;
                default:
                    break;
            }
        }

        private void OnSelectionChange(object data)
        {
            if (data is NodeView nodeView)
            {
                var e = _eventBus.CreateEvent<SelectNodeEvent>();
                e.nodeController = nodeView.nodeController;
                _eventBus.Dispatch(e);
            }
        }

        private void OnAddNode(AddNodeEvent eventData)
        {
            var nodeModel = NodeModel.CreateNodeModel(
                eventData.NodeType, eventData.createPos,
                eventData.isUtilityNode);

            AddNode(eventData, nodeModel);
        }

        private void OnRemoveNode(NodeModel removeNode)
        {
            graphModel.RemoveNode(removeNode);

            var resultNode = ChildNodes.OfType<NodeController>()
                                     .FirstOrDefault(node => node.nodeModel == removeNode);
            UnloadChildAsync(resultNode);
        }

        private void OnEdgeCreate(object data)
        {
            Edge edge = (Edge)data;
            var inputNodeView = edge.Input.ParentNode as NodeView;
            var outputNodeView = edge.Output.ParentNode as NodeView;
            var inputNode = inputNodeView.nodeController;
            var outputNode = outputNodeView.nodeController;
            var inputPort = inputNode.GetPortByView(edge.Input);
            var outputPort = outputNode.GetPortByView(edge.Output);

            var edgeData = new EdgeModel(
                inputNode.modelData as NodeModel, outputNode.modelData as NodeModel,
                inputPort.portInfo.id, outputPort.portInfo.id);
            AddEdge(edge, edgeData);
        }

        private void OnEdgeDelete(object data)
        {
            var edge = data as Edge;
            var inputNodeView = edge.Input.ParentNode as NodeView;
            var outputNodeView = edge.Output.ParentNode as NodeView;
            var inputNode = inputNodeView.nodeController;
            var outputNode = outputNodeView.nodeController;
            var inputPort = inputNode.GetPortByView(edge.Input);
            var outputPort = outputNode.GetPortByView(edge.Output);

            var targetEdge = ChildNodes
                .OfType<EdgeController>()
                .FirstOrDefault(edgeController => edgeController.edgeModel.inputNode.Equals(inputNode.modelData) &&
                                                  edgeController.edgeModel.outputNode.Equals(outputNode.modelData) &&
                                                  edgeController.edgeModel.inputId == inputPort.portInfo.id &&
                                                  edgeController.edgeModel.outputId == outputPort.portInfo.id);

            RemoveEdge(targetEdge);
        }

        private void AddNode(AddNodeEvent eventData, NodeModel nodeModel)
        {
            var nodeCtrl = new NodeController(nodeModel);
            graphModel.AddNode(nodeModel);
            nodeCtrl.modelData.serializedProperty =
                  nodeCtrl.modelData.OnBindSerializedProperty(modelData.serializedProperty,
                        eventData.isUtilityNode ? graphModel.utilityNodes.Count - 1 : graphModel.nodes.Count - 1);
            AddChildAsync(nodeCtrl);
        }

        private void AddEdge(Edge edge, EdgeModel edgeModel)
        {
            var edgeNode = new EdgeController(edgeModel, edge);
            graphModel.AddEdge(edgeModel);
            edgeNode.modelData.serializedProperty = edgeNode.modelData.OnBindSerializedProperty(
                modelData.serializedProperty,
                graphModel.edges.Count - 1);

            AddChildAsync(edgeNode);
        }

        private void RemoveEdge(EdgeController edgeController)
        {
            graphModel.RemoveEdge(edgeController.edgeModel);
            UnloadChildAsync(edgeController);
        }

        private PortController GetPortByEdge(NodeModel targetNode, int id, Direction direction)
        {
            return ChildNodes.OfType<NodeController>()
                    .Where(nodeController => nodeController.modelData.Equals(targetNode))
                    .Select(nodeController => nodeController.GetPortByIndex(direction, id))
                    .FirstOrDefault();
        }

        public NodeController GetNodeByModel(NodeModel targetNode)
        {
            return ChildNodes.OfType<NodeController>()
                            .Where(nodeController => nodeController.modelData.Equals(targetNode))
                            .FirstOrDefault();
        }

        private void AddShortcutEntry(Dictionary<Actions, Tuple<Func<bool>, Action<object>>> commands, Actions actionType, Func<bool> enabledCheck, Action<object> action)
        {
            commands.Add(actionType, new Tuple<Func<bool>, Action<object>>(enabledCheck, action));
        }

        private void OnDelete(object obj)
        {
            if (graphView.IsFocusedElementNullOrNotBindable)
            {
                var edgesToRemove = new List<Edge>();
                // go over every selected edge...
                graphView.ForEachSelectedEdgeDo((edge) =>
                {
                    edgesToRemove.Add(edge);
                });

                // go over every selected node and build a list of nodes that should be deleted....
                var nodesToRemove = new List<NodeModel>();
                graphView.ForEachSelectedNodeDo((node) =>
                {
                    NodeView scopedNodeView = node as NodeView;
                    if (scopedNodeView != null)
                    {
                        nodesToRemove.Add(scopedNodeView.nodeController.nodeModel);
                    }
                });

                // if we have nodes marked for deletion...
                if (nodesToRemove.Count > 0)
                {
                    for (int i = graphModel.edges.Count - 1; i >= 0; i--)
                    {
                        EdgeModel edge = graphModel.edges[i];
                        if (nodesToRemove.Contains(edge.inputNode) || nodesToRemove.Contains(edge.outputNode))
                        {
                            var targetEdge = ChildNodes.OfType<EdgeController>()
                                                     .FirstOrDefault(edgeController => edgeController.edgeModel == edge).edgeView.edge;

                            if (!edgesToRemove.Contains(targetEdge))
                            {
                                edgesToRemove.Add(targetEdge);
                            }
                        }
                    }
                }

                foreach (var edge in edgesToRemove)
                {
                    OnEdgeDelete(edge);
                }
                foreach (var node in nodesToRemove)
                {
                    OnRemoveNode(node);
                }
            }
        }

        private void OnDuplicate(object obj)
        {
            throw new NotImplementedException();
        }

        private void OnPaste(object obj)
        {
            throw new NotImplementedException();
        }

        private void OnCopy(object obj)
        {
            if (graphView.IsFocusedElementNullOrNotBindable)
            {
                List<NodeView> nodesToCapture = new List<NodeView>();

                graphView.ForEachSelectedNodeDo((node) =>
                {
                    NodeView scopedNodeView = node as NodeView;
                    if (scopedNodeView != null)
                    {
                        nodesToCapture.Add(scopedNodeView);
                    }
                });

                // copyPasteHandler.CaptureSelection(nodesToCapture, graphData);
            }
        }

        private void OnCut(object obj)
        {
            throw new NotImplementedException();
        }

        private void OnRename(object obj)
        {
            throw new NotImplementedException();
        }

        private void OnFrameGraph(object obj)
        {
            throw new NotImplementedException();
        }

        protected virtual bool DefaultEnabledCheck()
        {
            return graphView.GetSelectedNodeCount() > 0;
        }

    }
}
