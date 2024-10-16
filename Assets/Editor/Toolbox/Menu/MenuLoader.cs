using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cr7Sund.Editor.MenuItems
{
    public static class MenuLoader
    {
        private const string MenuAssetGuid = "59a6ba2a3adb4b02a93d9110af901468";

        public static MenuBar Load()
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(MenuAssetGuid);
            TextAsset menuXml = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            Assert.IsNotNull(menuXml);

            // Deserialize XML to MenuBar object
            XmlSerializer serializer = new XmlSerializer(typeof(MenuBar));
            using (StringReader reader = new StringReader(menuXml.text))
            {
                MenuBar menuBar = (MenuBar)serializer.Deserialize(reader);

                if (menuBar != null)
                {
                    menuBar.CheckCollision();
                    return menuBar;
                }
                else
                {
                    Debug.LogError("Failed to parse the XML.");
                }
            }

            return null;
        }
    }

}
