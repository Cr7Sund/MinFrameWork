using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace Cr7Sund.Editor
{
    public class ReflectionTools
    {
        [MenuItem("Assets/MyMenu/PeekAssemblyPath")]
        public static string PeekAssemblyPath()
        {
            if (Selection.activeObject is not AssemblyDefinitionAsset assemblyDef)
            {
                return String.Empty;
            }
            var jsonObject = JsonUtility.FromJson<AssemblyDefinition>(assemblyDef.text);

            string assemblyName = (string)jsonObject.name;
            // Get additional assembly info using CompilationPipeline
            Assembly assembly = CompilationPipeline.GetAssemblies().FirstOrDefault(a => a.name == assemblyName);
            EditorGUIUtility.systemCopyBuffer = assemblyName;
            Debug.Log($"AssetPath: {assemblyName}");
            return assemblyName;
        }

    }
}