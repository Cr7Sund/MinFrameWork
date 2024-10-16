using System;
using UnityEngine;
namespace Cr7Sund.Editor.MenuItems
{
    public static class ShortcutHelper
    {
        public static bool IsCombineKey()
        {
            // 获取当前事件的修饰符和按键
            var modifiers = Event.current.modifiers;
            var keyCode = Event.current.keyCode;

            // 检查是否为组合键
            bool hasModifier = (modifiers & (EventModifiers.Control | EventModifiers.Shift | EventModifiers.Alt | EventModifiers.Command)) != 0;

            // 检查是否按下了合法的主键
            bool isValidKey = keyCode != KeyCode.None &&
                              keyCode != KeyCode.LeftAlt &&
                              keyCode != KeyCode.RightAlt &&
                              keyCode != KeyCode.LeftControl &&
                              keyCode != KeyCode.RightControl &&
                              keyCode != KeyCode.Mouse0 &&
                              keyCode != KeyCode.Mouse1 &&
                              keyCode != KeyCode.Mouse2;

            // 返回是否为组合键并且按键有效
            return hasModifier && isValidKey;
        }
        public static bool TryParseShortcut(string shortcut, out EventModifiers modifiers, out KeyCode keyCode)
        {
            modifiers = EventModifiers.None;
            keyCode = KeyCode.None;

            if (string.IsNullOrEmpty(shortcut))
                return false;

            var parts = shortcut.Split('+');
            if (parts.Length == 0)
                return false;

            foreach (var part in parts)
            {
                switch (part.Trim().ToLower())
                {
                    case "ctrl":
                        modifiers |= EventModifiers.Control;
                        break;
                    case "shift":
                        modifiers |= EventModifiers.Shift;
                        break;
                    case "alt":
                        modifiers |= EventModifiers.Alt;
                        break;
                    case "command":
                        modifiers |= EventModifiers.Command;
                        break;
                    default:
                        if (Enum.TryParse(part.Trim(), true, out KeyCode parsedKeyCode))
                        {
                            keyCode = parsedKeyCode;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                }
            }

            return keyCode != KeyCode.None;
        }
    }
}
