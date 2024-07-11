using UnityEngine;

namespace UnityEditor.GraphView
{
    public static class PlatformUtils
    {
        public static readonly bool IsMac
            = Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer;
    }
}