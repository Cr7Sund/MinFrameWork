using System;
using System.Reflection;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Selector.Apis;
using Cr7Sund.Selector.Impl;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Impl;
using GraphProcessor;
using UnityEditor;
namespace Cr7Sund.NodeTree.Editor
{
    public class NodeTreeGraph : BaseGraph
    {
        protected override void OnInit()
        {
            var gameLauncherField = typeof(GameMgr)
                            .GetField("_launch", BindingFlags.NonPublic | BindingFlags.Instance);
            var gameLauncher = gameLauncherField.GetValue(GameMgr.Instance) as GameLauncher;

            var gameLogicField = typeof(GameLauncher)
                             .GetField("_gameLogic", BindingFlags.NonPublic | BindingFlags.Instance);
            var gameLogic = gameLogicField.GetValue(gameLauncher) as IGameLogic;

            var gameNodeField = typeof(GameLogic)
                            .GetField("_gameNode", BindingFlags.NonPublic | BindingFlags.Instance);
            var gameNode = gameLogicField.GetValue(gameLauncher) as GameNode;

            var parentNode = new EditorNode(gameNode);
            AddNode(parentNode);


            IContext context = gameNode.Context;
            IEventBus eventBus = context.InjectionBinder.GetInstance<IEventBus>();
            eventBus.AddObserver<SwitchSceneEvent>(OnSwitchScene);
            eventBus.AddObserver<RemoveSceneEndEvent>(OnUpdateScene);
            eventBus.AddObserver<AddSceneEndEvent>(OnUpdateScene);
            base.OnInit();
        }

        private void OnUpdateScene(BaseSceneEvent eventData)
        {
            var t = eventData.TargetScene;
            // node.OnUpdate();
            nodeView.Update();
        }
        private void OnSwitchScene(SwitchSceneEvent eventData)
        {
            var t = eventData.CurScene;
        }

        public void Connect(BaseNode inputNodeTarget, BaseNode outputNodeTarget,
                string inputFieldName, string inputId,
                string outputFieldName, string outputId)
        {
            var inputPort = inputNodeTarget.GetPort(inputFieldName, inputId);
            var outputPort = outputNodeTarget.GetPort(outputFieldName, outputId);

            Connect(inputPort, outputPort, true);
        }
    }
}