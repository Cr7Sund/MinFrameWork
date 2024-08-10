using System.Collections.Generic;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using System;
using Cr7Sund.IocContainer;

namespace Cr7Sund.NodeTree.Impl
{
    public abstract class Node : AsyncLoadable, INode
    {
        private readonly IAssetKey _key;
        private List<INode> _childNodes;
        private INode _parent;
        protected INodeContext _context;
        private Promise _addStatus;
        private Promise _removeStatus;
        private UnsafeCancellationTokenSource _addCancellation;
        private UnsafeCancellationTokenSource _removeCancellation;

        public INode Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }
        public NodeState NodeState
        {
            get;
            private set;
        }
        public IAssetKey Key => _key;
        public IPromise AddStatus { get => _addStatus; set => _addStatus = value as Promise; }
        public IPromise RemoveStatus { get => _removeStatus; set => _removeStatus = value as Promise; }
        public bool IsInjected
        {
            get;
            set;
        }
        public bool IsInit
        {
            get;
            set;
        }
        public bool IsStarted
        {
            get;
            set;
        }
        public bool IsActive
        {
            get;
            set;
        }

        public List<INode> ChildNodes
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
                if (_childNodes == null) return 0;
                else return _childNodes.Count;
            }
        }
        public INode this[int index] => GetChild(index);
        public INodeContext Context => _context;

        public UnsafeCancellationTokenSource AddCancellation
        {
            get
            {
                if (_addCancellation == null)
                {
                    _addCancellation = GetNewCancellation();
                }
                return _addCancellation;
            }
        }

        public UnsafeCancellationTokenSource RemoveCancellation
        {
            get
            {
                if (_removeCancellation == null)
                {
                    _removeCancellation = GetNewCancellation();
                }
                return _removeCancellation;
            }
        }

        public Node(IAssetKey assetKey)
        {
            _key = assetKey;
        }

        #region LifeCycle
        public async PromiseTask PreLoadChild(INode child)
        {
            child.StartPreload();

            //PLAN: test after remove below
            if (!child.IsInjected)
            {
                _context.AddContext(child.Context);
                child.Inject();
            }

            // since we don't add child first
            // on loaded will not be call 
            await child.PreLoadAsync(child.AddCancellation.Token);
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

        public async PromiseTask Start(UnsafeCancellationToken cancellation)
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
                var top = stack.Pop();
                resultQueue.Push(top);
                for (int i = top.ChildCount - 1; i >= 0; i--)
                {
                    if (!top.GetChild(i).IsStarted)
                    {
                        stack.Push(top.GetChild(i));
                    }
                }
            }

            try
            {
                while (resultQueue.Count > 0)
                {
                    var first = resultQueue.Pop();
                    first.IsStarted = true;
                    await first.OnStart(cancellation);
                }
            }
            finally
            {
                ReleaseCollection(stack, resultQueue);
            }
        }

        public async PromiseTask Enable()
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
                var top = stack.Pop();
                resultQueue.Push(top);
                for (int i = 0; i < top.ChildCount; i++)
                {
                    if (!top.GetChild(i).IsActive && top.GetChild(i).IsStarted)
                    {
                        stack.Push(top.GetChild(i));
                    }
                }
            }

            try
            {
                while (resultQueue.Count > 0)
                {
                    var first = resultQueue.Pop();
                    first.IsActive = true;
                    await first.OnEnable();
                }
            }
            finally
            {
                ReleaseCollection(stack, resultQueue);
            }
        }

        public async PromiseTask Disable(bool closeImmediately)
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
                var top = stack.Pop();
                resultQueue.Push(top);
                for (int i = top.ChildCount - 1; i >= 0; i--)
                {
                    if (top.GetChild(i).IsActive)
                    {
                        stack.Push(top.GetChild(i));
                    }
                }
            }

            try
            {
                while (resultQueue.Count > 0)
                {
                    var first = resultQueue.Pop();

                    await first.OnDisable(closeImmediately);
                    first.IsActive = false;
                }
            }
            finally
            {
                ReleaseCollection(stack, resultQueue);
            }
        }

        public async PromiseTask Stop()
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
                var top = stack.Pop();
                resultQueue.Push(top);
                for (int i = top.ChildCount - 1; i >= 0; i--)
                {
                    if (top.GetChild(i).IsStarted)
                    {
                        stack.Push(top.GetChild(i));
                    }
                }
            }

            try
            {
                while (resultQueue.Count > 0)
                {
                    var first = resultQueue.Pop();

                    await first.OnStop();
                    first.IsStarted = false;
                }
            }
            finally
            {
                ReleaseCollection(stack, resultQueue);
            }
        }

        public void CancelLoad()
        {
            GetCollection(out var stack, out var resultQueue);

            // N-ary Tree Postorder Traversal
            var root = this;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                resultQueue.Push(top);
                for (int i = top.ChildCount - 1; i >= 0; i--)
                {
                    INode node = top.GetChild(i);
                    if (node.NodeState == NodeState.Adding
                         && !node.AddCancellation.Token.IsCancellationRequested)
                    {
                        stack.Push(node);
                    }
                }
            }

            try
            {
                while (resultQueue.Count > 0)
                {
                    var first = resultQueue.Pop();
                    first.OnCancelLoad();
                }
            }
            finally
            {
                ReleaseCollection(stack, resultQueue);
            }
        }

        public void CancelUnLoad()
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
                var top = stack.Pop();
                resultQueue.Push(top);
                for (int i = top.ChildCount - 1; i >= 0; i--)
                {
                    INode node = top.GetChild(i);
                    if ((NodeState == NodeState.Unloading
                         || NodeState != NodeState.Removing)
                         && !node.RemoveCancellation.Token.IsCancellationRequested)
                    {
                        stack.Push(node);
                    }
                }
            }

            try
            {
                while (resultQueue.Count > 0)
                {
                    var first = resultQueue.Pop();
                    first.OnCancelUnLoad();
                }
            }
            finally
            {
                ReleaseCollection(stack, resultQueue);
            }
        }

        public async PromiseTask SetActive(bool active)
        {
            if (IsActive == active)
                return;

            if (active)
            {
                //delay create cancellation
                await Enable();
            }
            else
            {
                await Disable(false);
            }
        }

        public void CancelCurTask()
        {
            switch (NodeState)
            {
                case NodeState.Adding:
                case NodeState.Preloading:
                    CancelLoad();
                    return;
                case NodeState.Unloading:
                case NodeState.Removing:
                    CancelUnLoad();
                    return;
                case NodeState.Preloaded:
                case NodeState.Ready:
                case NodeState.Removed:
                case NodeState.Unloaded:
                case NodeState.Default:
                default:
                    return;
            }
        }

        public void OnCancelLoad()
        {
            if (NodeState != NodeState.Adding)
            {
                Console.Warn("try to cancel an not adding node: {NodeState}", NodeState);
                return;
            }
            if (AddCancellation.Token.IsCancellationRequested)
            {
                return;
            }
            // 😒 Why we need to await unload
            // if we Load SameInstance in race, 
            // A = ATask, B = BTask
            // A.Load -> A.Cancel, B.Load ->A.Finish
            // A win: A.Unload-> B.Load, new Task->valid
            // B win: B.Load-> A.Unload, After B->invalid

            // the situation 2 will cause expected result
            // 😊 but we can make sure when begin cancelling the cache asset will be remove from container
            // which means it will be decrease reference  count
            // and the new another load operation will be not be affected
            AddCancellation.Cancel();
            // await first.UnloadAsync();
        }

        public void OnCancelUnLoad()
        {
            if (NodeState != NodeState.Unloading
              || NodeState != NodeState.Removing)
            {
                Console.Warn($"try to cancel an not adding node: {NodeState}", NodeState);
                return;
            }
            if (RemoveCancellation.Token.IsCancellationRequested)
            {
                return;
            }

            RemoveCancellation.Cancel();
        }

        public void Destroy(INodeContext parentContext)
        {
            IsInit = false;
            _childNodes = null;
            _parent = null;

            if (_addCancellation != null)
            {
                ReturnCancellation(_addCancellation);
                _addCancellation = null;
            }
            if (_removeCancellation != null)
            {
                ReturnCancellation(_removeCancellation);
                _removeCancellation = null;
            }

            if (_removeStatus != null)
            {
                _removeStatus.Resolve();
                TryReturnPromise(_removeStatus);
                _removeStatus = null;
            }

            Deject();

            if (parentContext != null)
            {
                parentContext.RemoveContext(_context);
            }

            _context.Dispose();
            _context = null;
        }

        public override void Dispose()
        {
            base.Dispose();

            // NodeState = NodeState.Default;
            AssertUtil.IsTrue(_childNodes == null || _childNodes.Count <= 0);
            AssertUtil.IsFalse(IsInit, NodeTreeExceptionType.dispose_not_int);
            AssertUtil.IsNull(_addStatus);
            AssertUtil.IsNull(_removeStatus);
            OnDispose();
        }

        protected virtual void OnInit() { }
        public virtual PromiseTask OnStart(UnsafeCancellationToken cancellation) { return PromiseTask.CompletedTask; }
        public virtual PromiseTask OnEnable() { return PromiseTask.CompletedTask; }
        public virtual PromiseTask OnDisable(bool closeImmediately) { return PromiseTask.CompletedTask; }
        public virtual PromiseTask OnStop() { return PromiseTask.CompletedTask; }
        protected virtual void OnDispose() { }

        private void ReleaseCollection(Stack<INode> stack, Stack<INode> resultQueue)
        {
            if (!IsInjected) return;

            var poolBinder = _context.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<INode>>();
            var queuePool = poolBinder.GetOrCreate<Stack<INode>>();

            stack.Clear();
            resultQueue.Clear();

            stackPool.ReturnInstance(stack);
            queuePool.ReturnInstance(resultQueue);
        }

        private void GetCollection(out Stack<INode> stack, out Stack<INode> resultQueue)
        {
            var poolBinder = _context.GetInstance<IPoolBinder>();
            var stackPool = poolBinder.GetOrCreate<Stack<INode>>();
            var queuePool = poolBinder.GetOrCreate<Stack<INode>>();

            stack = stackPool.GetInstance();
            resultQueue = queuePool.GetInstance();
            stack.Clear();
            resultQueue.Clear();
        }
        #endregion

        #region INode
        public async PromiseTask AddChildAsync(INode child, bool overwrite = false)
        {
            AssertUtil.NotNull(child, NodeTreeExceptionType.EMPTY_NODE_ADD);

            // the below situations will be better handle outside 
            if (child.LoadState == LoadState.Unloading)
            {
                throw new MyException(
                    "can not add node when unloading");
            }
            if (child.NodeState == NodeState.Adding)
            {
                AssertUtil.NotNull(child.AddStatus);
                if (overwrite)
                {
                    child.AddStatus.Cancel();
                }
                await child.AddStatus.Join();
                return;
            }

            AssertUtil.IsNull(child.AddStatus);
            child.SetAdding();
            child.AddStatus = Promise.Create();

            if (!child.IsInjected)
            {
                _context.AddContext(child.Context);
                child.Inject();
            }

            child.Init();
            if (child.LoadState == LoadState.Default
                || child.LoadState == LoadState.Unloaded
                || child.LoadState == LoadState.Fail)
            {
                try
                {
                    await child.LoadAsync(child.AddCancellation.Token);
                }
                catch (Exception ex)
                {
                    // remove addStatus before unload all
                    if (child.AddStatus.CurState == PromiseState.Pending)
                    {
                        child.AddStatus.RejectWithoutDebug(ex);
                    }

                    TryReturnPromise(child.AddStatus);
                    child.AddStatus = null;

                    //UnloadChildAsync
                    await RemoveChildAsyncInternal(child, true, false, true);
                    throw;
                }
            }

            try
            {
                await AddChildInternal(child);
            }
            catch (Exception ex)
            {
                // remove addStatus before unload all
                if (child.AddStatus.CurState == PromiseState.Pending)
                { child.AddStatus.RejectWithoutDebug(ex); }
                TryReturnPromise(child.AddStatus);
                child.AddStatus = null;

                await RemoveChildAsyncInternal(child, false, false, true);
                throw;
            }

            child.SetReady();
            if (child.AddStatus != null)
            {
                if (child.AddStatus.CurState == PromiseState.Pending)
                {
                    child.AddStatus.Resolve();
                }

                TryReturnPromise(child.AddStatus);
                child.AddStatus = null;
            }
        }

        public async PromiseTask UnloadChildAsync(INode child, bool overwrite = false)
        {
            await RemoveChildAsyncInternal(child, true, overwrite, false);
        }

        public async PromiseTask RemoveChildAsync(INode child, bool overwrite = false)
        {
            await RemoveChildAsyncInternal(child, false, overwrite, false);
        }

        protected virtual void OnAddChild(INode child) { }
        protected virtual void OnRemoveChild(INode child) { }

        private async PromiseTask RemoveChildAsyncInternal(INode child, bool shouldUnload, bool overwrite, bool closeImmediately)
        {
            AssertUtil.NotNull(child, NodeTreeExceptionType.EMPTY_NODE_REMOVE);

            if (child.NodeState == NodeState.Unloaded)
            {
                // please check LoadModule
                // ---- -----
                // await _parentNode.OnCancelLoad(assetNode);
                // -- do something will unload asset node  --
                // await RemoveNodeFromNodeTree(assetKey, overwrite);
                // ---- -----
                return;
            }
            if (child.NodeState == NodeState.Removing
                || child.NodeState == NodeState.Unloading)
            {
                if (overwrite)
                {
                    child.RemoveStatus.Cancel();
                }
                await child.RemoveStatus.Join();
                return;
            }

            child.StartUnload(shouldUnload);

            AssertUtil.IsNull(child.RemoveStatus);
            child.RemoveStatus = Promise.Create();

            try
            {
                if (shouldUnload)
                {
                    // don't unload recursively
                    // let the use handle the disconnection 
                    // for (int i = child.ChildCount - 1; i >= 0; i--)
                    // {
                    //     await child.UnloadChildAsync(child.GetChild(i));
                    // }

                    await child.Disable(closeImmediately);
                    await child.Stop();
                    await child.UnloadAsync(child.RemoveCancellation.Token);
                    UnloadChildInternal(child);
                }
                else
                {
                    await child.Disable(closeImmediately);
                    RemoveChildInternal(child);
                }
            }
            catch (Exception e) // in case of another unexpected exceptions
            {
                if (child.RemoveStatus != null)
                {
                    child.RemoveStatus.Reject(e);
                }
                throw;
            }
            finally
            {
                if (child.RemoveStatus != null)
                {
                    if (child.RemoveStatus.CurState == PromiseState.Pending)
                    {
                        child.RemoveStatus.Resolve();
                    }
                    TryReturnPromise(child.RemoveStatus);
                    child.RemoveStatus = null;
                }
                child.EndUnLoad(shouldUnload);
            }
        }

        private void RemoveChildInternal(INode child)
        {
            AssertUtil.NotNull(child);

            ChildNodes.Remove(child);
            child.Parent = null;

            OnRemoveChild(child);
        }

        private void UnloadChildInternal(INode child)
        {
            RemoveChildInternal(child);

            child.Destroy(_context);
        }

        private async PromiseTask AddChildInternal(INode child)
        {
            AssertUtil.NotNull(child);
            AssertUtil.IsTrue(child.LoadState == LoadState.Loaded);
            AssertUtil.IsFalse(ChildNodes.Contains(child));

            child.Parent = this;
            var childParent = child.Parent;
            ChildNodes.Add(child);

            OnAddChild(child);

            if (childParent.IsStarted && !child.IsStarted)
            {
                await child.Start(AddCancellation.Token);
            }
            if (childParent.IsActive && !child.IsActive)
            {
                await child.SetActive(true);
            }
        }
        #endregion

        #region Load
        public void SetAdding() =>
            NodeState = NodeState.Adding;

        public void StartPreload() => NodeState = NodeState.Preloading;

        public void EndPreload() => NodeState = NodeState.Preloaded;

        public void SetReady() => NodeState = NodeState.Ready;

        public void StartUnload(bool shouldUnload) =>
            NodeState = shouldUnload ? NodeState.Unloading :
                NodeState.Removing;

        public void EndUnLoad(bool unload) => NodeState = unload ? NodeState.Unloaded :
            NodeState.Removed;

        protected override void OnPreLoaded()
        {
            EndPreload();
        }

        private UnsafeCancellationTokenSource GetNewCancellation()
        {
            // var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            // var cancellationPool = poolBinder.GetOrCreate<UnsafeCancellationTokenSource>();
            // return cancellationPool.GetInstance();
            return UnsafeCancellationTokenSource.Create();
        }

        private void ReturnCancellation(UnsafeCancellationTokenSource cancellationToken)
        {
            if (cancellationToken != null)
            {
                // var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
                // var cancellationPool = poolBinder.GetOrCreate<UnsafeCancellationTokenSource>();
                // cancellationToken.Dispose();
                // cancellationPool.ReturnInstance(cancellationToken);
                if (!cancellationToken.IsCancelling)
                {
                    cancellationToken.TryReturn();
                }
            }

        }

        #endregion

        #region Inject Config
        public void AssignContext(INodeContext context)
        {
            _context = context;
        }
        public virtual void Inject()
        {
            if (IsInjected)
                return;

            IsInjected = true;

            _context.AddComponents(this);
            _context.Inject(this);

            OnInject();
        }

        public virtual void Deject()
        {
            if (!IsInjected)
                return;
            IsInjected = false;

            OnDeject();
            _context.Deject(this);
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
            if (Key == null) return string.Empty;
            return Key.ToString();
        }

        public INode GetChild(int index)
        {
            AssertUtil.Greater(index, -1);
            AssertUtil.Greater(ChildNodes.Count, 0);

            return ChildNodes[index];
        }

        private void TryReturnPromise(IPromise promise)
        {
            promise.TryReturn(false);
        }
    }
}
