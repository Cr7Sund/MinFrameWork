using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Assertions;
using Assembly = UnityEditor.Compilation.Assembly;
namespace Cr7Sund.Editor.MenuItems
{
    [XmlRoot("MenuBar")]
    public class MenuBar
    {
        [XmlElement("Menu")]
        public List<Menu> Menus { get; set; }
        public MenuItem FindMenuItem(string title, string menuName)
        {
            foreach (var menu in Menus)
            {
                if (title != menu.Title)
                {
                    continue;
                }

                if (getMenuItem(menu, out var findMenuItem))
                    return findMenuItem;
            }
            return null;

            bool getMenuItem(Menu menu, out MenuItem findMenuItem)
            {
                findMenuItem = null;
                if (menu == null)
                {
                    return false;
                }

                foreach (var menuItem in menu.Items)
                {
                    if (menuItem.MenuName == menuName)
                    {
                        {
                            findMenuItem = menuItem;
                            return true;
                        }
                    }
                }

                if (getMenuItem(menu.SubMenu, out findMenuItem))
                    return true;
                return false;
            }
        }


        public void CheckCollision()
        {
            // 使用 HashSet 存储已经存在的菜单项，以检测重复
            var menuItemNames = new HashSet<string>();
            var menuItemShortcuts = new HashSet<string>();

            foreach (var menu in Menus)
            {
                string path = menu.Title;
                CheckMenuItems(path, menu);
            }

            void CheckMenuItems(string path, Menu menu)
            {
                if (menu == null)
                    return;

                foreach (var menuItem in menu.Items)
                {
                    string menuItemMenuName = $"{path}/{menuItem.MenuName}";
                    if (!menuItemNames.Add(menuItemMenuName))
                    {
                        Debug.LogError($"Duplicate menu item found: {menuItemMenuName}");
                    }
                    if (!string.IsNullOrEmpty(menuItem.Shortcut)
                        && !menuItemShortcuts.Add(menuItem.Shortcut))
                    {
                        Debug.LogError($"Duplicate menu shortCut found: {menuItemMenuName} -- {menuItem.Shortcut}");
                    }
                }

                if (menu.SubMenu != null)
                {
                    string subMenuPath = $"{path}/{menu.SubMenu.Title}";
                    CheckMenuItems(subMenuPath, menu.SubMenu);
                }
            }
        }
    }

    public class Menu
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlAttribute("icon")]
        public string Icon { get; set; }

        [XmlElement("MenuItem")]
        public List<MenuItem> Items { get; set; }
        [XmlElement("SubMenu")]
        public Menu SubMenu { get; set; }
    }

    public class MenuItem
    {
        [XmlAttribute("menuName")]
        public string MenuName { get; set; }
        [XmlAttribute("shortcut")]
        public string Shortcut { get; set; }
        [XmlAttribute("actionTarget")]
        public string ActionTarget { get; set; }
        [XmlAttribute("actionMethod")]
        public string ActionMethod { get; set; }
        [XmlAttribute("params")]
        public string Params { get; set; }

        public Action GetAction()
        {
            var targetType = this.GetType().Assembly.GetType(ActionTarget);
            if (targetType == null)
            {
                var assemblies = CompilationPipeline.GetAssemblies();
                foreach (Assembly compileAssembly in assemblies)
                {
                    var assembly = System.Reflection.Assembly.Load(compileAssembly.name);
                    targetType = assembly.GetType(ActionTarget);
                    if (targetType != null)
                    {
                        break;
                    }
                }
            }
            if (ActionMethod == "CreateAssetMenu")
            {
                Assert.IsNotNull(targetType, $"Please check the type is valid or full name {ActionTarget}");

                return () =>
                {
                    var instance = ScriptableObject.CreateInstance(targetType);
                    AssetDatabase.CreateAsset(instance, Params);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = instance;
                    Debug.Log($"Create Asset At: {Params}");
                };
            }
            else if (ActionMethod == "OpenURL")
            {
                return () => Application.OpenURL(Params);
            }
            else
            {
                Assert.IsNotNull(targetType, $"Please check the type is valid or full name {ActionTarget}");

                var methodInfo = targetType.GetMethod(ActionMethod, BindingFlags.Public | BindingFlags.Static);
                Assert.IsNotNull(methodInfo, $"Please check the method is static {targetType}.{ActionMethod}");
                return () => methodInfo.Invoke(null, null);
            }
        }

        public static void Test()
        {
            Debug.LogError("Hello world");
        }
    }

}
