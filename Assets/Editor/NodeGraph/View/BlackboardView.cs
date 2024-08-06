using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{
    public class BlackboardView : Blackboard, IView
    {
        public BlackboardView(VisualElement visualElement, Action<SerializedProperty, VisualElement, IModel, string> customDrawLogic = null) : base(visualElement, customDrawLogic)
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