using System.Collections.Generic;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.EventBus.Api;
using INode = Cr7Sund.NodeTree.Api.INode;

namespace Cr7Sund.Editor.NodeTree
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

        public List<EditorNode> ChildNodes => _childNodes;

        public EditorNode(IModel model)
        {
            this.modelData = model;
            _context = new EditorContext();
        }

        public void AddChildAsync(EditorNode child)
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
            if (_parent is EditorNode parentNode)
            {
                view.StopView(parentNode.view);
            }
            else
            {
                view.StopView(null);
            }

            //PLAN : remove it;
            // IEventBus eventBus;
            // eventBus.RemoveObserver(this);
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
                     nodeCtrl.modelData.serializedProperty = nodeCtrl.modelData.OnBindSerializedProperty(modelData.serializedProperty, index);
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