using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.Drawer
{
    [CustomPropertyDrawer(typeof(HexDrawerAttribute))]
    public class HexDrawerInspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            var label = new Label(property.displayName);
            // var hexDrawerAttribute = attribute as HexDrawerAttribute;
            var colorField = new ColorField();
            if (ColorUtility.TryParseHtmlString(property.stringValue, out var color))
            {
                colorField.value = color;
            }
            colorField.RegisterValueChangedCallback(v =>
            {
                property.stringValue = "#" + ColorUtility.ToHtmlStringRGB(v.newValue);
            });

            container.style.flexDirection = FlexDirection.Row;
            container.Add(label);
            container.Add(colorField);
            return container;
        }
    }

}