#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cr7Sund.Editor.MenuItems
{


    [InitializeOnLoad]
    public class SceneViewContextMenuInitializer
    {
        // Key that's got to be pressed when right clicking, to get a context menu.
        private const EventModifiers modifier = EventModifiers.Shift;
        private static List<Action<EventModifiers, KeyCode>> _shortCuts = new();

        static SceneViewContextMenuInitializer()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            OnHandleEvent();
        }
        private static void OnHandleEvent()
        {
            if (Event.current == null)
            {
                return;
            }

            if (Event.current.modifiers == modifier &&
                Event.current.button == 1 && Event.current.type == EventType.MouseDown)
            {
                Init();
            }

            if (ShortcutHelper.IsCombineKey())
            {
                foreach (var shortCut in _shortCuts)
                {
                    shortCut.Invoke(Event.current.modifiers, Event.current.keyCode);
                }
            }
        }

        private static void Init()
        {
            _shortCuts.Clear();

            MenuBar menuBar = MenuLoader.Load();
            var menu = new GenericMenu();
            foreach (var menuEntry in menuBar.Menus)
            {
                AddMenuItems(menuEntry.Title, menu, menuEntry);
            }

            menu.ShowAsContext();
            Event.current.Use(); // Consume the event to prevent further processing
        }

        private static void AddMenuItems(string path, GenericMenu menu, Menu menuData)
        {
            if (menuData == null)
            {
                return;
            }

            foreach (var menuEntry in menuData.Items)
            {
                var itemPath = $"{path}/{menuEntry.MenuName}";
                if (!string.IsNullOrEmpty(menuEntry.Shortcut))
                {
                    _shortCuts.Add(((modifiers, code) =>
                    {
                        if (ShortcutHelper.TryParseShortcut(menuEntry.Shortcut,
                            out var targetModifier, out var targetCode))
                        {
                            if (modifiers == targetModifier && targetCode == code)
                            {
                                menuEntry.GetAction()?.Invoke();
                            }
                        }
                    }));
                    
                }
                menu.AddItem(new GUIContent(itemPath, menuEntry.Shortcut), false,
                    () => menuEntry.GetAction()?.Invoke());
            }

            if (menuData.SubMenu != null && menuData.SubMenu.Items.Count > 0)
            {
                var subMenuPath = $"{path}/{menuData.SubMenu.Title}";
                AddMenuItems(subMenuPath, menu, menuData.SubMenu);
            }
        }
    }
}
#endif
