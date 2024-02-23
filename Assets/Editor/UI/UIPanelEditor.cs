using Cr7Sund.UGUI.Impls;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.UGUI.Editor
{
    [CustomEditor(typeof(UIPanel))]
    public class UIPanelEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement myInspector = new VisualElement();
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI/UIPanelEditor.uxml");
            var uiPanel = target as UIPanel;

            visualTree.CloneTree(myInspector);


            if (!uiPanel.ComponentContainers.ContainsKey(nameof(CanvasGroup)))
            {
                uiPanel.ComponentContainers.Add(nameof(CanvasGroup), uiPanel.GetComponent<CanvasGroup>());
            }
            if (!uiPanel.ComponentContainers.ContainsKey(nameof(Canvas)))
            {
                uiPanel.ComponentContainers.Add(nameof(Canvas), uiPanel.GetComponent<Canvas>());
            }
            uiPanel.ComponentContainers[nameof(Canvas)] = uiPanel.GetComponent<Canvas>();
            uiPanel.ComponentContainers[nameof(CanvasGroup)] = uiPanel.GetComponent<CanvasGroup>();


            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssetIfDirty(target);
            return myInspector;
        }
    }
}