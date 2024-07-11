using System;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Package.Impl;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeTree
{
    using GraphSettings = CustomSettingSingletons<NodeGraphSetting>;

    public class GraphWindowController : EditorNode
    {
        private GraphModel graphModel;
        public GraphController graphNode;
        private VisualElement _rootVisualElement;
        [Inject] private IEventBus eventBus;
        [Inject] private IPoolBinder poolBinder;
        public static Vector2 curClickPos;


        public GraphWindowController(VisualElement rootVisualElement, IAssetKey assetKey = default) : base(null)
        {
            var editorKey = assetKey as EditorKeys;
            graphModel = editorKey.graphModel;
            this._rootVisualElement = rootVisualElement;
            LoadStyle(_rootVisualElement);
        }

        public void StartGraph()
        {
            graphNode = new GraphController(graphModel, _rootVisualElement);
            AddChildAsync(graphNode);
        }

        public void StartView()
        {
            var graphRoot = _rootVisualElement.Q<VisualElement>("graphViewRoot");

            if (graphNode.view is VisualElement graphView)
            {
                graphView.StretchToParentSize();
                graphRoot.Add(graphView);
            }

            _rootVisualElement.Bind(graphModel.serializedObject);

            graphRoot.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public override void Start()
        {
            eventBus.AddObserver<RebindUISignal>(OnRebindUI);
        }

        private void OnRebindUI(RebindUISignal eventData)
        {
            _rootVisualElement.Bind(graphModel.serializedObject);
        }

        public override void Stop()
        {
            var graphRoot = _rootVisualElement.Q<VisualElement>("graphViewRoot");

            graphRoot.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            _rootVisualElement.Unbind();

            UnloadChildAsync(graphNode);
            GC.Collect();
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            curClickPos = evt.mousePosition;
            if (evt.button == 1)
            {
                OpenMenuEvent eventData = poolBinder.AutoCreate<OpenMenuEvent>();
                eventData.evt = evt;
                eventData.commands = graphNode.CreateCommands();
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

        public override string ToString()
        {
            return "GraphWindowNode";
        }

        protected override IView CreateView()
        {
            throw new NotImplementedException();
        }

        internal void AssignContext(NodeGraphContext nodeGraphContext)
        {
            _context = nodeGraphContext;
        }
    }
}