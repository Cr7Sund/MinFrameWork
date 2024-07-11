using Cr7Sund.Package.EventBus.Api;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeTree
{
    public class BlackboardNode : EditorNode
    {
        private VisualElement inspectorRoot;
        [Inject] private IEventBus eventBus;

        public BlackboardView BlackboardView => view as BlackboardView;

        private const string nodeDefine = "NodeSettings";
        private const string graphDefine = "GraphSettings";

        public BlackboardNode(InspectorInfo inspectorInfo, VisualElement uxmlRoot) : base(inspectorInfo)
        {
            this.inspectorRoot = uxmlRoot.Q<VisualElement>("inspectorRoot");
        }


        protected override void OnListen()
        {
            eventBus.AddObserver<SelectNodeEvent>(OnSelect);
        }


        private void OnSelect(SelectNodeEvent eventData)
        {
            BlackboardView.UpdateTab(nodeDefine, eventData.nodeController.nodeModel.serializedProperty);
            BlackboardView.SelectTab(1);
            eventBus.DispatchSignal<RebindUISignal>();
        }

        protected override IView CreateView()
        {
            return new BlackboardView(this.inspectorRoot, (serialProp, parentElement) =>
            {
                SerializedPropertyHelper.CreateGenericUI(serialProp, parentElement);
            });
        }

        protected override void OnBindUI()
        {
            BlackboardView.UpdateTab(graphDefine, modelData.serializedProperty);
        }
    }
}