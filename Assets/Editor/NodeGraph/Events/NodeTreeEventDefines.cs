using System;
using System.Collections.Generic;
using Cr7Sund.Package.EventBus.Impl;
using UnityEditor.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{
    // Same Level event
    public class AddNodeEvent : EventData
    {
        public Type NodeType { get; internal set; }
        public Vector2 createPos { get; internal set; }
        public bool isUtilityNode { get; internal set; }

        public override void Clear()
        {
            NodeType = null;
        }
    }

    public class OpenMenuEvent : EventData
    {
        public MouseDownEvent evt { get; internal set; }
        public Dictionary<Actions, Tuple<Func<bool>, Action<object>>> commands { get; internal set; }

        public override void Clear()
        {
            evt = null;
        }
    }

    public class SelectNodeEvent : EventData
    {
        public NodeController nodeController { get; internal set; }

        public override void Clear()
        {
            nodeController = null;
        }
    }

    public class AddPortListEvent : EventData
    {
        internal BasePort edgeView;
        internal PortInfo portInfo;
        internal NodeModel containerNode;

        public override void Clear()
        {
            portInfo = null;
            edgeView = null;
            containerNode = null;
        }
    }

    #region Signal
    public class RebindUISignal : EventData
    {
        public override void Clear()
        {
        }
    }

    #endregion

}