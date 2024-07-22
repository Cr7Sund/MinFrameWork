using UnityEditor;
using UnityEditor.GraphView;
using UnityEngine;

namespace Cr7Sund.Editor.NodeGraph
{
    [System.Serializable]
    public class InspectorInfo : BaseModel
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

        public InspectorInfo(BaseModel parentModel) : base(parentModel)
        {

        }

        public override SerializedProperty OnBindSerializedProperty(IModel model, SerializedProperty parentSerializedProperty, int index)
        {
            return parentSerializedProperty.FindPropertyRelative("inspectorInfo");
        }
    }
}