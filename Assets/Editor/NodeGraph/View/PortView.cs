using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{
    public class PortView : BasePort, IView
    {
        private PortInfo _portInfo;
        public static Dictionary<Type, VectorImage> portGraphicDict = new Dictionary<Type, VectorImage>();


        public PortView(PortInfo portInfo, Orientation orientation) : base(orientation, portInfo.direction, portInfo.capacity)
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
            foreach (var item in portGraphicDict)
            {
                GameObject.DestroyImmediate(item.Value);
            }

            portGraphicDict.Clear();
        }

        public override bool CanConnectTo(BasePort other, bool ignoreCandidateEdges = true)
        {
            return base.CanConnectTo(other, ignoreCandidateEdges)
                    && DefaultConnectRule(other);
        }

        public void UpdatePort(PortInfo portInfo, Orientation orientation)
        {
            Direction = portInfo.direction;
            Capacity = portInfo.capacity;
            this.PortName = portInfo.portName;
            _portInfo = portInfo;

            UpdateStyle();
        }

        private bool DefaultConnectRule(BasePort other)
        {
            if (other is PortView otherPortView)
            {
                return otherPortView._portInfo.serializeType.Equals(this._portInfo.serializeType);
            }
            return false;
        }

    }
}