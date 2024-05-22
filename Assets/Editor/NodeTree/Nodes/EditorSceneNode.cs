using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

namespace Cr7Sund.NodeTree.Editor
{

    [System.Serializable, NodeMenuItem("Operations/Sub")]
    public class EditorSceneNode : EditorNode
    {
        // [Output(name = "Out")]

        [Input(name = "In"), SerializeField]
        public Vector4 input;
        [Output(name = "Output"), SerializeField]
        public Vector4 output;
        public List<int> objs = new List<int>();

        public override string name => "List";

        protected override void Process()
        {
            objs.Add(2);
            objs.Add(4);
            output = input;
        }


    }
}