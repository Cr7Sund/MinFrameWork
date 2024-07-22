using System;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Package.Impl;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{
    using GraphSettings = CustomSettingSingletons<NodeGraphSetting>;

    public class GraphWindowController : EditorNode
    {
        private GraphModel graphModel;
        public GraphController GraphNode;
        private VisualElement _rootVisualElement;
        [Inject] private IEventBus eventBus;
        [Inject] private IPoolBinder poolBinder;

        public GraphWindowController(VisualElement rootVisualElement, IAssetKey assetKey = default) : base(null)
        {
            var editorKey = assetKey as EditorKeys;
            graphModel = editorKey.graphModel;
            this._rootVisualElement = rootVisualElement;
            LoadStyle(_rootVisualElement);
        }

        public override void Start()
        {
            if (isStart)
            {
                return;
            }
            isStart = true;

            StartGraph();
            StartView();

            eventBus.AddObserver<RebindUISignal>(OnRebindUI);
        }

        public override void Stop()
        {
            if (!isStart)
            {
                return;
            }
            isStart = false;

            var graphRoot = _rootVisualElement.Q<VisualElement>("graphViewRoot");

            graphRoot.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            _rootVisualElement.Unbind();

            UnloadChildAsync(GraphNode);
            GraphNode = null;
            GC.Collect();
        }

        public void AssignContext(NodeGraphContext nodeGraphContext)
        {
            _context = nodeGraphContext;
        }

        private void StartGraph()
        {
            GraphNode = new GraphController(graphModel, _rootVisualElement);
            AddChildAsync(GraphNode);
        }

        private void StartView()
        {
            var graphRoot = _rootVisualElement.Q<VisualElement>("graphViewRoot");

            if (GraphNode.view is VisualElement graphView)
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
                OpenMenuEvent eventData = poolBinder.AutoCreate<OpenMenuEvent>();
                eventData.evt = evt;
                eventData.commands = GraphNode.CreateCommands();
                eventBus.Dispatch(eventData);
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

        protected override IView CreateView()
        {
            throw new NotImplementedException();
        }
    }
}