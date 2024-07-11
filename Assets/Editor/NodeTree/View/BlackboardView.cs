using System;
using UnityEditor;
using UnityEditor.GraphView;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeTree
{
    public class BlackboardView : Blackboard, IView
    {
        public BlackboardView(VisualElement visualElement, Action<SerializedProperty, VisualElement> customDrawLogic = null) : base(visualElement, customDrawLogic)
        {

        }

        public void StartView(IView parentView)
        {
        }

        public void StopView(IView parentView)
        {
        }
    }
}