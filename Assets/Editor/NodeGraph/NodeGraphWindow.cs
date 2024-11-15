using UnityEditor;
using UnityEditor.UIElements;
using System;

namespace Cr7Sund.Editor.NodeGraph
{
    using NodeGraphSetting = CustomSettingSingletons<NodeGraphSetting>;

    public class NodeGraphWindow : EditorWindow
    {
        private IEditorGraphLogic _gameLogic;

        public static NodeGraphWindow OpenGraph< TManifest>(IAssetKey assetKey)
            where TManifest : Manifest
        {
            var graphWindow = GetWindow<NodeGraphWindow>();
            var gameLogic = Activator.CreateInstance<NodeTreeGraphLogic>();
            gameLogic.GraphKey = assetKey;

            NodeGraphSetting.Instance.LastGraphManifestType = new SerializeType(typeof(TManifest));
            NodeGraphSetting.Instance.LastOpenAssetGUID = AssetDatabase.AssetPathToGUID(assetKey.Key);
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
            string assetPath = AssetDatabase.GUIDToAssetPath(NodeGraphSetting.Instance.LastOpenAssetGUID);
            var asset = AssetDatabase.LoadAssetAtPath<ScriptableGraphModel>(assetPath);
            if (asset == null) return;
            SerializedObject serializedObject = new SerializedObject(asset);
            asset.Init(serializedObject);

            var gameLogic = Activator.CreateInstance<NodeTreeGraphLogic>() as IEditorGraphLogic;
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