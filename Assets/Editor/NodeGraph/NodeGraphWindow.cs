using UnityEditor;
using Cr7Sund.NodeTree.Impl;
using INode = Cr7Sund.NodeTree.Api.INode;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Package.EventBus.Impl;
using UnityEditor.UIElements;
using System;
using NUnit.Framework.Internal;

namespace Cr7Sund.Editor.NodeGraph
{
    using NodeGraphSetting = CustomSettingSingletons<NodeGraphSetting>;

    public class NodeGraphWindow : EditorWindow
    {
        private IEditorGraphLogic _gameLogic;
        
        public static NodeGraphWindow OpenGraph<TGameLogic>(IAssetKey assetKey) where TGameLogic : IEditorGraphLogic
        {
            var graphWindow = GetWindow<NodeGraphWindow>();

            var gameLogic = Activator.CreateInstance<TGameLogic>();
            gameLogic.GraphKey = assetKey;
            NodeGraphSetting.Instance.LastOpenAssetGUID = AssetDatabase.AssetPathToGUID(assetKey.Key);
            NodeGraphSetting.Instance.LastOpenType = new SerializeType(typeof(TGameLogic));
            graphWindow.Show();
            graphWindow.RunLogic(gameLogic);

            return graphWindow;
        }

        public static void ClearGraph()
        {
            if (HasOpenInstances<NodeGraphWindow>())
            {
                var graphWindow = GetWindow<NodeGraphWindow>();
                graphWindow.OnDestroy();
            }
        }

        private void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }
        
        void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }
        
        private void OnAfterAssemblyReload()
        {
            if (NodeGraphSetting.Instance.LastOpenType.IsEmpty())
            {
                return;
            }
            string assetPath = AssetDatabase.GUIDToAssetPath(NodeGraphSetting.Instance.LastOpenAssetGUID);
            var asset = AssetDatabase.LoadAssetAtPath<ScriptableGraphModel>(assetPath);
            if (asset == null) return;
            SerializedObject serializedObject = new SerializedObject(asset);
            asset.Init(serializedObject);

            var gameLogic = Activator.CreateInstance(NodeGraphSetting.Instance.LastOpenType.GetSerialType()) as IEditorGraphLogic;
            gameLogic.GraphKey = new EditorKeys(asset.graphModelBase);

            RunLogic(gameLogic);
        }
        
        private void OnBeforeAssemblyReload()
        {
            _gameLogic?.Stop();
        }

        private void RunLogic(IEditorGraphLogic targetGameLogic)
        {
            if (this._gameLogic != null)
            {
                return;
            }

            _gameLogic = targetGameLogic;

            var graphRoot = GetWindow<NodeGraphWindow>().rootVisualElement;
            graphRoot.Unbind();
            graphRoot.Clear();
            graphRoot.ClearBindings();

            _gameLogic.Init(graphRoot);
            _gameLogic.Run();
        }

        private void OnDestroy()
        {
            _gameLogic?.Stop();
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

            InjectionBinder.Bind<IInternalLog>().To(logger).AsCrossContext();
            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsCrossContext().AsSingleton();
            InjectionBinder.Bind<IEventBus>().To<EventBus>().AsCrossContext().AsSingleton(); // same level
        }

        public override void RemoveComponents()
        {
            InjectionBinder.Unbind<IInternalLog>();
            InjectionBinder.Unbind<IPoolBinder>();
            InjectionBinder.Unbind<IEventBus>();
        }
    }
}