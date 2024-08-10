using System;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Package.Api;
using NUnit.Framework;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund.Editor.NodeGraph
{
    using GraphSettings = CustomSettingSingletons<NodeGraphSetting>;

    public class GraphWindowNode : EditorNode
    {
        private GraphModel graphModel;
        private GraphNode _graphNode;
        private VisualElement _rootVisualElement;
        private IVisualElementScheduledItem _updateTask;
        [Inject] private IPromiseTimer _promiseTimer;

        public GraphNode GraphNode
        {
            get
            {
                Assert.IsTrue(_isStart);
                return _graphNode;
            }
        }

        public GraphWindowNode(VisualElement rootVisualElement, IAssetKey assetKey = default) : base(null)
        {
            var editorKey = assetKey as EditorKeys;
            graphModel = editorKey.GraphModel;
            this._rootVisualElement = rootVisualElement;

            LoadStyle(_rootVisualElement);
        }

        public override void Start()
        {
            if (_isStart)
            {
                return;
            }
            _isStart = true;

            _context = CreateContext();
            Inject();
            StartGraph();
            StartView();

            _eventBus.AddObserver<RebindUISignal>(OnRebindUI);
            _updateTask = _rootVisualElement.schedule.Execute(OnEditorUpdate).Every(10);
        }

        private void OnEditorUpdate(TimerState state)
        {
            _promiseTimer.Update(state.deltaTime);
        }

        protected override INodeContext CreateContext()
        {
            return new NodeGraphContext();
        }

        public override void Stop()
        {
            if (!_isStart)
            {
                return;
            }
            _isStart = false;

            _updateTask.Pause();

            var graphRoot = _rootVisualElement.Q<VisualElement>("graphViewRoot");
            graphRoot.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            _rootVisualElement.Unbind();

            UnloadChildAsync(_graphNode);
            _graphNode = null;
            GC.Collect();
        }

        private void StartGraph()
        {
            // _graphNode = new GraphController(graphModel, _rootVisualElement);
            if (_manifest.TryGetValue(nameof(NodeGraph.GraphNode), out var output))
            {
                _graphNode = Activator.CreateInstance(output.NodeType, graphModel, _rootVisualElement) as GraphNode;
            }
            AddChildAsync(_graphNode);
        }

        private void StartView()
        {
            var graphRoot = _rootVisualElement.Q<VisualElement>("graphViewRoot");

            if (_graphNode.View is VisualElement graphView)
            {
                graphView.StretchToParentSize();
                graphRoot.Add(graphView);
            }

            _rootVisualElement.Bind(graphModel.serializedObject);

            graphRoot.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnRebindUI(RebindUISignal eventData)
        {
            _rootVisualElement.Bind(graphModel.serializedObject);
        }

        public override string ToString()
        {
            return "GraphWindowNode";
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button == 1)
            {
                OpenMenuEvent eventData = _poolBinder.AutoCreate<OpenMenuEvent>();
                eventData.evt = evt;
                eventData.commands = _graphNode.CreateCommands();
                _eventBus.Dispatch(eventData);
            }
        }

        private static void LoadStyle(VisualElement rootVisualElement)
        {
            VisualElement uxmlRoot = GraphSettings.Instance.graphDocument.CloneTree();

            rootVisualElement.Add(uxmlRoot);
            uxmlRoot.StretchToParentSize();

            rootVisualElement.styleSheets.Add(GraphSettings.Instance.graphStylesheetVariables);
            rootVisualElement.styleSheets.Add(GraphSettings.Instance.graphStylesheet);
            // add potential custom stylesheet
            var graphSettings = CustomSettingSingletons<NodeGraphSetting>.Instance;
            if (graphSettings.customStylesheet != null)
            {
                rootVisualElement.styleSheets.Add(graphSettings.customStylesheet);
            }
        }

    }
}