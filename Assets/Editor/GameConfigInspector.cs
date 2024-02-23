using System.Reflection;
using Cr7Sund.Config;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
namespace Cr7Sund.GameLogic.Editor
{
    [CustomEditor(typeof(GameConfig))]
    public class GameConfigInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root the property UI
            var container = new VisualElement();
            var gameConfig = target as GameConfig;

            var assemblyField = new PropertyField(
                serializedObject.FindProperty(nameof(gameConfig.GameAssembly)));
            var gameLogicField = new PropertyField(
                serializedObject.FindProperty(nameof(gameConfig.GameLogicTypeFullName)));
            var assemblyInfoField = new PropertyField(
                serializedObject.FindProperty(nameof(gameConfig.AssemblyInfo)));
            var visualElement = new HelpBox("Please check the info", HelpBoxMessageType.Warning)
            {
                name = "tips"
            };

            container.Add(assemblyField);
            container.Add(gameLogicField);
            container.Add(assemblyInfoField);
            assemblyField.RegisterValueChangeCallback((_) =>
            {
                if (gameConfig.GameAssembly != null)
                {
                    gameConfig.AssemblyInfo = gameConfig.GameAssembly.name;
                }

                if (!string.IsNullOrEmpty(gameConfig.AssemblyInfo) &&
                    !string.IsNullOrEmpty(gameConfig.GameLogicTypeFullName))
                {
                    var assembly = Assembly.Load(gameConfig.AssemblyInfo);
                    var type = assembly.GetType(gameConfig.GameLogicTypeFullName);
                    var helpBox = container.Q<HelpBox>("tips");

                    if (type == null)
                    {
                        if (helpBox == null)
                        {
                            container.Add(visualElement);
                        }
                        else
                        {
                            helpBox.visible = true;
                        }
                    }
                    else
                    {
                        if (helpBox != null)
                        {
                            helpBox.visible = false;
                        }
                    }
                }
            });

            // Return the finished UI
            return container;
        }


    }
}
