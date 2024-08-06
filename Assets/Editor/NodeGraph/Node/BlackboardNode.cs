using System;
using Cr7Sund.Package.EventBus.Api;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor.NodeGraph
{
    public class BlackboardNode : EditorNode
    {
        private VisualElement inspectorRoot;
        [Inject] private IEventBus eventBus;

        public BlackboardView BlackboardView => View as BlackboardView;

        private const string nodeDefine = "NodeSettings";
        private const string graphDefine = "GraphSettings";

        public BlackboardNode(BlackboardInfo blackboardInfo, VisualElement uxmlRoot) : base(blackboardInfo)
        {
            this.inspectorRoot = uxmlRoot.Q<VisualElement>("inspectorRoot");
        }


        protected override void OnListen()
        {
            eventBus.AddObserver<SelectNodeEvent>(OnSelect);
        }

        private void OnSelect(SelectNodeEvent eventData)
        {
            BlackboardView.UpdateTab(nodeDefine, eventData.nodeController.nodeModel.serializedProperty, eventData.nodeController.nodeModel);
            BlackboardView.SelectTab(1);
            eventBus.Dispatch(new RebindUISignal());
        }

        protected override IView CreateView()
        {
            System.Action<UnityEditor.SerializedProperty, VisualElement, IModel, string> customDrawLogic = (serialProp, parentElement, model, tabName) =>
                        {
                            if (tabName == graphDefine)
                            {
                                SerializedPropertyHelper.CreateGenericUI(serialProp, parentElement);
                            }
                            else if (tabName == nodeDefine)
                            {
                                var nodeModel = model as NodeModel;
                                NodeParamsView.DrawInspector(nodeModel.nodeParamsInfo, parentElement);
                            }
                        };
            if (_manifest.TryGetValue(nameof(BlackboardNode), out var outPut))
            {
                return Activator.CreateInstance(outPut.ViewType, this.inspectorRoot, customDrawLogic) as IView;
            }
            return base.CreateView();
        }

        protected override void OnBindUI()
        {
            BlackboardView.UpdateTab(graphDefine, modelData.serializedProperty, null);
        }
    }
}