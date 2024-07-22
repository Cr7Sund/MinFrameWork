using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{

    public class NodeParamsView : IView
    {
        private IModel _model;
        private VisualElement _container;

        public NodeParamsView(IModel model)
        {
            _model = model;
        }

        public void StartView(IView parentView)
        {
            var nodeView = parentView as NodeView;
            var nodeParameters = _model as NodeParameters;
            _container = DrawInspector(_model, nodeView.ExtensionContainer);
        }

        public static VisualElement DrawInspector(IModel model, VisualElement rootElement)
        {
            // Style
            rootElement.styleSheets.Add(NodeGraphSetting.PinnedElementStyle);

            var main = NodeGraphSetting.PinnedElement.CloneTree();
            main.AddToClassList("mainContainer");
            var root = main.Q("content");
            var header = main.Q("header");
            var titleLabel = main.Q<Label>(name: "titleLabel");
            var content = main.Q<VisualElement>(name: "contentContainer");

            root.ClearClassList();
            root.AddToClassList("pinnedElement");

            var nodeParameters = model as NodeParameters;

            foreach (var category in nodeParameters.categories)
            {
                var valueParams = nodeParameters.GetSerialParamsByCategory(category);

                foreach (var serialProperty in valueParams)
                {
                    CreateItemField(category, serialProperty, content);
                }
            }

            rootElement.Add(main);

            content.visible = true;
            return content;
        }

        private static void CreateItemField(string category, SerializedProperty serializedProperty, VisualElement container)
        {
            var categoryFold = container.Q<Foldout>(category);
            if (categoryFold == null)
            {
                categoryFold = new Foldout
                {
                    name = category,
                    text = category,
                };
                container.Add(categoryFold);
            }

            var rowItem = new VisualElement();
            rowItem.name = "rowItem";
            rowItem.style.flexDirection = FlexDirection.Row;
            var label = new Label();
            label.name = "paramsLabel";
            label.BindProperty(serializedProperty.FindPropertyRelative("disPlayName"));

            var valueProp = serializedProperty.FindPropertyRelative("Value");
            var propField = new PropertyField(valueProp);
            // propField.BindProperty(valueProp);
            propField.name = "paramsValue";
            propField.label = string.Empty;

            rowItem.Add(label);
            rowItem.Add(propField);

            categoryFold.Add(rowItem);
        }

        public void AddParamsView(string category, SerializedProperty serialProperty)
        {
            CreateItemField(category, serialProperty, _container);
        }

        public void StopView(IView parentView)
        {
            if (_container != null)
            {
                _container.visible = false;
            }
            // var nodeView = parentView as NodeView;
            // nodeView.ExtensionContainer.Remove(_container);
        }
    }
}
