using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
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
        public string LastOpenAssetGUID
        {
            get => lastOpenAssetGuid;
            set
            {
                Instance.lastOpenAssetGuid = value;
                SaveModified<NodeGraphSetting>();
            }
        }
        [FormerlySerializedAs("lastOpenAssetPath")]
        [SerializeField]
        private string lastOpenAssetGuid = "cebf7fb0fe7f0cb4b94962a64da13516";
        public SerializeType LastGraphManifestType
        {
            get => lastGraphManifestType;
            set
            {
                Instance.lastGraphManifestType = value;
                SaveModified<NodeGraphSetting>();
            }
        }
  
        [SerializeField]
        private SerializeType lastGraphManifestType;
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

        [SerializeField]
        private string pinnedElementIdentifier = "d3f889c0a1158794083d393b941fcbfd";
        [NonSerialized]
        private static VisualTreeAsset pinnedElement = null;
        public static VisualTreeAsset PinnedElement
        {
            get
            {
                if (pinnedElement == null)
                {
                    pinnedElement = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(Instance.pinnedElementIdentifier));
                }
                return pinnedElement;
            }
        }
        
        [SerializeField]
        private string pinnedElementStyleIdentifier = "20d603bb19ea81f45b925ac726a9abee";
        public static StyleSheet pinnedElementStyle;
        public static StyleSheet PinnedElementStyle
        {
            get
            {
                if (pinnedElementStyle == null)
                {
                    pinnedElementStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(Instance.pinnedElementStyleIdentifier));
                }
                return pinnedElementStyle;
            }
        }
        public static VisualElement CreateHandleBars()
        {
            return HandleBarsPartial.Instantiate()[0];
        }

        [Button]
        private void ChangeToPalette(int index)
        {
            portTypePalette.CopyFromPalette(index == 0 ? portTypePalette.PaletteA : portTypePalette.PaletteB);
        }
    }

}