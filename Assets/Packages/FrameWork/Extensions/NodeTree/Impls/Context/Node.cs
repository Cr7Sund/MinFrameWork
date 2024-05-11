using System.Collections.Generic;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using System;
using System.Threading;

namespace Cr7Sund.NodeTree.Impl
{
    public abstract class Node : AsyncLoadable, INode
    {
        private readonly IAssetKey _key;
        private List<INode> _childNodes;
        private INode _parent;
        protected IContext _context;
        private Promise _addStatus;
        private Promise _removeStatus;
        private CancellationTokenSource _addCancellation;
        private CancellationTokenSource _removeCancellation;

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
        public INode this[int index] => GetChild(index);
        public IContext Context => _context;

        public CancellationTokenSource AddCancellation
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

        protected CancellationTokenSource RemoveCancellation
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
            await child.PreLoadAsync();
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

        public async PromiseTask Start(CancellationToken cancellation)
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
                    await first.OnStart();
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

        public async PromiseTask Disable()
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

                    await first.OnDisable();
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

                    await first.OnStopAsync();
                    first.IsStarted = false;
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
                await Disable();
            }
        }

        public async PromiseTask CancelCurTask()
        {
            switch (NodeState)
            {
                case NodeState.Adding:
                case NodeState.Preloading:
                    await CancelLoadAsync(AddCancellation.Token);
                    return;
                case NodeState.Unloading:
                case NodeState.Removing:
                    CancelUnload();
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

        public async PromiseTask CancelLoadChild(INode child)
        {
            if (child.NodeState != NodeState.Adding)
            {
                Console.Warn("try to cancel an not adding node");
                return;
            }

            child.AddCancellation.Cancel();

            // { child.AddStatus.Cancel(); }
            await child.CancelLoadAsync(child.AddCancellation.Token);
        }

        public void CancelUnload()
        {
            if (LoadState != LoadState.Unloading) return;

            RemoveCancellation.Cancel();
        }

        public void Destroy(IContext parentContext)
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
                _removeStatus.TryReturn();
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
        public virtual PromiseTask OnStart() { return PromiseTask.CompletedTask; }
        public virtual PromiseTask OnEnable() { return PromiseTask.CompletedTask; }
        public virtual PromiseTask OnDisable() { return PromiseTask.CompletedTask; }
        public virtual PromiseTask OnStopAsync() { return PromiseTask.CompletedTask; }
        protected virtual void OnDispose() { }


        private void ReleaseCollection(Stack<INode> stack, Stack<INode> resultQueue)
        {
            if (!IsInjected) return;

            var contextInjectionBinder = _context.InjectionBinder;
            var poolBinder = contextInjectionBinder.GetInstance<IPoolBinder>();
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
                await child.AddStatus.AsNewTask();
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
                    await child.LoadAsync();
                }
                catch (Exception ex)
                {
                    // remove addStatus before unload all
                    if (child.AddStatus.CurState == PromiseState.Pending)
                    { child.AddStatus.RejectWithoutDebug(ex); }
                    child.AddStatus.TryReturn();
                    child.AddStatus = null;

                    //UnloadChildAsync
                    await RemoveChildAsyncInternal(child, true, false);
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
                child.AddStatus.TryReturn();
                child.AddStatus = null;

                await RemoveChildAsyncInternal(child, false, false);
                throw;
            }

            child.SetReady();
            if (child.AddStatus != null)
            {
                if (child.AddStatus.CurState == PromiseState.Pending)
                {
                    child.AddStatus.Resolve();
                }

                child.AddStatus.TryReturn();
                child.AddStatus = null;
            }
        }

        public async PromiseTask UnloadChildAsync(INode child, bool overwrite = false)
        {
            await RemoveChildAsyncInternal(child, true, overwrite);
        }

        public async PromiseTask RemoveChildAsync(INode child, bool overwrite = false)
        {
            await RemoveChildAsyncInternal(child, false, overwrite);
        }

        protected virtual void OnAddChild(INode child) { }
        protected virtual void OnRemoveChild(INode child) { }

        private async PromiseTask RemoveChildAsyncInternal(INode child, bool shouldUnload, bool overwrite)
        {
            AssertUtil.NotNull(child, NodeTreeExceptionType.EMPTY_NODE_REMOVE);

            if (child.NodeState == NodeState.Unloaded)
            {
                // please check LoadModule
                // ---- -----
                // await _parentNode.CancelLoadChild(assetNode);
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
                await child.RemoveStatus.AsNewTask();
                return;
            }

            child.StartUnload(shouldUnload);

            AssertUtil.IsNull(child.RemoveStatus);
            child.RemoveStatus = Promise.Create();

            try
            {
                if (shouldUnload)
                {
                    await child.SetActive(false);
                    await child.Stop();
                    await child.UnloadAsync();
                    UnloadChildInternal(child);
                }
                else
                {
                    await child.SetActive(false);
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
                    child.RemoveStatus.TryReturn();
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

        private CancellationTokenSource GetNewCancellation()
        {
            var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            var cancellationPool = poolBinder.GetOrCreate<CancellationTokenSource>();
            return cancellationPool.GetInstance();
        }

        private void ReturnCancellation(CancellationTokenSource cancellationToken)
        {
            if (cancellationToken != null)
            {
                var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
                var cancellationPool = poolBinder.GetOrCreate<CancellationTokenSource>();
                cancellationPool.ReturnInstance(cancellationToken);
            }
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
            _context.InjectionBinder.Injector.Deject(this);
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
    }
}
