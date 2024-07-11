using System.Collections.Generic;
using UnityEditor.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeTree
{
    public class NodeView : BaseNode, IView
    {
        public List<EditableLabelElement> editableLabels = new List<EditableLabelElement>();
        public VisualElement inspectorContent;
        public NodeController nodeController;


        public NodeView(NodeController ctrl)
        {
            nodeController = ctrl;
        }
        

        public void StartView(IView parentView)
        {
            if (parentView is GraphView graphView)
            {
                graphView.AddElement(this);
            }
            inspectorContent = new VisualElement();

            var prop = nodeController.modelData.serializedProperty
                .FindPropertyRelative("name");
            CreateLabelUI(prop);
        }

        public void StopView(IView parentView)
        {
            if (parentView is GraphView graphView)
            {
                if (graphView.Contains(this))
                {
                    graphView.RemoveElement(this);
                }
            }
        }

        public void CreateLabelUI(SerializedProperty property)
        {
            // Add label to title Container
            PropertyField propertyField = CreatePropertyField(property);
            editableLabels.Add(new EditableLabelElement(propertyField));
            TitleContainer.Add(propertyField);

            // Add label to inspector
            if (inspectorContent != null)
            {
                PropertyField propertyFieldInspector = CreatePropertyField(property);
                editableLabels.Add(new EditableLabelElement(propertyFieldInspector));
                inspectorContent.Add(propertyFieldInspector);
            }
        }

        public override void SetPosition(Vector2 newPosition)
        {
            base.SetPosition(newPosition);
            nodeController.SetPosition(newPosition);
        }

        private PropertyField CreatePropertyField(SerializedProperty property)
        {
            PropertyField propertyField = new PropertyField(property.Copy())
            {
                name = property.name,
                bindingPath = property.propertyPath,
            };
            propertyField.BindProperty(property);

            return propertyField;
        }

    }
}