using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor
{

    public class CustomSettingsProvider : SettingsProviderBase
    {
        private PreferenceSettings graphSettings;
        private IResetSettings resetSettings;


        public CustomSettingsProvider(IResetSettings settings, PreferenceSettings preferenceSettings = null) : base(settings.pathPartialToCategory, settings.scope)
        {
            resetSettings = settings;
            graphSettings = preferenceSettings ?? new PreferenceSettings(settings.GetType().Name);
            keywords = settings.tags;
        }


        public static SettingsProvider Create<T>() where T : CustomSetting
        {
            try
            {
                var instance = new CustomSettingSingletons<T>();
                return new CustomSettingsProvider(instance);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Create providerException: {ex}");
                throw;
            }
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            // create a button to reset all properties back to the bleuprint value
            Button resetAll = new Button(() =>
            {
                resetSettings.CopyAllFromBlueprint(serializedObject);
            });
            resetAll.text = ConstSettings.resetAllLabel;
            resetAll.tooltip = ConstSettings.resetAllTooltip;
            resetAll.AddToClassList(nameof(resetAll));
            rootElement[0].Insert(1, resetAll);

            Label userSpecific = new Label("User Specific Settings");
            userSpecific.style.fontSize = 20;
            userSpecific.style.unityFontStyleAndWeight = FontStyle.Bold;
            userSpecific.style.marginBottom = 10;
            userSpecific.AddToClassList("title");
            rootElement[0].Add(userSpecific);

            Toggle userEnabledFoldoutExpansion = new Toggle("Expand all Foldouts");
            userEnabledFoldoutExpansion.value = graphSettings.ExpandAllSideFoldouts;
            userEnabledFoldoutExpansion.RegisterValueChangedCallback(value => graphSettings.ExpandAllSideFoldouts = value.newValue);
            rootElement[0].Add(userEnabledFoldoutExpansion);

            // add a custom stylesheet
            rootElement.styleSheets.Add(graphSettings.SettingsStylesheet);
        }

        protected override EventCallback<SerializedPropertyChangeEvent> GetValueChangedCallback()
        {
            return ValueChanged;
        }

        protected override Action<SerializedProperty, VisualElement> GetCreateAdditionalUIAction()
        {
            return CreateAdditionalUI;
        }

        /// <summary>
        /// Executed after every PropertyField.
        /// We'll attach a reset button here.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="container"></param>
        private void CreateAdditionalUI(SerializedProperty property, VisualElement container)
        {
            // create a resetButton per Row
            Button resetButton = new Button(() =>
            {
                // if reset is executed, copy the property from the blueprint to the property
                resetSettings.CopyFromBlueprint(property);
            });
            resetButton.AddToClassList(nameof(resetButton));
            resetButton.Add(graphSettings.ResetButtonIcon);
            resetButton.tooltip = ConstSettings.resetButtonTooltip;
            container.Add(resetButton);
        }

        /// <summary>
        /// Called when any value changed.
        /// </summary>
        /// <param name="evt"></param>
        private void ValueChanged(SerializedPropertyChangeEvent evt)
        { 
            // notify all listeners (ReactiveSettings)
            resetSettings.NotifyValueChanged(evt);
            serializedObject.ApplyModifiedProperties();
            // call save on our singleton as it is a strange hybrid and not a full ScriptableObject
            resetSettings.Save();
        }

        protected override string GetHeader()
        {
            return nameof(PreferenceSettings);
        }

        public override UnityEngine.Object GetInstance()
        {
            return resetSettings.instance;
        }


        private class ConstSettings
        {
            public static string resetButtonTooltip = "Reset this value back to the default value.";
            public static string resetAllLabel = "Reset All To Default";
            public static string resetAllTooltip = "Reset All values to default values.";

        }

    }

    public class PreferenceSettings
    {
        private const string defaultSettingsStyleSheetId = "7fbe7d63e648ddd4a8f0344909d59655";
        private const string defaultResetButtonIcon = "Refresh@2x";

        public string settingName;
        public string settingsStylesheetId;
        private string resetButtonIcon = "Refresh@2x";

        public PreferenceSettings(string settingName, string settingsStylesheetId = null, string resetButtonIcon = null)
        {
            this.settingName = settingName;
            this.settingsStylesheetId = settingsStylesheetId;
            this.resetButtonIcon = resetButtonIcon;
        }

        public Image ResetButtonIcon => new Image() { image = EditorGUIUtility.IconContent(resetButtonIcon ?? defaultResetButtonIcon).image };
        public StyleSheet SettingsStylesheet => AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(settingsStylesheetId ?? defaultSettingsStyleSheetId));
        public string expandAllSideFoldoutsKey => settingName + "." + nameof(expandAllSideFoldoutsKey);
        public bool ExpandAllSideFoldouts
        {
            get
            {
                return EditorPrefs.GetBool(expandAllSideFoldoutsKey, false);
            }
            set
            {
                EditorPrefs.SetBool(expandAllSideFoldoutsKey, value);
            }
        }
    }

}