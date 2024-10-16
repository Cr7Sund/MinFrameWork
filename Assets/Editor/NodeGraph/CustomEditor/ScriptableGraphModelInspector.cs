using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{
    [CustomEditor(typeof(ScriptableGraphModel))]
    public class ScriptableGraphModelInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var graphModel = target as ScriptableGraphModel;
            graphModel.Init(serializedObject);

            var graphKey = new EditorKeys(graphModel.graphModelBase,
                AssetDatabase.GetAssetPath(target));
            var root = new VisualElement();
            Button openBtn = new Button
            {
                text = "Open"
            };
            openBtn.RegisterCallback<ClickEvent>(_ => NodeGraphWindow.OpenGraph<Manifest>(graphKey));

            SerializedProperty property = serializedObject.FindProperty(nameof(graphModel.graphModelBase));
            var objectField = new PropertyField(property);
            root.Add(objectField);
            root.Add(openBtn);
            return root;
        }
    }

}
