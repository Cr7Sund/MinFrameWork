using System;
using Cr7Sund.Editor.MenuItems;
using UnityEditor;
using UnityEngine;

namespace Cr7Sund.Editor.NodeGraph
{
    [CreateAssetMenu(menuName = "Cgraph")]
    public class ScriptableGraphModel : ScriptableObject
    {
        public const string str = "";
        [SerializeReference]
        public GraphModel graphModelBase = new();

        public void Init(SerializedObject serializedObject)
        {
            graphModelBase.AssignSerializeObject(serializedObject);
            graphModelBase.serializedProperty = serializedObject.FindProperty("graphModelBase");
        }
    }
}