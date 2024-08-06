using UnityEditor;
using UnityEditor.GraphView;
using UnityEngine;

namespace Cr7Sund.Editor.NodeGraph
{
    [System.Serializable]
    public class BlackboardInfo : BaseModel
    {
        [SerializeField] private Orientation orientation = Orientation.Horizontal;

        public Orientation Orientation
        {
            get => orientation; set
            {
                orientation = value;
                ApplyModification();
            }
        }

        public BlackboardInfo(BaseModel parentModel) : base(parentModel)
        {

        }

        public override SerializedProperty OnBindSerializedProperty(IModel model, SerializedProperty parentSerializedProperty, int index)
        {
            return parentSerializedProperty.FindPropertyRelative("blackboardInfo");
        }
    }
}