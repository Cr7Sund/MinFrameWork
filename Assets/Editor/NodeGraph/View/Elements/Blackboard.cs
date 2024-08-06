using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{
    public class Blackboard
    {
        private readonly TabView _inspectorTabView;
        public readonly Action<SerializedProperty, VisualElement, IModel, string> _customDrawLogic;
        private Dictionary<string, VisualElement> _tableItems = new();

        public Blackboard(VisualElement rootVisualElement, Action<SerializedProperty, VisualElement, IModel, string> customDrawLogic = null)
        {
            _inspectorTabView = rootVisualElement.Q<TabView>("inspectorContainer");
            _customDrawLogic = customDrawLogic;
            rootVisualElement.Add(_inspectorTabView);
        }

        public void UpdateTab(string tabName, SerializedProperty serializeProp, IModel model)
        {
            if (!_tableItems.TryGetValue(tabName, out var tableItem))
            {
                tableItem = new Tab(tabName);
                _inspectorTabView.Add(tableItem);
                _tableItems.Add(tabName, tableItem);
            }

            if (_customDrawLogic == null)
            {
                var propField = tableItem.Q<PropertyField>("propItem");
                if (propField == null)
                {
                    propField = new PropertyField(serializeProp);
                    propField.name = "propItem";
                    tableItem.Add(propField);
                }

                propField.BindProperty(serializeProp);
            }
            else
            {
                _customDrawLogic.Invoke(serializeProp, tableItem, model, tabName);
            }
        }

        public void SelectTab(int index)
        {
            _inspectorTabView.selectedTabIndex = index;
        }

        private void OnTabChanged(Tab previousTab, Tab curTab)
        {

        }

    }
}
