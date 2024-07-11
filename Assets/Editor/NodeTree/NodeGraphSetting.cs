using System;
using Cr7Sund.FrameWork.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor
{
    [CustomSetting(category = "hello")]
    public class NodeGraphSetting : CustomSetting
    {
        public static NodeGraphSetting Instance
        {
            get
            {
                return CustomSettingSingletons<NodeGraphSetting>.Instance;
            }
        }

        public string createUtilityNodeLabel = "Create Utility";
        public string createNodeLabel = "Create Node";
        public string searchWindowCommandHeader = "Commands";
        public string searchWindowRootHeader = "Create Nodes";
        [SerializeField]
        public string graphDocumentIdentifier = "a0414adb70e38d944a6aae0349026331";
        public VisualTreeAsset graphDocument => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(graphDocumentIdentifier));
        [SerializeField]
        private string graphStylesheetVariablesIdentifier = "115571a842b38fd4dadd42f40c05370e";
        public StyleSheet graphStylesheetVariables => AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(graphStylesheetVariablesIdentifier));
        [SerializeField]
        private string graphStylesheetIdentifier = "aa6283743ea0a28499901ad9325353a7";
        public StyleSheet graphStylesheet => AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(graphStylesheetIdentifier));
        public string LastOpenAssetPath
        {
            get => lastOpenAssetPath;
            set
            {
                Instance.lastOpenAssetPath = value;
                SaveModified<NodeGraphSetting>();
            }
        }
        public string lastOpenAssetPath;
        public StyleSheet customStylesheet;
        public PortTypePalette portTypePalette;


        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            return CustomSettingsProvider.Create<NodeGraphSetting>();
        }



        [SerializeField]
        private string handleBarsPartialIdentifier = "5e06b9b7f66cfc6409849fc4882a3ebf";
        [NonSerialized]
        private static VisualTreeAsset handleBarsPartial = null;
        public static VisualTreeAsset HandleBarsPartial
        {
            get
            {
                if (handleBarsPartial == null)
                {
                    handleBarsPartial = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(Instance.handleBarsPartialIdentifier));
                }
                return handleBarsPartial;
            }
        }

        public static VisualElement CreateHandleBars()
        {
            return HandleBarsPartial.Instantiate()[0];
        }
    }

    [System.Serializable]
    public class PortTypePalette
    {
        public string IntHex = "#289D74";
        public string ShortHex = "#0DB31F";
        public string LongHex = "#C771BB";
        public string FloatHex = "#000000";
        public string DoubleHex = "#00A4B3";
        public string CharHex = "#DAB950";
        public string StringHex = "#CC6927";
        public string ColorHex = "#AD9280";
        public string Vector2Hex = "#5E4738";
        public string Vector3Hex = "#452C1B";
        public string QuaternionHex = "#3B1903";
        public string CollectionHex = "#D4D3D2";


        public string GetHex(Type type)
        {
            if (type == typeof(int))
            {
                return IntHex;
            }
            if (type == typeof(short))
            {
                return ShortHex;
            }
            if (type == typeof(long))
            {
                return LongHex;
            }
            if (type == typeof(float))
            {
                return FloatHex;
            }
            if (type == typeof(double))
            {
                return DoubleHex;
            }
            if (type == typeof(char))
            {
                return CharHex;
            }
            if (type == typeof(string))
            {
                return StringHex;
            }
            if (type == typeof(Color))
            {
                return ColorHex;
            }
            if (type == typeof(Vector2))
            {
                return Vector2Hex;
            }
            if (type == typeof(Vector3))
            {
                return Vector3Hex;
            }
            if (type == typeof(Quaternion))
            {
                return QuaternionHex;
            }
            if (ReflectUtil.IsCollection(type))
            {
                return CollectionHex;
            }
            throw new NotImplementedException();
        }

        public Color GetColor(Type type)
        {
            var hex = GetHex(type);
            ColorUtility.TryParseHtmlString(hex, out var color);
            return color;
        }
    }

}