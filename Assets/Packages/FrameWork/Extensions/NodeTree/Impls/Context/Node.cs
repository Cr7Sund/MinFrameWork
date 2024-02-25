using System.Collections.Generic;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class Node : AsyncLoadable<INode>, INode
    {
        private List<INode> _childNodes;
        private Node _parent;
        private IAssetKey _key;
        private IPromise<INode> _addPromise;
        private IPromise<INode> _removePromise;

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
            get
            {
                return _key;
            }
        }
        public NodeState NodeState
        {
            get;
            private set;
        }

        public IPromise<INode> AddStatus
        {
            get
            {
                return _addPromise;
            }
        }

        public IPromise<INode> RemoveStatus
        {
            get
            {
                return _removePromise;
            }
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

        protected List<INode> ChildNodes
        {
            get
            {
                return _childNodes ??= new List<INode>();
            }
        }
        public int ChildCount
        {
            get
            {
                return ChildNodes.Count;
            }
        }
        public INode this[int index] => ChildNodes[index];
        public IContext Context => _context;


        public Node(IAssetKey assetKey)
        {
            _key = assetKey;
        }

        #region LifeCycle

        public IPromise<INode> PreLoadChild(INode child)
        {
            if (child.LoadState != LoadState.Default)
            {
                return Promise<INode>.Rejected(new MyException(
                    $"Expected Default state , but it's {child.LoadState}"));
            }

            var childNode = child as Node;
            childNode.StartPreload();

            // since we don't add child first
            // on loaded will not be call 
            return childNode.PreLoadAsync(child);
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

            GetCollection(out var stack, out var resultQueue);

            // N-ary Tree Postorder Traversal
            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop() as Node;
                resultQueue.Push(top);
                for (int i = top.ChildNodes.Count - 1; i >= 0; i--)
                {
                    if (!top.ChildNodes[i].IsStarted)
                    {
                        stack.Push(top.ChildNodes[i]);
                    }
                }
            }

            while (resultQueue.Count > 0)
            {
                var first = resultQueue.Pop() as Node;
                first.OnStart();
                first.IsStarted = true;
            }

            ReleaseCollection(stack, resultQueue);
        }

        public void Enable()
        {
            if (IsActive || !IsStarted)
            {
                return;
            }

            GetCollection(out var stack, out var resultQueue);

            // N-ary Tree Postorder Traversal
            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop() as Node;
                resultQueue.Push(top);
                for (int i = 0; i < top.ChildNodes.Count; i++)
                {
                    if (!top.ChildNodes[i].IsActive && top.ChildNodes[i].IsStarted)
                    {
                        stack.Push(top.ChildNodes[i]);
                    }
                }
            }
            var list = new List<int>();

            while (resultQueue.Count > 0)
            {
                var first = resultQueue.Pop() as Node;
                first.OnEnable();
                first.IsActive = true;
            }
            ReleaseCollection(stack, resultQueue);
        }

        public void Disable()
        {
            if (!IsStarted || !IsActive)
            {
                return;
            }

            GetCollection(out var stack, out var resultQueue);

            // N-ary Tree Postorder Traversal
            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop() as Node;
                resultQueue.Push(top);
                for (int i = top.ChildNodes.Count - 1; i >= 0; i--)
                {
                    if (top.ChildNodes[i].IsActive)
                    {
                        stack.Push(top.ChildNodes[i]);
                    }
                }
            }

            while (resultQueue.Count > 0)
            {
                var first = resultQueue.Pop() as Node;
                first.OnDisable();
                first.IsActive = false;
            }
            ReleaseCollection(stack, resultQueue);
        }
        public void Stop()
        {
            if (!IsStarted)
            {
                return;
            }

            GetCollection(out var stack, out var resultQueue);

            // N-ary Tree Postorder Traversal

            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop() as Node;
                resultQueue.Push(top);
                for (int i = top.ChildNodes.Count - 1; i >= 0; i--)
                {
                    if (top.ChildNodes[i].IsStarted)
                    {
                        stack.Push(top.ChildNodes[i]);
                    }
                }
            }

            while (resultQueue.Count > 0)
            {
                var first = resultQueue.Pop() as Node;
                first.OnStop();
                first.IsStarted = false;
            }
            ReleaseCollection(stack, resultQueue);
        }
        public void Dispose()
        {
            if (!IsInit) return;
            if (IsActive)
            {
                SetActive(false);
            }
            if (IsStarted)
            {
                Stop();
            }

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

        private void ReleaseCollection(Stack<INode> stack, Stack<INode> resultQueue)
        {
            var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<INode>>();
            var queuePool = poolBinder.GetOrCreate<Stack<INode>>();

            stack.Clear();
            resultQueue.Clear();

            stackPool.ReturnInstance(stack);
            queuePool.ReturnInstance(resultQueue);
        }
        private void GetCollection(out Stack<INode> stack, out Stack<INode> resultQueue)
        {
            var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<INode>>();
            var queuePool = poolBinder.GetOrCreate<Stack<INode>>();

            stack = stackPool.GetInstance();
            resultQueue = queuePool.GetInstance();
            stack.Clear();
            resultQueue.Clear();
        }
        private void DisposeRecursively(INode root)
        {
            GetCollection(out var stack, out var resultQueue);

            // N-ary Tree Postorder Traversal
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop() as Node;
                resultQueue.Push(top);
                for (int i = top.ChildNodes.Count - 1; i >= 0; i--)
                {
                    if (top.ChildNodes[i].IsInit)
                    {
                        stack.Push(top.ChildNodes[i]);
                    }
                }
            }

            while (resultQueue.Count > 0)
            {
                var first = resultQueue.Pop() as Node;
                first.RealDispose();
            }

            ReleaseCollection(stack, resultQueue);
        }
        private void RealDispose()
        {
            OnDispose();

            //IoC Dispose
            Deject();
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
                return Promise<INode>.Rejected(new MyException(
                    $"can not add node when unloading", NodeTreeExceptionType.INVALID_NODESTATE));
            }
            if (child.LoadState == LoadState.Fail)
            {
                return Promise<INode>.Rejected(new MyException(
                    $"can not add node when already fail", NodeTreeExceptionType.INVALID_NODESTATE));
            }

            var childNode = child as Node;
            if (ChildNodes.Contains(childNode))
            {
                return child.LoadStatus;
            }

            childNode.SetAdding();

            _context.AddContext(childNode._context);
            if (!child.IsInjected)
            {
                child.Inject();
            }
            if (!child.IsInit)
            {
                child.Init();
            }

            if (child.LoadState == LoadState.Default
                || child.LoadState == LoadState.Unloaded)
            {
                childNode._addPromise = child.LoadAsync(child)
                                        .Then(AddChildInternal, ex =>
                                        {
                                            _context.RemoveContext(childNode._context);
                                            childNode.Dispose();
                                            childNode.EndUnLoad(true);
                                            return Promise<INode>.RejectedWithoutDebug(ex);
                                        });
            }
            else
            {
                childNode._addPromise = AddChildInternal(childNode);
            }

            return childNode._addPromise;
        }
        public IPromise<INode> UnloadChildAsync(INode child)
        {
            return RemoveChildAsyncInternal(child, true);
        }
        public IPromise<INode> RemoveChildAsync(INode child)
        {
            return RemoveChildAsyncInternal(child, false);
        }
        public IPromise<INode> GetCurStatus()
        {
            switch (NodeState)
            {
                case NodeState.Preloading:
                    return LoadStatus;
                case NodeState.Adding:
                    return AddStatus;
                case NodeState.Preloaded:
                case NodeState.Ready:
                    return LoadStatus;
                case NodeState.Unloading:
                case NodeState.Removing:
                case NodeState.Removed:
                    return RemoveStatus;
                case NodeState.Unloaded:
                    return UnloadStatus;
                case NodeState.Default:
                default:
                    return Promise<INode>.Resolved(this);
            }
        }

        protected virtual void OnAddChild(INode child) { }
        protected virtual void OnRemoveChild(INode child) { }

        private IPromise<INode> RemoveChildAsyncInternal(INode child, bool shouldUnload)
        {
            AssertUtil.NotNull(child, NodeTreeExceptionType.EMPTY_NODE_REMOVE);
            if (child.LoadState != LoadState.Loaded)
            {
                Promise<INode>.Rejected(new MyException(
                    $"can not remove node at : {child.LoadState} State", NodeTreeExceptionType.INVALID_NODESTATE));
            }
            AssertUtil.IsTrue(ChildNodes.Contains(child), NodeTreeExceptionType.REMOVE_NO_EXISTED);
            
            var childNode = child as Node;
            childNode.StartUnload(shouldUnload);

            if (childNode.IsActive)
            {
                childNode.SetActive(false);
            }

            _context.RemoveContext(childNode._context);

            if (shouldUnload)
            {
                if (child.IsStarted)
                {
                    child.Stop();
                }
                if (childNode.IsInjected)
                {
                    childNode.Deject();
                }
                childNode._removePromise = child.UnloadAsync(child)
                    .ContinueWith(() =>
                    {
                        RemoveChildInternal(childNode, shouldUnload);
                        return Promise<INode>.Resolved(childNode);
                    }); // if unload fail, we still remove from node tree
            }
            else
            {
                childNode._removePromise = RemoveChildInternal(childNode, shouldUnload);
            }

            return childNode._removePromise;
        }
        private IPromise<INode> RemoveChildInternal(INode child, bool shouldUnload)
        {
            AssertUtil.NotNull(child);
            AssertUtil.IsTrue(ChildNodes.Contains(child));

            var childNode = child as Node;
            ChildNodes.Remove(child);
            childNode._parent = null;
            OnRemoveChild(child);

            childNode.EndUnLoad(shouldUnload);
            return Promise<INode>.Resolved(child);
        }
        private IPromise<INode> AddChildInternal(INode child)
        {
            AssertUtil.NotNull(child);
            AssertUtil.IsTrue(child.LoadState == LoadState.Loaded);
            AssertUtil.IsFalse(ChildNodes.Contains(child));

            ChildNodes.Add(child);
            OnAddChild(child);

            var childNode = child as Node;
            childNode._parent = this;

            var childParent = childNode._parent;
            if (childParent.IsStarted && !child.IsStarted)
            {
                child.Start();
            }

            if (childParent.IsActive && !child.IsActive)
            {
                childNode.SetActive(true);
            }

            childNode.SetReady();
            return Promise<INode>.Resolved(child);
        }
        #endregion

        #region Load
        public bool IsLoading() => LoadState == LoadState.Loading;
        private void SetAdding() =>
            NodeState = NodeState.Adding;
        private void StartPreload() => NodeState = NodeState.Preloading;
        private void EndPreload() => NodeState = NodeState.Preloaded;
        private void SetReady() => NodeState = NodeState.Ready;
        private void StartUnload(bool shouldUnload) =>
            NodeState = shouldUnload ? NodeState.Removing :
                NodeState.Unloading;
        private void EndUnLoad(bool unload) =>
            NodeState = unload ? NodeState.Unloaded :
                NodeState.Removed;

        protected override IPromise<INode> OnLoadAsync(INode content)
        {
            return Promise<INode>.Resolved(content);
        }

        protected override IPromise<INode> OnPreloadAsync(INode content)
        {
            return Promise<INode>.Resolved(content);
        }
        protected override IPromise<INode> OnUnloadAsync(INode content)
        {
            return Promise<INode>.Resolved(content);
        }
        protected sealed override void OnLoaded()
        {

        }
        protected sealed override void OnUnloaded()
        {

        }
        protected override void OnPreLoaded()
        {
            EndPreload();
        }
        #endregion

        #region Inject Config
        public void AssignContext(IContext context)
        {
            _context = context;
        }
        public virtual void Inject()
        {
            if (IsInjected)
                return;

            IsInjected = true;

            _context.AddComponents(this);

            _context.InjectionBinder.Injector.Inject(this);
            OnInject();
        }

        public virtual void Deject()
        {
            if (!IsInjected)
                return;
            IsInjected = false;

            OnDeject();
            _context.InjectionBinder.Injector.Uninject(this);
            
            _context.RemoveComponents();
        }
        /// <summary>
        /// will be called after inject, 
        /// do something lik inject other elements
        /// </summary>
        protected virtual void OnInject()
        {

        }
        /// <summary>
        /// will be called before Deject, 
        /// do something lik deject other elements
        /// </summary>
        protected virtual void OnDeject()
        {

        }

        #endregion

        public override string ToString()
        {
            return Key.ToString();
        }
    }
}
