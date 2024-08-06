using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

namespace Cr7Sund.Editor
{
    public class SerializedPropertyHelper
    {
        public static void ReflectProp(object instance, SerializedProperty rootProp)
        {
            var fieldInfos = GetSerializeFieldInfos(instance).ToDictionary(field => field.Name);

            ForEachProperty(rootProp, iterProp =>
            {
                if (fieldInfos.TryGetValue(iterProp.name, out var fieldInfo))
                    MapSerializedProperty(instance, iterProp, fieldInfo);
            });
        }

        public static List<FieldInfo> GetSerializeFieldInfos(object instance)
        {
            Assert.IsNotNull(instance);
            return instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                           .Where(field => (field.IsPublic && field.GetCustomAttribute<NonSerializedAttribute>() == null) ||
                                           (!field.IsPublic && field.GetCustomAttribute<SerializeField>() != null))
                           .ToList();
        }

        // Method to update a SerializedProperty based on the FieldInfo and instance
        private static void MapSerializedProperty(object instance, SerializedProperty serializedProperty, FieldInfo fieldInfo)
        {
            var fieldValue = fieldInfo.GetValue(instance);
            try
            {
                MapSerializedProperty(fieldValue, serializedProperty);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public static void MapSerializedProperty(object fieldValue, SerializedProperty serializedProperty)
        {
            Type fieldType = fieldValue.GetType();

            if (fieldType == typeof(string))
            {
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
            else if (serializedProperty.propertyType == SerializedPropertyType.ManagedReference)
                serializedProperty.managedReferenceValue = fieldValue;
            else if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
                serializedProperty.objectReferenceValue = fieldValue as Object;
            else if (serializedProperty.propertyType == SerializedPropertyType.Generic)
                return;
            else
                Debug.LogWarning($"Field '{fieldType}' is of an unsupported type '{serializedProperty.propertyType}'.");
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

        public static System.Type GetTargetType(SerializedProperty prop)
        {
            if (prop == null) return null;

            System.Reflection.FieldInfo field;
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Generic:
                    return TypeUtil.FindType(prop.type) ?? typeof(object);
                case SerializedPropertyType.Integer:
                    return prop.type == "long" ? typeof(int) : typeof(long);
                case SerializedPropertyType.Boolean:
                    return typeof(bool);
                case SerializedPropertyType.Float:
                    return prop.type == "double" ? typeof(double) : typeof(float);
                case SerializedPropertyType.String:
                    return typeof(string);
                case SerializedPropertyType.Color:
                    {
                        field = GetFieldOfProperty(prop);
                        return field != null ? field.FieldType : typeof(Color);
                    }
                case SerializedPropertyType.ObjectReference:
                    {
                        field = GetFieldOfProperty(prop);
                        return field != null ? field.FieldType : typeof(UnityEngine.Object);
                    }
                case SerializedPropertyType.LayerMask:
                    return typeof(LayerMask);
                case SerializedPropertyType.Enum:
                    {
                        field = GetFieldOfProperty(prop);
                        return field != null ? field.FieldType : typeof(System.Enum);
                    }
                case SerializedPropertyType.Vector2:
                    return typeof(Vector2);
                case SerializedPropertyType.Vector3:
                    return typeof(Vector3);
                case SerializedPropertyType.Vector4:
                    return typeof(Vector4);
                case SerializedPropertyType.Rect:
                    return typeof(Rect);
                case SerializedPropertyType.ArraySize:
                    return typeof(int);
                case SerializedPropertyType.Character:
                    return typeof(char);
                case SerializedPropertyType.AnimationCurve:
                    return typeof(AnimationCurve);
                case SerializedPropertyType.Bounds:
                    return typeof(Bounds);
                case SerializedPropertyType.Gradient:
                    return typeof(Gradient);
                case SerializedPropertyType.Quaternion:
                    return typeof(Quaternion);
                case SerializedPropertyType.ExposedReference:
                    {
                        field = GetFieldOfProperty(prop);
                        return field != null ? field.FieldType : typeof(UnityEngine.Object);
                    }
                case SerializedPropertyType.FixedBufferSize:
                    return typeof(int);
                case SerializedPropertyType.Vector2Int:
                    return typeof(Vector2Int);
                case SerializedPropertyType.Vector3Int:
                    return typeof(Vector3Int);
                case SerializedPropertyType.RectInt:
                    return typeof(RectInt);
                case SerializedPropertyType.BoundsInt:
                    return typeof(BoundsInt);
                default:
                    {
                        field = GetFieldOfProperty(prop);
                        return field != null ? field.FieldType : typeof(object);
                    }
            }
        }
        public static System.Type GetTargetType(SerializedObject obj)
        {
            if (obj == null) return null;

            if (obj.isEditingMultipleObjects)
            {
                var c = obj.targetObjects[0];
                return c.GetType();
            }
            else
            {
                return obj.targetObject.GetType();
            }
        }

        public static System.Reflection.FieldInfo GetFieldOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;

            var tp = GetTargetType(prop.serializedObject);
            if (tp == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');
            System.Reflection.FieldInfo field = null;
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));

                    //field = tp.GetMember(elementName, MemberTypes.Field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault() as System.Reflection.FieldInfo;
                    // field = DynamicUtil.GetMemberFromType(tp, element, true, MemberTypes.Field) as System.Reflection.FieldInfo;
                    if (field == null) return null;
                    tp = field.FieldType;
                }
                else
                {
                    //tp.GetMember(element, MemberTypes.Field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault() as System.Reflection.FieldInfo;
                    // field = DynamicUtil.GetMemberFromType(tp, element, true, MemberTypes.Field) as System.Reflection.FieldInfo;
                    if (field == null) return null;
                    tp = field.FieldType;
                }
            }
            return field;
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
