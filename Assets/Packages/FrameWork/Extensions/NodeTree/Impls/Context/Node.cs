using System;
using System.Collections.Generic;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.PackageTest.Util;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class Node : AsyncLoadable<INode>, INode
    {
        private List<INode> _childNodes;
        private Node _parent;
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
            get;
            set;
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
        public IContext Context => _context;

        #region LifeCycle
        public override IPromise<INode> PreLoadAsync(INode self)
        {
            AssertUtil.IsTrue(LoadState == LoadState.Default);

            var selfNode = self as Node;
            selfNode.StartPreload();
            if (!IsInjected)
            {
                Inject();
            }
            if (!IsInit)
            {
                Init();
            }

            // since we don't add child first
            // on loaded will not be call 
            return base.PreLoadAsync(self);
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
                var top = stack.Pop() as Node;
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
                var first = queue.Dequeue() as Node;
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
                var top = stack.Pop() as Node;
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
                var first = queue.Dequeue() as Node;
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
                var top = stack.Pop() as Node;
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
                var first = queue.Dequeue() as Node;
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
                var top = stack.Pop() as Node;
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
                var first = queue.Dequeue() as Node;
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

        private void ReleaseCollection(Stack<INode> stack, Queue<INode> queue)
        {
            var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<INode>>();
            var queuePool = poolBinder.GetOrCreate<Queue<INode>>();

            stack.Clear();
            queue.Clear();

            stackPool.ReturnInstance(stack);
            queuePool.ReturnInstance(queue);
        }
        private void GetCollection(out Stack<INode> stack, out Queue<INode> queue)
        {
            var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<INode>>();
            var queuePool = poolBinder.GetOrCreate<Queue<INode>>();

            stack = stackPool.GetInstance();
            queue = queuePool.GetInstance();
            stack.Clear();
            queue.Clear();
        }
        private void DisposeRecursively(INode root)
        {
            GetCollection(out var stack, out var queue);

            // N-ary Tree Postorder Traversal
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop() as Node;
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
                var first = queue.Dequeue() as Node;
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
                return Promise<INode>.Rejected(new MyException(
                    $"can not add node when unloading", NodeTreeExceptionType.INVALID_NODESTATE));
            }
            if (child.LoadState == LoadState.Fail)
            {
                throw new NotImplementedException();
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
                    .Then(AddChildInternal);
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
                    childNode.DeInject();
                }
                childNode._removePromise = child.UnloadAsync(child)
                    .Then(RemoveChildInternal);
            }
            else
            {
                childNode._removePromise = RemoveChildInternal(childNode);
            }

            return childNode._removePromise;
        }
        private IPromise<INode> RemoveChildInternal(INode child)
        {
            AssertUtil.NotNull(child);
            AssertUtil.IsTrue(ChildNodes.Contains(child));

            var childNode = child as Node;
            ChildNodes.Remove(child);
            childNode._parent = null;
            OnRemoveChild(child);

            childNode.EndUnLoad(true);
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
        private void StartPreload() => NodeState = NodeState.Preload;
        private void EndPreload() => NodeState = NodeState.Preloaded;
        private void SetReady() => NodeState = NodeState.Ready;
        private void StartUnload(bool shouldUnload) =>
            NodeState = shouldUnload ? NodeState.Removing :
                NodeState.Unloading;
        private void EndUnLoad(bool unload) =>
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


        public override string ToString()
        {
            return Key.ToString();
        }
    }
}
