using System;
using System.Collections.Generic;
using UnityEditor.GraphView;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeTree
{
    public class PortView : BasePort, IView
    {
        private PortInfo _portInfo;
        public static Dictionary<Type, VectorImage> portGraphicDict = new Dictionary<Type, VectorImage>();


        public PortView(PortInfo portInfo) : base(portInfo.orientation, portInfo.direction, portInfo.capacity)
        {
            this.PortName = portInfo.portName;
            _portInfo = portInfo;
        }

        public PortView() : base(Orientation.Horizontal, Direction.Input, PortCapacity.Single)
        {
        }

        private void UpdateStyle()
        {
            Type keyType = _portInfo.GetPortType();
            PortColor = NodeGraphSetting.Instance.portTypePalette.GetColor(keyType);

            if (!portGraphicDict.TryGetValue(keyType, out var portCircleGraphics))
            {
                portCircleGraphics = VectorImage.CreateInstance<VectorImage>();
                RepaintCircleGraphics(portCircleGraphics, PortColor);
                portGraphicDict.Add(keyType, portCircleGraphics);
            }
            m_ConnectorBox.style.backgroundImage = new StyleBackground(portCircleGraphics);
        }

        public void StartView(IView parentView)
        {
            if (parentView is NodeView nodeView)
            {
                nodeView.AddPort(this);
            }
            // port list controller

            UpdateStyle();
        }

        public void StopView(IView parentView)
        {
            if (parentView is NodeView nodeView)
            {
                nodeView.RemovePort(this);
            }
        }

        public override bool CanConnectTo(BasePort other, bool ignoreCandidateEdges = true)
        {
            return base.CanConnectTo(other, ignoreCandidateEdges)
                    && DefaultConnectRule(other);
        }

        public void UpdatePort(PortInfo portInfo)
        {
            Orientation = portInfo.orientation;
            Direction = portInfo.direction;
            Capacity = portInfo.capacity;
            this.PortName = portInfo.portName;
            _portInfo = portInfo;

            UpdateStyle();
        }

        private bool DefaultConnectRule(BasePort other)
        {
            if (other is PortView portView)
            {
                return portView._portInfo.fullTypeName == this._portInfo.fullTypeName;
            }
            return true;
        }

    }
}