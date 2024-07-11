
using UnityEditor.GraphView;
using UnityEngine;

namespace Cr7Sund.Editor.NodeTree
{
    public class EdgeView : IView
    {
        public Edge edge;

        public EdgeView(Edge edge, EdgeModel edgeModel)
        {
            var outputPort = edgeModel.outputNode.GetPortByIndex(edgeModel.outputId, Direction.Output);
            var type = outputPort.GetPortType();

            this.edge = edge;
            Color color = NodeGraphSetting.Instance.portTypePalette.GetColor(type);
            this.edge.ColorSelected = Color.cyan;
            this.edge.ColorUnselected = color;
            this.edge.InputColor = color;
            this.edge.OutputColor = color;
        }

        public void StartView(IView parentView)
        {
            if (parentView is GraphView graphView)
            {
                graphView.AddElement(edge);
                edge.DrawToCap = false;
                edge.DrawFromCap = false;
                edge.UpdateLayout();
            }
        }

        public void StopView(IView parentView)
        {
            if (parentView is GraphView graphView)
            {
                graphView.RemoveElement(edge);
            }
        }

        public static Edge CreateEdge(BasePort input, BasePort output)
        {
            var edge = new Edge();
            edge.Input = input;
            edge.Output = output;

            return edge;
        }
    }
}