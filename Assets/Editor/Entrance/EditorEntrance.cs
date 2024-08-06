using UnityEditor;
using Cr7Sund.Editor.NodeGraph;
using Cr7Sund.Editor.NodeTree;

namespace Cr7Sund.Editor
{
    // ensure class initializer is called whenever scripts recompile
    [InitializeOnLoadAttribute]
    public static class EditorEntrance
    {
        // register an event handler when the class is initialized
        static EditorEntrance()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                NodeTreeProxyLogic.ClearGraph();
                NodeTreeProxyLogic.OpenGraph();
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                NodeTreeProxyLogic.ClearGraph();
            }
        }
    }
}