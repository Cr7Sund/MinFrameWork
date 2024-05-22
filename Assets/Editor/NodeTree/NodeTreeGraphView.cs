using GraphProcessor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Cr7Sund.NodeTree.Editor
{
    public class NodeTreeGraphView : BaseGraphView
    {
        public NodeTreeGraphView(EditorWindow window) : base(window)
        {
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            BuildStackNodeContextualMenu(evt);
            base.BuildContextualMenu(evt);
        }

        /// <summary>
        /// Add the New Stack entry to the context menu
        /// </summary>
        /// <param name="evt"></param>
        protected void BuildStackNodeContextualMenu(ContextualMenuPopulateEvent evt)
        {
            Vector2 position = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            evt.menu.AppendAction("New Stack", (e) => AddStackNode(new BaseStackNode(position)), DropdownMenuAction.AlwaysEnabled);
        }
    }
}