using UnityEditor;
using UnityEngine;

namespace Cr7Sund.Editor.NodeTree
{
    [CreateAssetMenu(menuName = "GraphModel")]
    public class ScriptableGraphModel : ScriptableObject
    {
        [SerializeReference]
        public GraphModel graphModelBase = new();

        public void Init(SerializedObject serializedObject)
        {
            graphModelBase.AssignSerializeObject(serializedObject);
            graphModelBase.serializedProperty = serializedObject.FindProperty("graphModelBase");
        }
    }
}