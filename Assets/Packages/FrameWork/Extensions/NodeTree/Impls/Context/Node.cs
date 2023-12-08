using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.NodeTree.Impls
{
    public class Node : AsyncLoadable, INode
    {
        private Action<Node> _addChildHandler;
        private Action<Node> _removeChildHandler;
        private readonly Action _disposeHandler;
        private List<Node> _childNodes;
        private IContext _context;
        private Node _parent;

        public bool IsInjected
        {
            get;
            private set;
        }
        public bool IsActive
        {
            get;
            private set;
        }
        public INode Parent
        {
            get
            {
                return _parent;
            }
        }
        public bool IsStarted
        {
            get;
            private set;
        }
        public bool IsInit
        {
            get;
            private set;
        }


        public Node()
        {
            _disposeHandler = DisposeRecursively;
        }

        #region LifeCycle
        void IInitializable.Init()
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

            var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<Node>>();
            var stack = stackPool.GetInstance();
            var queuePool = poolBinder.GetOrCreate<Queue<Node>>();
            var queue = queuePool.GetInstance();

            // N-ary Tree Postorder Traversal

            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                queue.Enqueue(top);
                for (int i = top._childNodes.Count; i >= 0; i--)
                {
                    if (!top._childNodes[i].IsStarted)
                    {
                        stack.Push(top._childNodes[i]);
                    }
                }
            }

            while (queue.Count > 0)
            {
                var first = queue.Dequeue();
                first.OnStart();
                first.IsStarted = true;
            }
            stack.Clear();
            stackPool.ReturnInstance(stack);
            queue.Clear();
            queuePool.ReturnInstance(queue);
        }
        public void Enable()
        {
            if (IsActive)
            {
                return;
            }

            var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<Node>>();
            var stack = stackPool.GetInstance();
            var queuePool = poolBinder.GetOrCreate<Queue<Node>>();
            var queue = queuePool.GetInstance();

            // N-ary Tree Postorder Traversal
            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                queue.Enqueue(top);
                for (int i = top._childNodes.Count; i >= 0; i--)
                {
                    if (!top._childNodes[i].IsActive)
                    {
                        stack.Push(top._childNodes[i]);
                    }
                }
            }

            while (queue.Count > 0)
            {
                var first = queue.Dequeue();
                first.OnEnable();
                first.IsActive = true;
            }
            stack.Clear();
            stackPool.ReturnInstance(stack);
            queue.Clear();
            queuePool.ReturnInstance(queue);
        }

        public void Disable()
        {
            if (!IsActive)
            {
                return;
            }

            var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<Node>>();
            var stack = stackPool.GetInstance();
            var queuePool = poolBinder.GetOrCreate<Queue<Node>>();
            var queue = queuePool.GetInstance();

            // N-ary Tree Postorder Traversal
            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                queue.Enqueue(top);
                for (int i = top._childNodes.Count; i >= 0; i--)
                {
                    if (top._childNodes[i].IsActive)
                    {
                        stack.Push(top._childNodes[i]);
                    }
                }
            }

            while (queue.Count > 0)
            {
                var first = queue.Dequeue();
                first.OnDisable();
                first.IsActive = false;
            }
            stack.Clear();
            stackPool.ReturnInstance(stack);
            queue.Clear();
            queuePool.ReturnInstance(queue);
        }
        public void Stop()
        {
            if (!IsStarted)
            {
                return;
            }

            var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<Node>>();
            var stack = stackPool.GetInstance();
            var queuePool = poolBinder.GetOrCreate<Queue<Node>>();
            var queue = queuePool.GetInstance();

            // N-ary Tree Postorder Traversal

            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                queue.Enqueue(top);
                for (int i = top._childNodes.Count; i >= 0; i--)
                {
                    if (top._childNodes[i].IsStarted)
                    {
                        stack.Push(top._childNodes[i]);
                    }
                }
            }

            while (queue.Count > 0)
            {
                var first = queue.Dequeue();
                first.OnStop();
                first.IsStarted = false;
            }
            stack.Clear();
            stackPool.ReturnInstance(stack);
            queue.Clear();
            queuePool.ReturnInstance(queue);
        }
        public void Dispose()
        {
            if (!IsInit) return;
            if (IsActive)
            {
                SetActive(false);
            }

            Stop();

            //先卸载完成后,再进行OnDispose的回调以及销毁IoC
            if (State == LoadState.Loading || State == LoadState.Loaded)
                UnloadAsync().Then(_disposeHandler);
            else
                DisposeRecursively();
        }
        public void SetActive(bool active)
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

        /// <summary>
        /// Init回调
        /// </summary>
        protected virtual void OnInit() { }
        /// <summary>
        /// 开始回调
        /// </summary>
        protected virtual void OnStart() { }
        /// <summary>
        /// Enable回调
        /// </summary>
        protected virtual void OnEnable() { }
        /// <summary>
        /// Disable回调
        /// </summary>
        protected virtual void OnDisable() { }
        /// <summary>
        /// 停止回调
        /// </summary>
        protected virtual void OnStop() { }
        /// <summary>
        /// 销毁回调
        /// </summary>
        protected virtual void OnDispose() { }

        private void DisposeRecursively()
        {
            var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<Node>>();
            var stack = stackPool.GetInstance();
            var queuePool = poolBinder.GetOrCreate<Queue<Node>>();
            var queue = queuePool.GetInstance();

            // N-ary Tree Postorder Traversal
            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                queue.Enqueue(top);
                for (int i = top._childNodes.Count; i >= 0; i--)
                {
                    if (top._childNodes[i].IsInit)
                    {
                        stack.Push(top._childNodes[i]);
                    }
                }
            }

            while (queue.Count > 0)
            {
                var first = queue.Dequeue();
                first.RealDispose();
            }
            stack.Clear();
            stackPool.ReturnInstance(stack);
            queue.Clear();
            queuePool.ReturnInstance(queue);
        }
        private void RealDispose()
        {
            OnDispose();

            //IoC Dispose
            DeInject();
            _context.Dispose();

            //NodeTree Dispose
            _parent?._childNodes.Remove(this);
            _childNodes.Clear();
            _parent = null;
            IsInit = false;
        }
        #endregion

        #region INode
        public IPromise AddChildAsync(INode child)
        {
            AssertUtil.NotNull(NodeTreeExceptionType.EMPTY_NODE_ADD);
            if (child.State == LoadState.Loading || child.State == LoadState.Unloading)
            {
                throw new MyException($"can not add node at : {child.State} State", NodeTreeExceptionType.UNVALILD_NODESTATE);
            }

            var implChildNode = child as Node;
            if (_childNodes.Contains(implChildNode))
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

            if (child.State == LoadState.Default || child.State == LoadState.Unloaded)
            {
                // TO avoid anonymous function
                // we will delay the resolve function until child been created
                // -----  ----
                // child.LoadAsync().Then(() => AddChild(child));
                child.LoadAsync();
            }
            else if (child.State == LoadState.Loaded)
            {
                AddChild(implChildNode);
            }

            return child.LoadStatus;
        }
        public IPromise UnloadChildAsync(INode child)
        {
            return RemoveChildAsyncInternal(child, true);
        }
        public IPromise RemoveChildAsync(INode child)
        {
            return RemoveChildAsyncInternal(child, false);
        }
        private IPromise RemoveChildAsyncInternal(INode child, bool shouldUnload)
        {
            AssertUtil.NotNull(child, NodeTreeExceptionType.EMPTY_NODE_REMOVE);
            if (child.State != LoadState.Loaded)
            {
                throw new MyException($"can not remove node at : {child.State} State", NodeTreeExceptionType.UNVALILD_NODESTATE);
            }

            var implChildNode = child as Node;
            if (!_childNodes.Contains(implChildNode)) return child.UnloadStatus;

            _removeChildHandler ??= RemoveChild;
            implChildNode._removeChildHandler = _removeChildHandler;
            if (child.IsActive)
            {
                implChildNode.SetActive(false);
            }

            if (shouldUnload)
            {
                if (child.State == LoadState.Loaded)
                {
                    if (child.IsStarted)
                    {
                        child.Stop();
                    }

                    child.UnloadAsync();
                }
            }
            else
            {
                RemoveChild(implChildNode);
            }

            return child.UnloadStatus;
        }
        private void RemoveChild(Node child)
        {
            AssertUtil.NotNull(child);
            AssertUtil.IsFalse(_childNodes.Contains(child));

            _childNodes.Remove(child);
            child._parent = null;
            OnRemoveChild(child);
        }
        private void AddChild(Node child)
        {
            AssertUtil.NotNull(child);
            AssertUtil.IsTrue(child.State == LoadState.Loaded);
            AssertUtil.IsFalse(_childNodes.Contains(child));

            _childNodes.Add(child);
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
        protected virtual void OnAddChild(Node child) { }
        protected virtual void OnRemoveChild(Node child) { }
        #endregion

        #region Load
        protected override IPromise OnLoadAsync()
        {
            var promise = CreateLoadAsync();
            AssertUtil.NotNull(promise);

            foreach (var child in _childNodes)
            {
                promise.Then(child.LoadAsync);
            }
            return promise;
        }
        protected override IPromise OnUnloadAsync()
        {
            var promise = CreateUnLoadAsync();
            AssertUtil.NotNull(promise);

            foreach (var child in _childNodes)
            {
                promise.Then(child.LoadAsync);
            }
            return promise;
        }
        protected virtual IPromise CreateUnLoadAsync()
        {
            return Promise.Resolved();
        }
        protected virtual IPromise CreateLoadAsync()
        {
            return Promise.Resolved();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            _addChildHandler?.Invoke(this);
        }

        protected override void OnUnloaded()
        {
            base.OnUnloaded();
            _removeChildHandler?.Invoke(this);
        }
        #endregion

        #region Inject Config
        public virtual void Inject()
        {
            if (IsInjected)
                return;

            IsInjected = true;
            OnInjected();
        }

        public virtual void DeInject()
        {
            if (!IsInjected)
                return;

            IsInjected = false;
            OnDeInjected();
        }

        protected virtual void OnInjected()
        {
        }
        protected virtual void OnDeInjected()
        {
        }
        #endregion
    }
}
