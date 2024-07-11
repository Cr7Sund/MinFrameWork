using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Cr7Sund.Editor
{
    public abstract class CustomSetting : ScriptableObject
    {
        // public T instance { get => CustomSettingSingletons<T>.Instance.; }
        protected virtual void NotifyValueChanged(SerializedPropertyChangeEvent evt)
        {
        }

        public static void SaveModified<T>() where T : CustomSetting
        {
            CustomSettingSingletons<T>.SaveModified();
        }
    }

    public class CustomSettingSingletons<T> : IResetSettings where T : CustomSetting
    {
        public const string menuItemBase = "CustomSettings/";
        public const string settingFolder = "Assets/CustomSettings/";
        public static string Path => settingFolder + typeof(T).Name + ".asset";
        public string pathPartialToCategory
        {
            get
            {
                string category = typeof(T).GetCustomAttribute<CustomSettingAttribute>()?.category;
                if (!category.EndsWith('/')) category += "/";
                return $"{menuItemBase}{category}{typeof(T).Name}";
            }
        }

        public SettingsScope scope => typeof(T)
                .GetCustomAttribute<CustomSettingAttribute>()?.scope ?? SettingsScope.User;
        public string[] tags => SerializedPropertyHelper.GetSerializeFieldInfos(Instance)
                .Select(fieldInfo => fieldInfo.Name).ToArray();
        Object IResetSettings.instance => Instance;


        private static T blueprintSettings = null;
        public static T BlueprintSettings
        {
            get
            {
                if (blueprintSettings == null)
                {
                    Debug.Log($"create settings blueprint !");
                    blueprintSettings = ScriptableObject.CreateInstance<T>();
                }
                return blueprintSettings;
            }
        }

        public static T Instance
        {
            get
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(Path);
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<T>();
                    AssetDatabaseUtility.CreateAssetAndDirectories(asset, Path);
                    AssetDatabase.SaveAssetIfDirty(asset);
                }
                return asset;
            }
        }


        public static void SaveModified()
        {
            var serializedObject = new SerializedObject(Instance);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssetIfDirty(Instance);
        }

        /// <summary>
        /// Publicly expose the save method so we can call this externally
        /// </summary>
        public void Save()
        {
            AssetDatabase.SaveAssetIfDirty(Instance);
        }


        /// <summary>
        /// Copy a specific property from the blueprint to a property (requires matching objects)
        /// </summary>
        /// <param name="dest"></param>
        public void CopyFromBlueprint(SerializedProperty dest)
        {
            SerializedProperty blueprintProperty = new SerializedObject(BlueprintSettings).FindProperty(dest.propertyPath);
            dest.serializedObject.CopyFromSerializedProperty(blueprintProperty);
            dest.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Copy all values from the blueprints to our settings.
        /// </summary>
        /// <param name="serializedObject"></param>
        public void CopyAllFromBlueprint(SerializedObject serializedObject)
        {
            EditorUtility.CopySerialized(BlueprintSettings, Instance);
            serializedObject.ApplyModifiedProperties();
        }

        public void NotifyValueChanged(SerializedPropertyChangeEvent evt)
        {
        }

    }

}