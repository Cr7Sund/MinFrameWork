using System;
using System.Linq;
using System.Reflection;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Selector.Apis;
using Cr7Sund.Selector.Impl;
using Cr7Sund.Server;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Impl;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{
    public class NodeTreeProxyLogic : BaseGraphLogic
    {
        private GraphController _graphNode => _graphWindowNode.GraphNode;
        private GameNode _gameNode;
        private bool _initRunTimeLogic;

        private const string GUID = "1c01fc9f6d337fa41a9b544fa28320ff";

        public static void OpenGraph()
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(GUID);
            var graphModel = AssetDatabase.LoadAssetAtPath<ScriptableGraphModel>(assetPath);
            var graphKey = new EditorKeys(graphModel.graphModelBase, assetPath);
            graphModel.Init(new SerializedObject(graphModel));
            NodeGraphWindow.OpenGraph<NodeTreeProxyLogic>(graphKey);
        }

        public static void ClearGraph()
        {
            NodeGraphWindow.ClearGraph();
        }

        protected override void OnEditorUpdate()
        {
            InitGraph();
        }

        protected override void OnStop()
        {
            base.OnStop();
            Destroy();
        }

        private void InitGraph()
        {
            if (GameMgr.Instance.Status != Selector.Api.GameStatus.Started)
            {
                return;
            }
            if (_initRunTimeLogic)
            {
                return;
            }

            var gameLauncherField = typeof(GameMgr)
                .GetField("_launch", BindingFlags.NonPublic | BindingFlags.Instance);
            var gameLauncher = gameLauncherField.GetValue(GameMgr.Instance) as GameLauncher;
            var gameLogicField = typeof(GameLauncher)
                .GetField("_gameLogic", BindingFlags.NonPublic | BindingFlags.Instance);
            var gameLogic = gameLogicField.GetValue(gameLauncher) as IGameLogic;

            var gameNodeField = typeof(Cr7Sund.GameLogic.GameLogic).GetField("_gameNode", BindingFlags.NonPublic | BindingFlags.Instance);
            _gameNode = gameNodeField.GetValue(gameLogic) as GameNode;

            try
            {
                GenerateNodes(_gameNode);
                AddListeners();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            _initRunTimeLogic = true;
        }

        private void Destroy()
        {
            RemoveListeners();
            _initRunTimeLogic = false;
           _graphNode. graphModel.ClearAll();
           _gameNode = null;
        }

        private void AddListeners()
        {
            IContext context = _gameNode.Context;
            IEventBus eventBus = context.InjectionBinder.GetInstance<IEventBus>();
            eventBus.AddObserver<SwitchSceneEvent>(OnSwitchScene);
            eventBus.AddObserver<RemoveSceneEndEvent>(OnRemoveScene);
            eventBus.AddObserver<AddSceneBeginEvent>(OnAddScene);
            eventBus.AddObserver<AddUIBeginEvent>(OnAddUIBegin);
            eventBus.AddObserver<AddUIFailEvent>(OnAddUIFail);
            eventBus.AddObserver<RemoveUIEndEvent>(OnRemoveUI);
        }
        private void RemoveListeners()
        {
            if (_gameNode == null)
            {
                return;
            }
            IContext context = _gameNode.Context;
            if (context == null)
            {
                return;
            }

            IEventBus eventBus = context.InjectionBinder.GetInstance<IEventBus>();
            eventBus.RemoveObserver<SwitchSceneEvent>(OnSwitchScene);
            eventBus.RemoveObserver<RemoveSceneEndEvent>(OnRemoveScene);
            eventBus.RemoveObserver<AddSceneBeginEvent>(OnAddScene);
            eventBus.RemoveObserver<AddUIBeginEvent>(OnAddUIBegin);
            eventBus.RemoveObserver<AddUIFailEvent>(OnAddUIFail);
            eventBus.RemoveObserver<RemoveUIEndEvent>(OnRemoveUI);
        }

        private void GenerateNodes(GameNode gameNode)
        {
            // scene
            foreach (INode sceneNode in gameNode.ChildNodes)
            {
                var firstLevelNode = AddFirstLevelRoot(sceneNode.Key.ToString());
                AddNodeRecursively(sceneNode, firstLevelNode);
            }

            void AddNodeRecursively(INode node, NodeController parentNode)
            {
                for (int i = 0; i < node.ChildCount; i++)
                {
                    var newNode = AddUI(parentNode, node.GetChild(i).Key.ToString());
                    AddNodeRecursively(node.GetChild(i), newNode);
                }
            }
        }

        #region Event
        private void OnAddUIBegin(AddUIBeginEvent eventData)
        {
            string parentNodeName = eventData.ParentUI.ToString();
            string childNodeName = eventData.TargetUI.ToString();
            var parentNode = _graphNode.GetNodeByNodeName(parentNodeName);
            if (parentNode is NodeController parentNodeCtrl)
            {
                var childNode = AddUI(parentNodeCtrl, childNodeName);

                bool enable = true;
                AddOrUpdateGraphParams(childNode, eventData.GUID, eventData.GUID, nameof(eventData.GUID));
                AddOrUpdateGraphParams(childNode, enable, eventData.GUID, nameof(enable));
            }
        }

        public void AddOrUpdateGraphParams(NodeController nodeController, object value, string guid, string fieldName)
        {
            Assert.NotNull(value);

            string variableName = $"{guid}_{fieldName}";
            nodeController.TryAddNodeParams(value, value.GetType(), variableName, fieldName);
        }
        private void OnAddUIFail(AddUIFailEvent eventData)
        {
            string parentNodeName = eventData.ParentUI.ToString();
            string childNodeName = eventData.TargetUI.ToString();
            var parentNode = _graphNode.GetNodeByNodeName(parentNodeName);
            if (parentNode is NodeController parentNodeCtrl)
            {
                if (eventData.IsUnload)
                {
                    UnloadNode(parentNodeCtrl, eventData.TargetUI.ToString());
                }
                else
                {
                    var childNode = _graphNode.GetNodeByNodeName(childNodeName) as NodeController;
                    RemoveEdge(parentNodeCtrl, eventData.TargetUI.ToString());

                    bool enable = false;
                    AddOrUpdateGraphParams(childNode, enable, eventData.GUID, nameof(enable));
                }
            }
        }

        private void OnRemoveUI(RemoveUIEndEvent eventData)
        {
            if (eventData.IsUnload)
            {
                string parentNodeName = eventData.ParentUI.ToString();
                var parentNode = _graphNode.GetNodeByNodeName(parentNodeName);
                if (parentNode is NodeController parentNodeCtrl)
                {
                    UnloadNode(parentNodeCtrl, eventData.TargetUI.ToString());
                }
            }
            else
            {
                var parentNode = _graphNode.GetNodeByNodeName(eventData.ParentUI.ToString());
                if (parentNode is NodeController parentNodeCtrl)
                {
                    RemoveEdge(parentNodeCtrl, eventData.TargetUI.ToString());
                }
            }
        }

        private void OnAddScene(AddSceneBeginEvent eventData)
        {
            AddFirstLevelRoot(eventData.TargetScene.ToString());
        }

        private void OnRemoveScene(RemoveSceneEndEvent eventData)
        {
            if (eventData.IsUnload)
            {
                UnloadNode(null, eventData.TargetScene.ToString());
            }
            else
            {
                // RemoveUI(graphNode, eventData.TargetScene.ToString());
            }
        }

        private void OnSwitchScene(SwitchSceneEvent eventData)
        {
            var t = eventData.CurScene;
        }
        #endregion

        #region private methods

        private NodeController AddFirstLevelRoot(string nodeName)
        {
            Vector2 pos = new Vector2(0, _graphNode.ChildNodes.Count * 20);
            var type = typeof(Node);
            var nodeModel = NodeModel.CreateNodeModel(_graphNode.graphModel, nodeName, type, pos);
            return _graphNode.AddNode(nodeModel);
        }

        private void RemoveEdge(NodeController connectNode, string nodeName)
        {
            var edge = _graphNode.GetEdgeByModel(connectNode.modelData.Name, nodeName);
            if (edge != null)
                _graphNode.UnloadChildAsync(edge);
        }

        private void UnloadNode(NodeController connectNode, string nodeName)
        {
            if (connectNode != null)
            {
                RemoveEdge(connectNode, nodeName);
            }

            var node = _graphNode.GetNodeByNodeName(nodeName);
            _graphNode.UnloadChildAsync(node);

            if (connectNode != null)
            {
                UpdateChildItemPos(connectNode);
            }
        }

        private NodeController AddUI(NodeController connectNode, string nodeName)
        {
            Vector2 pos = Vector2.zero;
            var type = typeof(int);

            var childNodeModel = NodeModel.CreateNodeModel(_graphNode.graphModel, nodeName, type, pos);
            var childNode = _graphNode.AddNode(childNodeModel);

            // output node add port
            var outPortInfo = new PortInfo(childNodeModel)
            {
                portName = nodeName,
                serializeType = new SerializeType(type),
                id = connectNode.ChildNodes.Count,
                direction = Direction.Output,
            };
            var outPortCtrl = connectNode.AddPort(outPortInfo);
            // input node add port
            var inputPortInfo = new PortInfo(childNodeModel)
            {
                portName = connectNode.nodeModel.Name,
                serializeType = new SerializeType(type),
                id = 0,
                direction = Direction.Input,
            };
            var inputPortCtrl = childNode.AddPort(inputPortInfo);

            // create edge
            var edgeModel = new EdgeModel(childNodeModel, connectNode.nodeModel, 0, connectNode.ChildNodes.Count - 1, _graphNode.graphModel);
            var edgeView = EdgeView.CreateEdge(inputPortCtrl.portView, outPortCtrl.portView);
            var edgeCtrl = _graphNode.AddEdge(edgeView, edgeModel);

            childNode.nodeView.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
            return childNode;
        }

        private void OnGeometryChange(GeometryChangedEvent evt)
        {
            _graphNode.ChildNodes
                .OfType<NodeController>()
                .ToList()
                .ForEach(nodeController => UpdateChildItemPos(nodeController));
        }

        private void UpdateChildItemPos(NodeController connectNode, Orientation orientation = Orientation.Horizontal)
        {
            var parentPos = connectNode.nodeModel.GetPosition();
            float interval = 40;
            float width = connectNode.nodeView.layout.width;
            float height = connectNode.nodeView.layout.height;

            Vector2 totalChildSize = Vector2.zero;
            var connectedNodes = _graphNode.GetConnectNodes(connectNode.nodeModel);

            Vector2 space = Vector2.one * 10;

            foreach (var childNode in connectedNodes)
            {
                totalChildSize += childNode.nodeView.layout.size;
                totalChildSize += space;
            }

            var startPos = parentPos - totalChildSize / 2;
            foreach (var nodeController in connectedNodes)
            {
                var nodePos = startPos + nodeController.nodeView.layout.size / 2;
                startPos += nodeController.nodeView.layout.size;
                startPos += space;

                var newPos = SetPosByOrientation(parentPos, connectNode.nodeView.layout.size,
                    orientation, interval,
                    nodePos);
                nodeController.SetPosition(newPos);
            }
        }

        private static Vector2 SetPosByOrientation(Vector2 parentPos, Vector2 parentSize, Orientation orientation, float offset, Vector2 startPos)
        {
            if (orientation == Orientation.Horizontal)
            {
                parentPos.x += parentSize.x + offset;
                parentPos.y = startPos.y;
            }
            else
            {
                parentPos.y += parentSize.y + offset;
                parentPos.x = startPos.x;
            }

            return parentPos;
        }
        #endregion
    }
}
