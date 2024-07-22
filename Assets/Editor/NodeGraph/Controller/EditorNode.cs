using System.Collections.Generic;
using System.Linq;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.EventBus.Api;
using INode = Cr7Sund.NodeTree.Api.INode;

namespace Cr7Sund.Editor.NodeGraph
{
    public class EditorKeys : AssetKey
    {
        public GraphModel graphModel;

        public EditorKeys(GraphModel graphModel, string path = null) : base(path)
        {
            this.graphModel = graphModel;
        }
    }

    public class EditorContext : CrossContext
    {
        protected EditorNode _editorNode;

        public EditorContext(EditorNode editorNode)
        {
            _editorNode = editorNode;
        }

        public override void AddComponents(INode self)
        {
        }
        public override void RemoveComponents()
        {
        }
    }

    public abstract class EditorNode
    {
        public IModel modelData;
        public IView view;
        [Inject] protected IEventBus _eventBus;
        [Inject] protected IPoolBinder _poolBinder;
        private List<EditorNode> _childNodes = new();
        protected EditorNode _parent;
        protected IContext _context;
        protected bool isStart;

        public List<EditorNode> ChildNodes => _childNodes;

        public EditorNode(IModel model)
        {
            this.modelData = model;
            _context = CreateContext();
        }


        protected void AddChildAsync(EditorNode child)
        {
            _context.AddContext(child._context);
            child._parent = this;
            child.Inject();
            child.Start();
            _childNodes.Add(child);
        }

        public void UnloadChildAsync(EditorNode child)
        {
            _context.RemoveContext(child._context);
            _context.InjectionBinder.Injector.Deject(child);
            child.Stop();
            _childNodes.Remove(child);
        }

        public void Inject()
        {
            _context.AddComponents(null);
            _context.InjectionBinder.Injector.Inject(this);
        }

        public virtual void Start()
        {
            if (isStart)
            {
                return;
            }
            isStart = true;

            view = CreateView();
            if (view != null)
            {
                if (_parent is EditorNode parentNode)
                {
                    view.StartView(parentNode.view);
                }
                else
                {
                    view.StartView(null);
                }
            }

            OnBindUI();

            IterateChildData();
            OnListen();
        }

        public virtual void Stop()
        {
            if (!isStart)
            {
                return;
            }
            isStart = false;

            for (int i = 0; i < ChildNodes.Count; i++)
            {
                ChildNodes[i].Stop();
            }

            if (_parent is EditorNode parentNode)
            {
                view.StopView(parentNode.view);
            }
            else
            {
                view.StopView(null);
            }

            //PLAN : remove from injector;
            // IEventBus eventBus;
            // eventBus.RemoveObserver(this);
        }

        public EditorNode GetNodeByNodeName(string targetNodeName)
        {
            return ChildNodes
                .FirstOrDefault(nodeController => nodeController.modelData.Name == targetNodeName);
        }

        protected virtual EditorContext CreateContext()
        {
            return new EditorContext(this);
        }

        private void IterateChildData()
        {
            if (modelData == null)
            {
                return;
            }

            modelData.IterateChildNodes((node, index) =>
             {
                 var nodeCtrl = CreateChildNode(node);
                 if (nodeCtrl != null)
                 {
                     nodeCtrl.modelData.serializedProperty = nodeCtrl.modelData.OnBindSerializedProperty(node, modelData.serializedProperty, index);
                     AddChildAsync(nodeCtrl);
                 }
             });
        }

        protected virtual void OnBindUI()
        {
        }

        protected virtual EditorNode CreateChildNode(IModel model)
        {
            return null;
        }

        protected abstract IView CreateView();

        protected virtual void OnListen()
        {

        }


    }
}