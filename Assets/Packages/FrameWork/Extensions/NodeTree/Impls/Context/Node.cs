using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class Node : AsyncLoadable<INode>, INode
    {
        private Action<Node> _addChildHandler;
        private Action<Node> _removeChildHandler;
        private List<Node> _childNodes;
        private Node _parent;

        protected IContext _context;

        public INode Parent
        {
            get
            {
                return _parent;
            }
        }
        public IAssetKey Key
        {
            get;
            set;
        }
        public NodeState NodeState
        {
            get;
            private set;
        }
        public bool IsInjected
        {
            get;
            protected set;
        }
        public bool IsInit
        {
            get;
            private set;
        }
        public bool IsStarted
        {
            get;
            private set;
        }
        public bool IsActive
        {
            get;
            private set;
        }

        protected List<Node> ChildNodes
        {
            get
            {
                return _childNodes ??= new List<Node>();
            }
        }


        #region LifeCycle

        public IPromise<INode> PreLoad(INode self)
        {
            AssertUtil.IsTrue(LoadState == LoadState.Default);
            Inject();
            if (!IsInit)
                Init();
            // since we don't add child first
            // on loaded will not be call 
            return PreLoadAsync(self);
        }

        public void Init()
        {
            if (IsInit)
            {
                return;
            }

            IsInit = true;
            OnInit();
        }

        public void Start()
        {
            if (IsStarted)
            {
                return;
            }

            GetCollection(out var stack, out var queue);

            // N-ary Tree Postorder Traversal
            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                queue.Enqueue(top);
                for (int i = top.ChildNodes.Count - 1; i >= 0; i--)
                {
                    if (!top.ChildNodes[i].IsStarted)
                    {
                        stack.Push(top.ChildNodes[i]);
                    }
                }
            }

            while (queue.Count > 0)
            {
                var first = queue.Dequeue();
                first.OnStart();
                first.IsStarted = true;
            }

            ReleaseCollection(stack, queue);
        }

        public void Enable()
        {
            if (IsActive || !IsStarted)
            {
                return;
            }

            GetCollection(out var stack, out var queue);

            // N-ary Tree Postorder Traversal
            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                queue.Enqueue(top);
                for (int i = top.ChildNodes.Count - 1; i >= 0; i--)
                {
                    if (!top.ChildNodes[i].IsActive && top.ChildNodes[i].IsStarted)
                    {
                        stack.Push(top.ChildNodes[i]);
                    }
                }
            }

            while (queue.Count > 0)
            {
                var first = queue.Dequeue();
                first.OnEnable();
                first.IsActive = true;
            }
            ReleaseCollection(stack, queue);
        }

        public void Disable()
        {
            if (!IsStarted || !IsActive)
            {
                return;
            }

            GetCollection(out var stack, out var queue);

            // N-ary Tree Postorder Traversal
            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                queue.Enqueue(top);
                for (int i = top.ChildNodes.Count - 1; i >= 0; i--)
                {
                    if (top.ChildNodes[i].IsActive)
                    {
                        stack.Push(top.ChildNodes[i]);
                    }
                }
            }

            while (queue.Count > 0)
            {
                var first = queue.Dequeue();
                first.OnDisable();
                first.IsActive = false;
            }
            ReleaseCollection(stack, queue);
        }
        public void Stop()
        {
            if (!IsStarted)
            {
                return;
            }

            GetCollection(out var stack, out var queue);

            // N-ary Tree Postorder Traversal

            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                queue.Enqueue(top);
                for (int i = top.ChildNodes.Count - 1; i >= 0; i--)
                {
                    if (top.ChildNodes[i].IsStarted)
                    {
                        stack.Push(top.ChildNodes[i]);
                    }
                }
            }

            while (queue.Count > 0)
            {
                var first = queue.Dequeue();
                first.OnStop();
                first.IsStarted = false;
            }
            ReleaseCollection(stack, queue);
        }
        public void Dispose()
        {
            if (!IsInit) return;
            if (IsActive)
            {
                SetActive(false);
            }

            Stop();

            if (LoadState == LoadState.Loading || LoadState == LoadState.Loaded)
                UnloadAsync(this).Then(DisposeRecursively);
            else
                DisposeRecursively(this);
        }


        protected virtual void OnInit() { }
        protected virtual void OnStart() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void OnStop() { }
        protected virtual void OnDispose() { }


        protected void SetActive(bool active)
        {
            if (IsActive == active)
                return;

            var controller = this as ILifeTime;

            if (active)
            {
                controller.Enable();
            }
            else
            {
                controller.Disable();
            }
        }

        private void ReleaseCollection(Stack<Node> stack, Queue<Node> queue)
        {
            var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<Node>>();
            var queuePool = poolBinder.GetOrCreate<Queue<Node>>();

            stack.Clear();
            queue.Clear();

            stackPool.ReturnInstance(stack);
            queuePool.ReturnInstance(queue);
        }
        private void GetCollection(out Stack<Node> stack, out Queue<Node> queue)
        {
            var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<Node>>();
            var queuePool = poolBinder.GetOrCreate<Queue<Node>>();

            stack = stackPool.GetInstance();
            queue = queuePool.GetInstance();
            stack.Clear();
            queue.Clear();
        }
        private void DisposeRecursively(INode root)
        {
            GetCollection(out var stack, out var queue);

            // N-ary Tree Postorder Traversal
            stack.Push((Node)root);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                queue.Enqueue(top);
                for (int i = top.ChildNodes.Count - 1; i >= 0; i--)
                {
                    if (top.ChildNodes[i].IsInit)
                    {
                        stack.Push(top.ChildNodes[i]);
                    }
                }
            }

            while (queue.Count > 0)
            {
                var first = queue.Dequeue();
                first.RealDispose();
            }

            ReleaseCollection(stack, queue);
        }
        private void RealDispose()
        {
            OnDispose();

            //IoC Dispose
            DeInject();
            _context.Dispose();

            //NodeTree Dispose
            _parent?.ChildNodes.Remove(this);
            ChildNodes.Clear();
            _parent = null;
            IsInit = false;
        }
        #endregion

        #region INode
        public IPromise<INode> AddChildAsync(INode child)
        {
            AssertUtil.NotNull(child, NodeTreeExceptionType.EMPTY_NODE_ADD);

            if (child.LoadState == LoadState.Loading)
            {
                return child.LoadStatus;
            }
            if (child.LoadState == LoadState.Unloading)
            {
                throw new MyException($"can not add node when unloading", NodeTreeExceptionType.INVALID_NODESTATE);
            }

            var implChildNode = child as Node;
            if (ChildNodes.Contains(implChildNode))
            {
                return child.LoadStatus;
            }

            _addChildHandler ??= AddChild;
            implChildNode._addChildHandler = _addChildHandler;

            _context.AddContext(implChildNode._context);
            if (!child.IsInjected)
            {
                child.Inject();
            }
            if (!child.IsInit)
            {
                child.Init();
            }

            if (child.LoadState == LoadState.Default || child.LoadState == LoadState.Unloaded)
            {
                // TO avoid closure
                // child.LoadAsync().Then(() => AddChild(child));
                // we will delay the resolve until child been created
                // -----  ----
                return child.LoadAsync(child);
            }
            else if (child.LoadState == LoadState.Loaded)
            {
                AddChild(implChildNode);
            }

            return child.LoadStatus;
        }
        public IPromise<INode> UnloadChildAsync(INode child)
        {
            return RemoveChildAsyncInternal(child, true);
        }
        public IPromise<INode> RemoveChildAsync(INode child)
        {
            return RemoveChildAsyncInternal(child, false);
        }

        protected virtual void OnAddChild(Node child) { }
        protected virtual void OnRemoveChild(Node child) { }

        private IPromise<INode> RemoveChildAsyncInternal(INode child, bool shouldUnload)
        {
            AssertUtil.NotNull(child, NodeTreeExceptionType.EMPTY_NODE_REMOVE);

            if (child.LoadState != LoadState.Loaded)
            {
                throw new MyException($"can not remove node at : {child.LoadState} State", NodeTreeExceptionType.INVALID_NODESTATE);
            }

            var implChildNode = child as Node;
            if (!ChildNodes.Contains(implChildNode)) return child.UnloadStatus;
            _removeChildHandler ??= RemoveChild;
            implChildNode._removeChildHandler = _removeChildHandler;

            if (implChildNode.IsActive)
            {
                implChildNode.SetActive(false);
            }
            if (child.IsStarted)
            {
                child.Stop();
            }
            if (implChildNode.IsInjected)
            {
                implChildNode.DeInject();
            }
            _context.RemoveContext(implChildNode._context);

            if (shouldUnload)
            {
                return child.UnloadAsync(child);
            }
            else
            {
                RemoveChild(implChildNode);
                return Promise<INode>.Resolved(child);
            }
        }
        private void RemoveChild(Node child)
        {
            AssertUtil.NotNull(child);
            AssertUtil.IsTrue(ChildNodes.Contains(child));

            ChildNodes.Remove(child);
            child._parent = null;
            OnRemoveChild(child);
        }
        private void AddChild(Node child)
        {
            AssertUtil.NotNull(child);
            AssertUtil.IsTrue(child.LoadState == LoadState.Loaded);
            AssertUtil.IsFalse(ChildNodes.Contains(child));

            ChildNodes.Add(child);
            OnAddChild(child);

            child._parent = this;

            var childParent = child._parent;
            if (childParent.IsStarted && !child.IsStarted)
            {
                child.Start();
            }

            if (childParent.IsActive && !child.IsActive)
            {
                child.SetActive(true);
            }
        }
        #endregion

        #region Load
        public bool IsLoading() => NodeState == NodeState.Loading || NodeState == NodeState.Preloading;
        public void StartLoad() => NodeState = NodeState.Loading;
        public void SetAdding() => NodeState = NodeState.Adding;
        public void StartPreload() => NodeState = NodeState.Preloading;
        public void EndPreload() => NodeState = NodeState.Preloaded;
        public void SetReady() => NodeState = NodeState.Ready;
        public void StartUnload(bool unload) =>
                            NodeState = unload ? NodeState.Removing :
                                                 NodeState.Unloading;
        public void EndLoad(bool unload) =>
                            NodeState = unload ? NodeState.Removed :
                                                 NodeState.Unloaded;

        protected override IPromise<INode> OnLoadAsync(INode content)
        {
            return Promise<INode>.Resolved(content);
        }
        protected override IPromise<INode> OnUnloadAsync(INode content)
        {
            return Promise<INode>.Resolved(content);
        }
        protected sealed override void OnLoaded()
        {
            _addChildHandler?.Invoke(this);
        }
        protected sealed override void OnUnloaded()
        {
            _removeChildHandler?.Invoke(this);
        }
        protected override void OnPreLoaded()
        {

        }

        #endregion

        #region Inject Config
        public virtual void Inject()
        {
            if (IsInjected)
                return;

            IsInjected = true;

            _context.AddComponents();
            _context.InjectionBinder.Injector.Inject(this);
        }

        public virtual void DeInject()
        {
            if (!IsInjected)
                return;
            IsInjected = false;

            _context.InjectionBinder.Injector.Uninject(this);
            _context.RemoveComponents();
        }

        #endregion

        public int Test_GetChildNodeCount()
        {
            return ChildNodes.Count;
        }
    }
}
