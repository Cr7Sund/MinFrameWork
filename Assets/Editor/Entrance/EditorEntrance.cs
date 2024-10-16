using System;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using UnityEngine;

namespace Cr7Sund.Editor
{
    public class EditorEntranceConfig : ScriptableObject
    {
        public string[] EditorAssemblies;
    }
    
    // ensure class initializer is called whenever scripts recompile
    [InitializeOnLoadAttribute]
    public static class EditorEntrance
    {
        private static List<IUpdatable> _updates = new();
        private static List<IPlayModeChange> _lifeTimes = new();
        private const int _millSeconds = 200;
        private static readonly string[] EditorAssemblies =
        {
            "Cr7Sund.Editor.NodeTree"
        };


        static EditorEntrance()
        {
            foreach (var assemblyName in EditorAssemblies)
            {
                var currentAssem = Assembly.Load(assemblyName);
                var types = currentAssem.GetTypes();
                foreach (var type in types)
                {
                    if (typeof(IPlayModeChange).IsAssignableFrom(type))
                    {
                        var instance = Activator.CreateInstance(type) as IPlayModeChange;
                        RegisterPlayModeChangeCallback(instance);
                    }
                }
            }

            UnityEditor.EditorApplication.update += OnUpdate;
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        private static void RegisterUpdateCallback(IUpdatable updatable)
        {
            _updates.Add(updatable);
        }

        private static void UnRegisterUpdateCallback(IUpdatable updatable)
        {
            _updates.Remove(updatable);
        }

        public static void RegisterPlayModeChangeCallback(IPlayModeChange lifeTime)
        {
            foreach (var VARIABLE in _lifeTimes)
            {
                if (VARIABLE.GetType() == lifeTime.GetType())
                {
                    return;
                }
            }
            _lifeTimes.Add(lifeTime);
        }

        public static void UnRegisterPlayModeChangeCallback(IPlayModeChange lifeTime)
        {
            _lifeTimes.Remove(lifeTime);
        }

        private static void OnUpdate()
        {
            foreach (var update in _updates)
            {
                update.Update(_millSeconds);
            }
        }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                foreach (var item in _lifeTimes)
                {
                    item.Enable();
                }
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                foreach (var item in _lifeTimes)
                {
                    item.Disable();
                }
            }
        }
    }
}
