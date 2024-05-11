using System;
using UnityEditor;
using UnityEngine;


namespace Cr7Sund.Server.Editor
{
    [CustomPropertyDrawer(typeof(EnabledIfAttribute))]
    public class EnabledIfAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as EnabledIfAttribute;
            var isEnabled = GetIsEnabled(attr, property);

            if (attr.hideMode == EnabledIfAttribute.HideMode.Disabled)
            {
                GUI.enabled &= isEnabled;
            }

            if (GetIsVisible(attr, property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

            if (attr.hideMode == EnabledIfAttribute.HideMode.Disabled)
            {
                GUI.enabled = true;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = attribute as EnabledIfAttribute;
            return GetIsVisible(attr, property) ? EditorGUI.GetPropertyHeight(property) : -2;
        }

        private bool GetIsVisible(EnabledIfAttribute attribute, SerializedProperty property)
        {
            if (GetIsEnabled(attribute, property))
            {
                return true;
            }

            if (attribute.hideMode != EnabledIfAttribute.HideMode.Invisible)
            {
                return true;
            }

            return false;
        }

        private bool GetIsEnabled(EnabledIfAttribute attribute, SerializedProperty property)
        {
            return attribute.enableIfValueIs == GetSwitcherPropertyValue(attribute, property);
        }

        private int GetSwitcherPropertyValue(EnabledIfAttribute attribute, SerializedProperty property)
        {
            var propertyNameIndex = property.propertyPath.LastIndexOf(property.name, StringComparison.Ordinal);
            var switcherPropertyName =
                property.propertyPath.Substring(0, propertyNameIndex) + attribute.switcherFieldName;
            var switcherProperty = property.serializedObject.FindProperty(switcherPropertyName);
            switch (switcherProperty.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return switcherProperty.boolValue ? 1 : 0;
                case SerializedPropertyType.Enum:
                    return switcherProperty.intValue;
                default:
                    throw new Exception("unsupported type.");
            }
        }
    }
}