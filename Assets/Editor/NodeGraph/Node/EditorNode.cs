using System;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.IocContainer;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Package.EventBus.Api;
using INode = Cr7Sund.NodeTree.Api.INode;

namespace Cr7Sund.Editor.NodeGraph
{
    public class EditorKeys : AssetKey
    {
        public readonly GraphModel GraphModel;

        public EditorKeys(GraphModel graphModel, string path = null) : base(path)
        {
            this.GraphModel = graphModel;
        }
    }

    public class EditorContext : CrossContext, INodeContext
    {
        protected readonly EditorNode _editorNode;

        public EditorContext(EditorNode editorNode)
        {
            _editorNode = editorNode;
        }

        public virtual void AddComponents(INode node)
        {
        }

        public virtual void RemoveComponents()
        {
        }
    }

    public abstract class EditorNode
    {
        public readonly IModel modelData;
        public IView View;
        [Inject] protected IEventBus _eventBus;
        [Inject] protected IPoolBinder _poolBinder;
        [Inject] protected Manifest _manifest;

        private List<EditorNode> _childNodes = new();
        protected EditorNode _parent;
        protected INodeContext _context;
        protected bool _isStart;

        public List<EditorNode> ChildNodes => _childNodes;

        public EditorNode(IModel model)
        {
            this.modelData = model;
        }

        protected void AddChildAsync(EditorNode child)
        {
            child._context = child.CreateContext();
            _context.AddContext(child._context);
            child.Inject();

            child._parent = this;
            child.Start();
            _childNodes.Add(child);
        }

        public void UnloadChildAsync(EditorNode child)
        {
            _context.RemoveContext(child._context);
            _context.Deject(child);
            child.Stop();
            _childNodes.Remove(child);
        }

        public void Inject()
        {
            _context.AddComponents(null);
            _context.Inject(this);
        }

        public virtual void Start()
        {
            if (_isStart)
            {
                return;
            }
            _isStart = true;

            try
            {
                View = CreateView();
            }
            catch (System.Exception ex)
            {
                Console.Error(ex, this.GetType().ToString());
                throw ex;
            }
            if (View != null)
            {
                if (_parent is EditorNode parentNode)
                {
                    View.StartView(parentNode.View);
                }
                else
                {
                    View.StartView(null);
                }
            }

            OnStart();
            OnBindUI();
            IterateChildData();
            OnListen();
        }


        public virtual void Stop()
        {
            if (!_isStart)
            {
                return;
            }
            _isStart = false;

            for (int i = 0; i < ChildNodes.Count; i++)
            {
                ChildNodes[i].Stop();
            }

            if (_parent is EditorNode parentNode)
            {
                View.StopView(parentNode.View);
            }
            else
            {
                View.StopView(null);
            }

            OnStop();

            //PLAN : remove from injector;
            // IEventBus eventBus;
            // eventBus.RemoveObserver(this);
        }

        public EditorNode GetNodeByNodeName(string targetNodeName)
        {
            return ChildNodes
                .FirstOrDefault(nodeController => nodeController.modelData.Name == targetNodeName);
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnStop()
        {
        }

        protected virtual INodeContext CreateContext()
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

        protected virtual IView CreateView()
        {
            if (_manifest.TryGetValue(this.GetType().Name, out var outPut))
            {
                return Activator.CreateInstance(outPut.ViewType) as IView;
            }
            return null;
        }

        protected virtual void OnListen()
        {

        }

    }
}