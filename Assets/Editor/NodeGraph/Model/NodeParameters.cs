using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Cr7Sund.Editor.NodeGraph
{
    public abstract class ValueParams
    {
        public abstract string Name { get; set; }
        public abstract string Category { get; set; }

        // public abstract Type ParamType { get; }

        public ValueParams(string category, string name)
        {
            this.Category = category;
            this.Name = name;
        }

        public override bool Equals(object obj)
        {
            if (obj is ValueParams otherParams)
            {
                return this.Name == otherParams.Name && this.Category == otherParams.Category;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Category);
        }
    }

    [Serializable]
    public class IntParams : ValueParams
    {
        public int Value;
        [SerializeField] private string name;
        [SerializeField] private string category;
        [SerializeField] private string disPlayName;
        public override string Name { get => name; set => name = value; }
        public override string Category { get => category; set => category = value; }

        public IntParams(int value, string name, string category, string disPlayName) : base(category, name)
        {
            this.Value = value;
            this.disPlayName = disPlayName;
        }
    }

    [Serializable]
    public class StringParams : ValueParams
    {
        public string Value;
        [SerializeField] private string name;
        [SerializeField] private string category;
        [SerializeField] private string disPlayName;
        public override string Name { get => name; set => name = value; }
        public override string Category { get => category; set => category = value; }

        public StringParams(string value, string name, string category, string disPlayName) : base(category, name)
        {
            this.Value = value;
            this.disPlayName = disPlayName;
        }
    }

    [Serializable]
    public class BoolParams : ValueParams
    {
        public bool Value;
        [SerializeField] private string name;
        [SerializeField] private string category;
        [SerializeField] private string disPlayName;
        public override string Name { get => name; set => name = value; }
        public override string Category { get => category; set => category = value; }

        public BoolParams(bool value, string name, string category, string disPlayName) : base(category, name)
        {
            this.Value = value;
            this.disPlayName = disPlayName;
        }
    }
    [Serializable]
    public class NodeParameters : BaseModel
    {
        // [SerializeReference]
        public List<IntParams> ints = new();
        // [SerializeReference]
        public List<StringParams> strings = new();
        public List<BoolParams> bools = new();
        [HideInInspector] public List<string> categories = new();
        private Dictionary<Type, object> paramsDict = new();


        public NodeParameters(BaseModel parentModel) : base(parentModel)
        {
            paramsDict.Add(typeof(int), ints);
            paramsDict.Add(typeof(string), strings);
            paramsDict.Add(typeof(bool), bools);
        }

        public override SerializedProperty OnBindSerializedProperty(IModel model, SerializedProperty parentSerializedProperty, int index)
        {
            SerializedProperty paramsProp = parentSerializedProperty.FindPropertyRelative("nodeParameter");
            BindSerializeProps(this, paramsProp);
            return paramsProp;
        }

        public List<ValueParams> GetParamsByCategory(string category)
        {
            var result = new List<ValueParams>();
            foreach (var kvp in this.ints)
            {
                if (kvp.Category == category)
                {
                    result.Add(kvp);
                }
            }
            foreach (var kvp in this.strings)
            {
                if (kvp.Category == category)
                {
                    result.Add(kvp);
                }
            }

            return result;
        }

        public List<SerializedProperty> GetSerialParamsByCategory(string category)
        {
            var result = new List<SerializedProperty>();
            for (int i = 0; i < ints.Count; i++)
            {
                var kvp = ints[i];
                var prop = serializedProperty.FindPropertyRelative(nameof(ints));
                if (kvp.Category == category)
                {
                    result.Add(prop.GetArrayElementAtIndex(i));
                }
            }
            for (int i = 0; i < strings.Count; i++)
            {
                var kvp = this.strings[i];
                var prop = serializedProperty.FindPropertyRelative(nameof(strings));
                if (kvp.Category == category)
                {
                    result.Add(prop.GetArrayElementAtIndex(i));
                }
            }
            for (int i = 0; i < bools.Count; i++)
            {
                var kvp = this.bools[i];
                var prop = serializedProperty.FindPropertyRelative(nameof(bools));
                if (kvp.Category == category)
                {
                    result.Add(prop.GetArrayElementAtIndex(i));
                }
            }
            return result;
        }

        public SerializedProperty GetSerialParamsByCategory(ValueParams valueParam)
        {
            SerializedProperty result = null;
            var listProp = serializedProperty.FindPropertyRelative(nameof(ints));

            for (int i = 0; i < ints.Count; i++)
            {
                var kvp = ints[i];
                if (kvp.Category == valueParam.Category
                        && kvp.Name == valueParam.Name)
                {
                    result = listProp.GetArrayElementAtIndex(i);
                }
            }

            listProp = serializedProperty.FindPropertyRelative(nameof(strings));
            for (int i = 0; i < strings.Count; i++)
            {
                var kvp = this.strings[i];
                if (kvp.Category == valueParam.Category
                        && kvp.Name == valueParam.Name)
                {
                    result = listProp.GetArrayElementAtIndex(i);
                }
            }

            listProp = serializedProperty.FindPropertyRelative(nameof(bools));
            for (int i = 0; i < bools.Count; i++)
            {
                var kvp = this.bools[i];
                if (kvp.Category == valueParam.Category
                        && kvp.Name == valueParam.Name)
                {
                    result = listProp.GetArrayElementAtIndex(i);
                }
            }
            return result;
        }

        public bool TryAddValue(object value, Type fieldType, string name, string category, string disPlayName, out ValueParams resultParams)
        {
            if (value != null)
            {
                Assert.IsInstanceOf(fieldType, value);
            }

            resultParams = null;
            bool containsValue = false;
            if (fieldType == typeof(int))
            {
                var matches = ints.Where(t => t.Name == name && category == t.Category)
                     .ToList();
                if (matches.Count > 0)
                {
                    foreach (var item in matches)
                    {
                        resultParams = item;
                        item.Value = (int)value;
                        // SerializedPropertyHelper.MapSerializedProperty(item, GetSerialParamsByCategory(t));
                    }
                    containsValue = true;
                }
                else
                {
                    IntParams item = new IntParams((int)value, name, category, disPlayName);
                    resultParams = item;
                    ints.Add(item);
                }
            }
            else if (fieldType == typeof(string))
            {
                string str = string.Empty;
                if (value != null)
                {
                    str = (string)value;
                }
                var matches = strings
                   .Where(t => t.Name == name && category == t.Category)
                   .ToList();
                if (matches.Count > 0)
                {
                    foreach (var item in matches)
                    {
                        resultParams = item;
                        item.Value = str;
                        // SerializedPropertyHelper.MapSerializedProperty(t, GetSerialParamsByCategory(t));
                    }
                    containsValue = true;
                }
                else
                {
                    StringParams item = new StringParams(str, name, category, disPlayName);
                    resultParams = item;
                    strings.Add(item);
                }
            }
            else if (fieldType == typeof(bool))
            {
                var matches = bools
                .Where(t => t.Name == name && category == t.Category)
                .ToList();
                if (matches.Count > 0)
                {
                    foreach (var item in matches)
                    {
                        resultParams = item;
                        item.Value = (bool)value;
                        // SerializedPropertyHelper.MapSerializedProperty(t, GetSerialParamsByCategory(t));
                    }
                    containsValue = true;
                }
                else
                {
                    BoolParams item = new BoolParams((bool)value, name, category, disPlayName);
                    resultParams = item;
                    bools.Add(item);
                }
            }

            if (!categories.Contains(category))
            {
                categories.Add(category);
            }

            return containsValue;
        }

        private string GetParamFiledName(Type fieldType)
        {
            if (fieldType == typeof(int))
            {
                return nameof(ints);
            }
            else if (fieldType == typeof(string))
            {
                return nameof(strings);
            }
            else if (fieldType == typeof(bool))
            {
                return nameof(bools);
            }
            return string.Empty;
        }

        public SerializedProperty AddSerialization(object value, ValueParams valueParams)
        {
            string arrayName = GetParamFiledName(value.GetType());
            var listProperty = serializedProperty.FindPropertyRelative(arrayName);
            listProperty.InsertArrayElementAtIndex(listProperty.arraySize);
            var elementProp = listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1);
            SerializedPropertyHelper.ReflectProp(valueParams, elementProp);

            return elementProp;
        }

        private static void BindSerializeProps(NodeParameters nodeParameters, SerializedProperty nodeParamSerialProp)
        {
            // reflection instead
            MapSerializeParams(nodeParameters.ints.Cast<ValueParams>().ToList(),
                    nodeParamSerialProp.FindPropertyRelative(nameof(ints)));
            MapSerializeParams(nodeParameters.strings.Cast<ValueParams>().ToList(),
                nodeParamSerialProp.FindPropertyRelative(nameof(strings)));
            MapSerializeParams(nodeParameters.bools.Cast<ValueParams>().ToList(),
                nodeParamSerialProp.FindPropertyRelative(nameof(bools)));
        }

        private static void MapSerializeParams(List<ValueParams> valueParams, SerializedProperty paramsProp)
        {
            if (valueParams.Count == paramsProp.arraySize)
            {
                return;
            }

            paramsProp.ClearArray();
            for (int i = 0; i < valueParams.Count; i++)
            {
                paramsProp.InsertArrayElementAtIndex(i);
                var portListElementProp = paramsProp.GetArrayElementAtIndex(i);
                SerializedPropertyHelper.ReflectProp(valueParams[i], portListElementProp);
            }
        }
    }
}