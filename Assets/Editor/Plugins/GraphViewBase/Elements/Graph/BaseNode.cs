// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

//using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.GraphView
{
    public class BaseNode : GraphElement
    {
        #region Constructor
        public BaseNode()
        {
            // Root Container
            MainContainer = this;

            // Title Label
            //TitleLabel = new() { pickingMode = PickingMode.Ignore };
            //TitleLabel.AddToClassList("node-title-label");

            // Title Container
            TitleContainer = new() { pickingMode = PickingMode.Ignore };
            TitleContainer.AddToClassList("node-title");
            //TitleContainer.Add(TitleLabel);
            //TitleContainer.Add(EditableTitleLabel);
            hierarchy.Add(TitleContainer);

            // Input Container
            InputContainer = new() { pickingMode = PickingMode.Ignore };
            InputContainer.AddToClassList("node-io-input");

            // Output Container
            OutputContainer = new() { pickingMode = PickingMode.Ignore };
            OutputContainer.AddToClassList("node-io-output");

            // Top Container 
            TopContainer = new() { pickingMode = PickingMode.Ignore };
            TopContainer.AddToClassList("node-io");
            TopContainer.Add(InputContainer);
            TopContainer.Add(OutputContainer);
            hierarchy.Add(TopContainer);

            // Extension Container
            ExtensionContainer = new() { pickingMode = PickingMode.Ignore };
            ExtensionContainer.AddToClassList("node-extension");
            hierarchy.Add(ExtensionContainer);

            // Style
            AddToClassList("node");

            // Capability
            Capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Deletable | Capabilities.Ascendable;
            usageHints = UsageHints.DynamicTransform;

            //Debug.Log(this.HasMouseCapture());
        }

        #endregion

        #region Properties
        //protected Label TitleLabel { get; }
        //protected TextField EditableTitleLabel { get; }
        public VisualElement MainContainer { get; }
        public VisualElement TitleContainer { get; }
        public VisualElement TopContainer { get; }
        public VisualElement InputContainer { get; }
        public VisualElement OutputContainer { get; }
        public VisualElement ExtensionContainer { get; }

        /*
        public override string Title {
            get => TitleLabel != null ? TitleLabel.text : string.Empty;
            set {
                if (TitleLabel != null) { TitleLabel.text = value; }
            }
        }*/

        public override bool Selected
        {
            get => base.Selected;
            set
            {
                if (base.Selected == value) { return; }
                base.Selected = value;
                if (value) { AddToClassList("node-selected"); } else { RemoveFromClassList("node-selected"); }
            }
        }
        #endregion

        #region Position
        public override void SetPosition(Vector2 newPosition)
        {
            base.SetPosition(newPosition);
        }
        #endregion

        #region Ports
        public virtual void AddPort(BasePort port)
        {
            port.ParentNode = this;
            if (port.Direction == Direction.Input) { InputContainer.Add(port); } else { OutputContainer.Add(port); }
        }
        public virtual void RemovePort(BasePort port)
        {
            port.ParentNode = null;
            if (port.Direction == Direction.Input) { InputContainer.Remove(port); } else { OutputContainer.Remove(port); }
        }
        #endregion

        #region Drag Events

        [EventInterest(typeof(DragOfferEvent))]
#pragma warning disable CS0672 // Member overrides obsolete member
        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
#pragma warning restore CS0672 // Member overrides obsolete member
        {
#pragma warning disable CS0618 // Type or member is obsolete
            base.ExecuteDefaultActionAtTarget(evt);
#pragma warning restore CS0618 // Type or member is obsolete
            if (evt.eventTypeId == DragOfferEvent.TypeId()) { OnDragOffer((DragOfferEvent)evt); }
        }

        private void OnDragOffer(DragOfferEvent e)
        {

            if (Graph != null && Graph.IsViewDrag(e))
            {
                Graph.OnDragOffer(e, true);
            }
            else
            {

                // Check if this is a node drag event 
                if (!IsNodeDrag(e) || !IsMovable())
                {
                    return;
                }

                // Accept Drag
                e.AcceptDrag(this);
            }
        }


        private bool IsNodeDrag<T>(DragAndDropEvent<T> e) where T : DragAndDropEvent<T>, new()
        {
            if ((MouseButton)e.button != MouseButton.LeftMouse) { return false; }
            if (!e.modifiers.IsNone()) { return false; }
            return true;
        }
        #endregion
    }
}