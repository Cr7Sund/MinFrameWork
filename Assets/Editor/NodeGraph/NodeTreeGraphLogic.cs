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

    public class NodeTreeGraphLogic : IEditorGraphLogic
    {
        protected GraphWindowNode _graphWindowNode;

        public IAssetKey GraphKey { get; set; }


        public void Init(VisualElement rootVisualElement )
        {
            Console.Init(InternalLoggerFactory.Create());
            _graphWindowNode = new GraphWindowNode(rootVisualElement, GraphKey);
        }

        public virtual void Run()
        {
            try
            {
                _graphWindowNode.Start();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
            }
        }

        public virtual void Stop()
        {
            try
            {
                _graphWindowNode.Stop();
                _graphWindowNode = null;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
            }
        }
        
    }
}