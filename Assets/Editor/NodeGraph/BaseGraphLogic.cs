using UnityEngine.UIElements;
using System;

namespace Cr7Sund.Editor.NodeGraph
{
    public interface IEditorGraphLogic
    {
        IAssetKey GraphKey { get; set; }

        void Init(VisualElement rootVisualElement);
        void Run();
        void Stop();
    }

    public class BaseGraphLogic : IEditorGraphLogic
    {
        protected GraphWindowController _graphWindowNode;
        IVisualElementScheduledItem _updateTask;

        public IAssetKey GraphKey { get; set; }

        public void Init(VisualElement rootVisualElement)
        {
            Console.Init(InternalLoggerFactory.Create());
            _graphWindowNode = new GraphWindowController(rootVisualElement, GraphKey);
            _graphWindowNode.AssignContext(new NodeGraphContext());

            _updateTask = rootVisualElement.schedule.Execute(OnEditorUpdate).Every(10);
        }

        public void Run()
        {
            try
            {
                _graphWindowNode.Inject();
                _graphWindowNode.Start();

                OnRun();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
            }
        }

        public void Stop()
        {
            try
            {
                GraphModel graphModel = _graphWindowNode.GraphNode.graphModel;
                OnStop();
                _updateTask.Pause();
                _updateTask = null;
                _graphWindowNode.Stop();
                _graphWindowNode = null;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
            }
        }

        protected virtual void OnRun()
        {
        }

        protected virtual void OnStop()
        {
        }

        protected virtual void OnEditorUpdate()
        {
        }
    }
}