using System;
using System.Collections.Generic;
using System.Reflection;
using Cr7Sund.Editor;
using UnityEditor;
using UnityEditor.GraphView;

namespace Cr7Sund.Editor.NodeTree
{

    public class ContextInfo : IModel
    {
        /// <summary>
        /// Node entries need to be pre-processed this helper class allows us to freely add all nodes first and process them later.
        /// </summary>
        public class NodeCreationEntry
        {
            /// <summary>
            /// Full menu path of the node.
            /// </summary>
            public string fullpath;
            /// <summary>
            /// Action to be executed when the menu item was clicked.
            /// </summary>
            public Action<object> action;
        }

        protected Dictionary<Type, string> nodeTypeToCreationLabel = new Dictionary<Type, string>();
        public Dictionary<Type, List<string>> ValueTypeToCreationLabel = new();
        public Dictionary<Type, List<Type>> ValueTypeToCreationNode = new();

        public SerializedProperty serializedProperty { get; set; }


        public SerializedProperty OnBindSerializedProperty(SerializedProperty parentSerializedProperty, int index)
        {
            return parentSerializedProperty.FindPropertyRelative("contextInfo");
        }

        public void IterateChildNodes(Action<IModel, int> action)
        {
        }

        public virtual string GetHeader()
        {
            return CustomSettingSingletons<NodeGraphSetting>.Instance.searchWindowRootHeader;
        }

        public void AddCreationLabel(Type nodeType, string label)
        {
            if (!nodeTypeToCreationLabel.ContainsKey(nodeType))
            {
                nodeTypeToCreationLabel.Add(nodeType, label);
            }
        }

        public void FilleCreateValueType(Type nodeType, string createNodeLabel)
        {
            var fieldInfos = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fieldInfo in fieldInfos)
            {
                var attributes = fieldInfo.GetCustomAttributes(true);

                foreach (var attr in attributes)
                {
                    if (attr.GetType() == typeof(InPortAttribute))
                    {
                        Type fieldType = fieldInfo.FieldType;
                        if (!ValueTypeToCreationLabel.TryGetValue(fieldType, out var labelCreationList))
                        {
                            labelCreationList = new();
                            ValueTypeToCreationLabel.Add(fieldType, labelCreationList);
                        }

                        string label = createNodeLabel.Substring(1);
                        if (!labelCreationList.Contains(label))
                        {
                            labelCreationList.Add(label);
                        }

                        if (!ValueTypeToCreationNode.TryGetValue(fieldType, out var nodeCreationList))
                        {
                            nodeCreationList = new();
                            ValueTypeToCreationNode.Add(fieldType, nodeCreationList);
                        }
                        if (!nodeCreationList.Contains(nodeType))
                        {
                            nodeCreationList.Add(nodeType);
                        }
                    }
                }
            }
        }

    }
}