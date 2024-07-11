using System;
using UnityEditor;
using UnityEngine;

namespace Cr7Sund.Editor.NodeTree
{
    [Serializable]
    public class EdgeModel : IModel
    {
        [SerializeReference]
        public NodeModel outputNode;
        [SerializeReference]
        public NodeModel inputNode;
        public int outputId;
        public int inputId;
        public SerializedProperty serializedProperty { get; set; }

        public EdgeModel(NodeModel inputNode, NodeModel outNode, int inputId, int outputId)
        {
            this.outputNode = outNode;
            this.inputNode = inputNode;
            this.inputId = inputId;
            this.outputId = outputId;
        }

        public void IterateChildNodes(Action<IModel, int> action)
        {
        }

        public SerializedProperty OnBindSerializedProperty(SerializedProperty parentSerializedProperty, int index)
        {
            var listProperty = parentSerializedProperty.FindPropertyRelative("edges");
            return listProperty.GetArrayElementAtIndex(index);
        }
    }

}