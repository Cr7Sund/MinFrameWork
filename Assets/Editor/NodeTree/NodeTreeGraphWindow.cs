using UnityEditor;
using Cr7Sund.Server.Impl;
using Cr7Sund.NodeTree.Impl;
using UnityEngine.UIElements;
using INode = Cr7Sund.NodeTree.Api.INode;
using System;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Package.EventBus.Impl;
using UnityEditor.UIElements;


namespace Cr7Sund.Editor.NodeTree
{
    using NodeGraphSetting = CustomSettingSingletons<NodeGraphSetting>;

    public class NodeTreeGraphWindow : EditorWindow
    {
        private NodeGraphGameLogic _gameLogic;


        public static NodeTreeGraphWindow OpenGraph(IAssetKey assetKey)
        {
            var graphWindow = GetWindow<NodeTreeGraphWindow>();

            var gameLogic = new NodeGraphGameLogic
            {
                RootVisualElement = graphWindow.rootVisualElement,
                GraphKey = assetKey
            };
            NodeGraphSetting.Instance.LastOpenAssetPath = assetKey.Key;

            graphWindow.Show();
            graphWindow.RunLogic(gameLogic);

            return graphWindow;
        }

        private void OnEnable()
        {
            var asset = AssetDatabase.LoadAssetAtPath<ScriptableGraphModel>(NodeGraphSetting.Instance.LastOpenAssetPath);
            if (asset == null) return;
            asset.Init(new SerializedObject(asset));

            var graphRoot = GetWindow<NodeTreeGraphWindow>().rootVisualElement;
            var gameLogic = new NodeGraphGameLogic
            {
                RootVisualElement = graphRoot,
                GraphKey = new EditorKeys(asset.graphModelBase)
            };

            RunLogic(gameLogic);
        }

        private void RunLogic(NodeGraphGameLogic targetGameLogic)
        {
            if (this._gameLogic != null)
            {
                return;
            }

            _gameLogic = targetGameLogic;

            var graphRoot = targetGameLogic.RootVisualElement;
            graphRoot.Unbind();
            graphRoot.Clear();
            graphRoot.ClearBindings();

            _gameLogic.Init();
            _gameLogic.Run();
        }

        private void OnDestroy()
        {
            _gameLogic?.DestroyAsync();
            _gameLogic = null;
        }
    }

    public class NodeGraphContext : CrossContext
    {
        private string Channel => "NodeGraph";

        public NodeGraphContext() : base()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }

        public override void AddComponents(INode self)
        {
            var logger = InternalLoggerFactory.Create(Channel);

            InjectionBinder.Bind<IInternalLog>().To(logger).ToName(ServerBindDefine.GameLogger);
            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsCrossContext().AsSingleton();
            InjectionBinder.Bind<IEventBus>().To<EventBus>().AsCrossContext().AsSingleton(); // same level
        }

        public override void RemoveComponents()
        {
            InjectionBinder.Unbind<IInternalLog>(ServerBindDefine.GameLogger);
            InjectionBinder.Unbind<IPoolBinder>();
            InjectionBinder.Unbind<IEventBus>();
        }
    }

    public class NodeGraphGameLogic
    {
        public VisualElement RootVisualElement;
        public IAssetKey GraphKey;
        protected GraphWindowController _graphWindowNode;

        public void Init()
        {
            Console.Init(InternalLoggerFactory.Create());
            _graphWindowNode = new GraphWindowController(RootVisualElement, GraphKey);
            _graphWindowNode.AssignContext(new NodeGraphContext());
        }

        public void Run()
        {
            try
            {
                _graphWindowNode.Inject();
                _graphWindowNode.StartGraph();
                _graphWindowNode.StartView();
                _graphWindowNode.Start();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
            }
        }

        public void DestroyAsync()
        {
            _graphWindowNode.Stop();
        }
    }
}