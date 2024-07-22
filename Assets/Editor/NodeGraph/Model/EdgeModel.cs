using System;
using UnityEditor;
using UnityEngine;

namespace Cr7Sund.Editor.NodeGraph
{
    [Serializable]
    public class EdgeModel : BaseModel
    {
        [SerializeReference]
        public NodeModel outputNode;
        [SerializeReference]
        public NodeModel inputNode;
        public int outputId;
        public int inputId;
  

        public EdgeModel(NodeModel inputNode, NodeModel outNode, int inputId, int outputId, BaseModel parentModel):base(parentModel)
        {
            this.outputNode = outNode;
            this.inputNode = inputNode;
            this.inputId = inputId;
            this.outputId = outputId;
        }

        public override SerializedProperty OnBindSerializedProperty(IModel model, SerializedProperty parentSerializedProperty, int index)
        {
            var listProperty = parentSerializedProperty.FindPropertyRelative("edges");
            if (listProperty.arraySize <= index)
            {
                return _parentModel.AddChildModel(model, "edges");
            }
            else
            {
                return listProperty.GetArrayElementAtIndex(index);
            }
        }
    }

}