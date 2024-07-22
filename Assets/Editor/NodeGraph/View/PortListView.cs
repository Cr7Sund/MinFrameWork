using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{
    public class PortListView : ListView, IView
    {
        private NodeView _parentView;
        private Label _staticHeader;
        public List<PortView> portViews = new();

        public void StartView(IView parentView)
        {
            _parentView = parentView as NodeView;

            this.name = "PortListView";

            _staticHeader = new Label();
            _staticHeader.AddToClassList(nameof(_staticHeader));
            hierarchy.Add(_staticHeader);
            _staticHeader.SendToBack();

            // Set Flags
            reorderable = true;
            showAddRemoveFooter = true;
            showBorder = true;
            showBoundCollectionSize = false;
            showFoldoutHeader = false;
            showAlternatingRowBackgrounds = AlternatingRowBackground.All;

            // FixedHeight is faster but doesn't play well with foldouts!
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            // animated is a mess in conjunction with dragging ports
            reorderMode = ListViewReorderMode.Simple;

            // style changes
            // Your ListView needs to take all the remaining space
            style.flexGrow = 1;
            // hook up internal callback methods
            makeItem = MakeItem;
            _parentView.Add(this);
        }

        public void StopView(IView parentView)
        {

        }

        public void UpdateHeader(string name)
        {
            _staticHeader.text = name;
        }

        private VisualElement MakeItem()
        {
            VisualElement itemRow = new VisualElement();
            itemRow.style.flexDirection = FlexDirection.Row;

            if (reorderable)
            {
                // add handlebars with hand symbol to imply dragging support
                itemRow.Add(NodeGraphSetting.CreateHandleBars());
            }

            // create a label that gives some indication about the 
            Label fieldLabel = new Label();
            fieldLabel.AddToClassList(nameof(fieldLabel));
            itemRow.Add(fieldLabel);

            PortView portView = new PortView();
            portView.SetParent(_parentView);
            itemRow.Add(portView);
            portView.BringToFront();

            portViews.Add(portView);
            return itemRow;
        }

    }
}