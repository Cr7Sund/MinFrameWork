using UnityEditor;
using UnityEditor.UIElements;
using System;

namespace Cr7Sund.Editor.NodeGraph
{
    using NodeGraphSetting = CustomSettingSingletons<NodeGraphSetting>;

    public class NodeGraphWindow : EditorWindow
    {
        private IEditorGraphLogic _gameLogic;

        public static NodeGraphWindow OpenGraph<TGameLogic, TManifest>(IAssetKey assetKey)
            where TManifest : Manifest
            where TGameLogic : IEditorGraphLogic
        {
            var graphWindow = GetWindow<NodeGraphWindow>();
            var gameLogic = Activator.CreateInstance<TGameLogic>();
            gameLogic.GraphKey = assetKey;

            NodeGraphSetting.Instance.LastGraphManifestType = new SerializeType(typeof(TManifest));
            NodeGraphSetting.Instance.LastOpenAssetGUID = AssetDatabase.AssetPathToGUID(assetKey.Key);
            NodeGraphSetting.Instance.LastOpenGraphAssetType = new SerializeType(typeof(TGameLogic));
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
            if (NodeGraphSetting.Instance.LastOpenGraphAssetType.IsEmpty())
            {
                return;
            }
            string assetPath = AssetDatabase.GUIDToAssetPath(NodeGraphSetting.Instance.LastOpenAssetGUID);
            var asset = AssetDatabase.LoadAssetAtPath<ScriptableGraphModel>(assetPath);
            if (asset == null) return;
            SerializedObject serializedObject = new SerializedObject(asset);
            asset.Init(serializedObject);

            var gameLogic = Activator.CreateInstance(NodeGraphSetting.Instance.LastOpenGraphAssetType.GetSerialType()) as IEditorGraphLogic;
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
}