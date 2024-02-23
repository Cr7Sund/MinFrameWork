using Cr7Sund.Config;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor
{
    [CustomPropertyDrawer(typeof(AssetKey))]
    // [CustomPropertyDrawer(typeof(UIKey))]
    // [CustomPropertyDrawer(typeof(SceneKey))]
    public class AssetKeyDrawer : PropertyDrawer
    {
        private const string KeyDefine = "FullName";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            var objectField = new ObjectField();
            var tipBox = new HelpBox("the selected asset is not addressable", HelpBoxMessageType.Error);
            var assetKey = property.FindPropertyRelative(KeyDefine);
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetKey.stringValue);

            tipBox.visible = (false);

            objectField.value = asset;
            objectField.allowSceneObjects = false;
            objectField.RegisterValueChangedCallback(evt =>
                {
                    tipBox.visible = (false);

                    if (evt.newValue != null)
                    {
                        if (!AddressableHelper.IsAssetAddressable(evt.newValue))
                        {
                            tipBox.visible = (true);
                        }
                        else
                        {
                            assetKey.stringValue = AssetDatabase.GetAssetPath(evt.newValue);
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    }
                });


            container.Add(objectField);
            container.Add(tipBox);
            return container;
        }
    }


}