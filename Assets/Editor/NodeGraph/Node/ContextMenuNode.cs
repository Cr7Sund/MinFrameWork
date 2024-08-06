using System;
using System.Collections.Generic;
using Cr7Sund.Package.Impl;
using UnityEditor;
using UnityEditor.GraphView;
using UnityEngine;

namespace Cr7Sund.Editor.NodeGraph
{
    using Settings = CustomSettingSingletons<NodeGraphSetting>;

    public class ContextMenuNode : EditorNode
    {
        private ContextInfo contextInfo => modelData as ContextInfo;
        private ContextMenu contextMenu => View as ContextMenu;

        public ContextMenuNode(IModel model) : base(model)
        {
        }

        protected override IView CreateView()
        {
            return ScriptableObject.CreateInstance<ContextMenu>();
        }
        
        protected override void OnListen()
        {
            _eventBus.AddObserver<OpenMenuEvent>(OnOpenMenu);
        }

        private void OnOpenMenu(OpenMenuEvent eventData)
        {
            BuildContextMenu(eventData.commands);
            OpenMenu();
        }

        private void OpenMenu()
        {
            GraphView graphView = contextMenu.graphView;
            Vector2 screenMousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

            graphView.schedule.Execute(() =>
                        {
                            if (graphView.IsFocusedElementNullOrNotBindable)
                            {
                                var context = new SearchWindowContext(screenMousePosition, graphView);
                                SearchWindow.Open(context, contextMenu);
                            }
                        });
        }

        private void BuildContextMenu(Dictionary<Actions, Tuple<Func<bool>, Action<object>>> commands)
        {
            contextMenu.StartAddingMenuEntries(contextInfo.GetHeader());
            AddNodeEntries();
            contextMenu.ResolveNodeEntries(DefaultNodeEnabledCheck);
            AddCommands(commands);
        }

        private bool DefaultNodeEnabledCheck()
        {
            return true;
        }

        private void AddNodeEntries()
        {
            TypeCache.TypeCollection nodeTypes = TypeCache.GetTypesWithAttribute<NodeAttribute>();
            foreach (Type nodeType in nodeTypes)
            {
                NodeAttribute nodeAttribute = NodeModel.GetNodeAttribute(nodeType);
                // check if we have a utility node...
                bool isUtilityNode = nodeAttribute.isUtilityNode;

                // retrieve subcategories
                string categoryPath = nodeAttribute.categories;
                string endSlash = "/";
                categoryPath.Replace(@"\", "/");
                if (string.IsNullOrWhiteSpace(categoryPath))
                {
                    categoryPath = endSlash;
                }
                else if (!categoryPath.EndsWith(endSlash))
                {
                    categoryPath += endSlash;
                }
                if (!categoryPath.StartsWith(endSlash))
                {
                    categoryPath = endSlash + categoryPath;
                }

                // add to the list of createable nodes
                string createNodeLabel = $"{categoryPath}{nodeAttribute.GetName(nodeType)}";
                contextInfo.AddCreationLabel(nodeType, createNodeLabel.Substring(1));
                contextInfo.FilleCreateValueType(nodeType, createNodeLabel);
                createNodeLabel = (!isUtilityNode ? Settings.Instance.createNodeLabel : Settings.Instance.createUtilityNodeLabel) + createNodeLabel;

                GraphView graphView = contextMenu.graphView;
                Vector2 clickViewPos = graphView.GetMouseViewPosition();

                contextMenu.AddNodeEntry(createNodeLabel, t =>
                {
                    AddNodeEvent @event = _poolBinder.AutoCreate<AddNodeEvent>();
                    @event.isUtilityNode = isUtilityNode;
                    @event.NodeType = nodeType;
                    @event.createPos = clickViewPos;
                    _eventBus.Dispatch<AddNodeEvent>(@event);
                });
            }
        }

        private void AddCommands(Dictionary<Actions, Tuple<Func<bool>, Action<object>>> commands)
        {
            contextMenu.AddSeparator(Settings.Instance.searchWindowCommandHeader);

            foreach (var item in commands)
            {
                contextMenu.AddShortcutEntry(item.Key, item.Value.Item1, item.Value.Item2);
            }
        }

    }
}