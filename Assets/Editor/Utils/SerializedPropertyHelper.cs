using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Cr7Sund.Editor
{
    public class SerializedPropertyHelper
    {
        public static List<FieldInfo> GetSerializeFieldInfos(object instance)
        {
            Assert.IsNotNull(instance);
            return instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                           .Where(field => (field.IsPublic && field.GetCustomAttribute<NonSerializedAttribute>() == null) ||
                                           (!field.IsPublic && field.GetCustomAttribute<SerializeField>() != null))
                           .ToList();
        }

        public static void ReflectProp(object instance, SerializedProperty rootProp)
        {
            var fieldInfos = GetSerializeFieldInfos(instance).ToDictionary(field => field.Name);

            ForEachProperty(rootProp, iterProp =>
            {
                if (fieldInfos.TryGetValue(iterProp.name, out var fieldInfo))
                    MapSerializedProperty(instance, iterProp, fieldInfo);
            });
        }

        // Method to update a SerializedProperty based on the FieldInfo and instance
        public static void MapSerializedProperty(object instance, SerializedProperty serializedProperty, FieldInfo fieldInfo)
        {
            var fieldType = fieldInfo.FieldType;
            var fieldValue = fieldInfo.GetValue(instance);

            if (fieldType == typeof(string))
            {
                if (fieldValue is not string)
                {
                    Debug.Log(fieldInfo.FieldType + " " + fieldValue);
                }
                serializedProperty.stringValue = (string)fieldValue;
            }
            else if (fieldType == typeof(int))
                serializedProperty.intValue = (int)fieldValue;
            else if (fieldType == typeof(float))
                serializedProperty.floatValue = (float)fieldValue;
            else if (fieldType == typeof(bool))
                serializedProperty.boolValue = (bool)fieldValue;
            else if (fieldType == typeof(Color))
                serializedProperty.colorValue = (Color)fieldValue;
            else if (fieldType == typeof(Vector2))
                serializedProperty.vector2Value = (Vector2)fieldValue;
            else if (fieldType == typeof(Vector3))
                serializedProperty.vector3Value = (Vector3)fieldValue;
            else if (fieldType == typeof(Vector4))
                serializedProperty.vector4Value = (Vector4)fieldValue;
            else if (typeof(Enum).IsAssignableFrom(fieldType))
                serializedProperty.enumValueIndex = (int)fieldValue;
            else
                Debug.LogWarning($"Field '{fieldInfo.Name}' is of an unsupported type '{fieldType}'.");
        }

        /// <summary>
        /// Create a PropertyField with a callback.
        /// </summary>
        /// <param name="property">SerializedProperty that should be used to create the PropertyField.</param>
        /// <param name="changeCallback">Callback that gets triggered if a value changed.</param>
        /// <returns></returns>
        public static PropertyField CreatePropertyFieldWithCallback(SerializedProperty property, EventCallback<SerializedPropertyChangeEvent> changeCallback = null)
        {
            PropertyField propertyField = new PropertyField(property);
            if (changeCallback != null)
            {
                propertyField.RegisterCallback(changeCallback);
            }
            return propertyField;
        }

        /// <summary>
        /// This is an alternative to InspectorElement.FillDefaultInspector that works without having to provide an editor object.
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <param name="root">The UI root object.</param>
        /// <param name="changeCallback">Optional: Callback fired when a property changed.</param>
        /// <param name="CreateAdditionalUI">Optional: A method executed to create additional UI for every row.</param>
        /// <param name="rootPropertyPath">Optional: A relative property path that can be provided to start with a different property inside the serializedObject.</param>
        public static void CreateGenericUI(SerializedProperty rootProperty, VisualElement root, EventCallback<SerializedPropertyChangeEvent> changeCallback = null, System.Action<SerializedProperty, VisualElement> CreateAdditionalUI = null, string rootPropertyPath = null)
        {
            Action<SerializedProperty, ScrollView> creationLogic;
            if (CreateAdditionalUI != null)
            {
                creationLogic = (prop, scrollView) =>
                {
                    VisualElement container = new VisualElement();
                    container.AddToClassList(nameof(container) + nameof(PropertyField));
                    container.Add(CreatePropertyFieldWithCallback(prop, changeCallback));
                    CreateAdditionalUI(prop, container);
                    scrollView.Add(container);
                };
            }
            else
            {
                creationLogic = (prop, scrollView) =>
                {
                    scrollView.Add(CreatePropertyFieldWithCallback(prop, changeCallback));
                };
            }


            ScrollView scrollView = new ScrollView();
            scrollView.AddToClassList("propertyList" + nameof(ScrollView));
            root.Add(scrollView);
            ForEachProperty(rootProperty, iterProp => creationLogic(iterProp.Copy(), scrollView));
        }

        private static void ForEachProperty(SerializedProperty rootProperty, Action<SerializedProperty> iterateAction)
        {
            SerializedProperty iterator = rootProperty.Copy();
            SerializedProperty endProperty = rootProperty.GetEndProperty();

            while (iterator.NextVisible(true) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                iterateAction?.Invoke(iterator);
            }
        }
    }

}
