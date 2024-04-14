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
        public static readonly CancellationTokenSource UnitCancellation = new CancellationTokenSource();

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
        public IPromise RemoveStatus { get => _addStatus; set => _addStatus = value as Promise; }
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


        public Node(IAssetKey assetKey)
        {
            _key = assetKey;
        }

        #region LifeCycle

        public async PromiseTask PreLoadChild(INode child)
        {
            child.StartPreload();

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

        public async PromiseTask Start()
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

            List<Exception> aggregateExList = null;
            while (resultQueue.Count > 0)
            {
                var first = resultQueue.Pop();
                first.IsStarted = true;
                try
                {
                    await first.OnStart();
                }
                catch (Exception e)
                {
                    if (aggregateExList == null)
                    {
                        aggregateExList = new List<Exception>();
                    }
                    aggregateExList.Add(e);
                }
            }
            if (aggregateExList != null)
            {
                throw new AggregateException(aggregateExList);
            }
            ReleaseCollection(stack, resultQueue);
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

            List<Exception> aggregateExList = null;
            while (resultQueue.Count > 0)
            {
                var first = resultQueue.Pop();
                first.IsActive = true;
                try
                {
                    await first.OnEnable();
                }
                catch (Exception e)
                {
                    if (aggregateExList == null)
                    {
                        aggregateExList = new List<Exception>();
                    }
                    aggregateExList.Add(e);
                }
            }
            if (aggregateExList != null)
            {
                throw new AggregateException(aggregateExList);
            }
            ReleaseCollection(stack, resultQueue);
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

            List<Exception> aggregateExList = null;
            while (resultQueue.Count > 0)
            {
                var first = resultQueue.Pop();
                first.IsActive = false;
                try
                {
                    await first.OnDisable();
                }
                catch (Exception e)
                {
                    if (aggregateExList == null)
                    {
                        aggregateExList = new List<Exception>();
                    }
                    aggregateExList.Add(e);
                }
            }
            if (aggregateExList != null)
            {
                throw new AggregateException(aggregateExList);
            }
            ReleaseCollection(stack, resultQueue);
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

            List<Exception> aggregateExList = null;
            while (resultQueue.Count > 0)
            {
                var first = resultQueue.Pop();
                first.IsStarted = false;
                try
                {
                    await first.OnStop();
                }
                catch (Exception e)
                {
                    if (aggregateExList == null)
                    {
                        aggregateExList = new List<Exception>();
                    }
                    aggregateExList.Add(e);
                }
            }
            if (aggregateExList != null)
            {
                throw new AggregateException(aggregateExList);
            }
            ReleaseCollection(stack, resultQueue);
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

        public void CancelLoad()
        {
            if (LoadState != LoadState.Loading) return;

            if (_addCancellation == null)
            {
                _addCancellation = GetNewCancellation();
                if (!_addStatus.IsRecycled)
                {
                    _addCancellation.Token.Register(AddStatus.Cancel);
                }
                RegisterAddTask(_addCancellation.Token);
            }

            _addCancellation.Cancel();
        }

        public void CancelUnload()
        {
            if (LoadState != LoadState.Unloading) return;

            if (_removeCancellation == null)
            {
                _removeCancellation = GetNewCancellation();
                if (!_removeStatus.IsRecycled)
                {
                    _removeCancellation.Token.Register(RemoveStatus.Cancel);
                }
                RegisterRemoveTask(_removeCancellation.Token);
            }
            _removeCancellation.Cancel();
        }
        public virtual void RegisterAddTask(CancellationToken cancellationToken)
        {
        }
        public virtual void RegisterRemoveTask(CancellationToken cancellationToken)
        {
        }
        public void Destroy()
        {
            IsInit = false;
            _childNodes = null;
            _parent = null;

            ReturnCancellation(_addCancellation);
            ReturnCancellation(_removeCancellation);
            _addCancellation = null;
            _removeCancellation = null;
            if (_addStatus != null && !_addStatus.IsRecycled)
            {
                _addStatus.TryReturn();
            }
            if (_removeStatus != null && !_removeStatus.IsRecycled)
            {
                _removeStatus.TryReturn();
            }
            _addStatus = null;
            _removeStatus = null;

            Deject();
            _context = null;
        }

        public override void Dispose()
        {
            base.Dispose();

            AssertUtil.IsTrue(_childNodes == null || _childNodes.Count <= 0);
            AssertUtil.IsFalse(IsInit, NodeTreeExceptionType.dispose_not_int);
            OnDispose();
        }

        protected virtual void OnInit() { }
        public virtual PromiseTask OnStart() { return PromiseTask.CompletedTask; }
        public virtual PromiseTask OnEnable() { return PromiseTask.CompletedTask; }
        public virtual PromiseTask OnDisable() { return PromiseTask.CompletedTask; }
        public virtual PromiseTask OnStop() { return PromiseTask.CompletedTask; }
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
            AssertUtil.IsFalse(child.AddStatus != null && _addStatus.IsRecycled, NodeTreeExceptionType.ADD_RECYCLED);

            // the below situations will be better handle outside 
            if (child.LoadState == LoadState.Unloading)
            {
                throw new MyException(
                    "can not add node when unloading");
            }
            if (child.LoadState == LoadState.Fail)
            {
                throw new MyException(
                    "can not add node when already fail");
            }
            if (child.NodeState == NodeState.Adding)
            {
                if (overwrite)
                {
                    child.AddStatus.Cancel();
                }
                await child.AddStatus.AsNewTask();
                return;
            }

            child.SetAdding();

            if (child.AddStatus != null)
            {
                child.AddStatus.TryReturn();
            }
            child.AddStatus = Promise.Create();
            _context.AddContext(child.Context);

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
                try
                {
                    await child.LoadAsync();
                }
                catch
                {
                    await RemoveChildAsyncInternal(child, true, false);
                    throw;
                }
            }

            try
            {
                await AddChildInternal(child);
            }
            catch (Exception e)
            {
                await RemoveChildAsyncInternal(child, false, false);
                if (child.AddStatus.CurState == PromiseState.Pending)
                    child.AddStatus.Reject(e);
                throw;
            }

            if (child.AddStatus.CurState == PromiseState.Pending)
                child.AddStatus.Resolve();

            await child.AddStatus.AsNewTask();
        }

        public async PromiseTask UnloadChildAsync(INode child, bool overwrite = false)
        {
            if (child.LoadState != LoadState.Loaded)
            {
                throw new MyException(string.Format(
                     "can not remove node at : {LoadState} State", child.LoadState));
            }
            await RemoveChildAsyncInternal(child, true, overwrite);
        }

        public async PromiseTask RemoveChildAsync(INode child, bool overwrite = false)
        {
            if (child.LoadState != LoadState.Loaded)
            {
                throw new MyException(string.Format(
                     "can not remove node at : {LoadState} State", child.LoadState));
            }
            await RemoveChildAsyncInternal(child, false, overwrite);
        }

        protected virtual void OnAddChild(INode child) { }
        protected virtual void OnRemoveChild(INode child) { }

        private async PromiseTask RemoveChildAsyncInternal(INode child, bool shouldUnload, bool overwrite)
        {
            AssertUtil.NotNull(child, NodeTreeExceptionType.EMPTY_NODE_REMOVE);

            if (child.NodeState == NodeState.Removing
                || child.NodeState == NodeState.Unloading)
            {
                if (overwrite)
                {
                    RemoveStatus.Cancel();
                }
                await child.RemoveStatus.AsNewTask();
                return;
            }

            child.StartUnload(shouldUnload);

            if (child.RemoveStatus != null)
            {
                child.RemoveStatus.TryReturn();
            }
            child.RemoveStatus = Promise.Create();

            try
            {
                try
                {
                    if (child.IsActive)
                    {
                        await child.SetActive(false);
                    }
                }
                finally
                {
                    if (shouldUnload)
                    {
                        try
                        {
                            _context.RemoveContext(child.Context);
                            if (child.IsStarted)
                            {
                                await child.Stop();
                            }
                        }
                        finally
                        {
                            try
                            {
                                await child.UnloadAsync();
                            }
                            finally
                            {
                                UnloadChildInternal(child);
                            }
                        }
                    }
                    else
                    {
                        RemoveChildInternal(child, shouldUnload);
                    }
                }
            }
            catch (Exception ex)
            {
                if (child.RemoveStatus != null && child.RemoveStatus.CurState == PromiseState.Pending)
                {
                    child.RemoveStatus.Reject(ex);
                }
                throw;
            }

            if (child.RemoveStatus != null)
            {
                if (child.RemoveStatus.CurState == PromiseState.Pending)
                {
                    child.RemoveStatus.Resolve();
                }

                await child.RemoveStatus.AsNewTask();
            }
        }

        private void RemoveChildInternal(INode child, bool shouldUnload)
        {
            AssertUtil.NotNull(child);
            AssertUtil.IsNull(_removeStatus);

            ChildNodes.Remove(child);
            child.Parent = null;
            OnRemoveChild(child);
            child.EndUnLoad(shouldUnload);
        }

        private void UnloadChildInternal(INode child)
        {
            RemoveChildInternal(child, true);
            child.Destroy();
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
                await child.Start();
            }
            if (childParent.IsActive && !child.IsActive)
            {
                await child.SetActive(true);
            }

            child.SetReady();
        }

        #endregion

        #region Load
        public bool IsLoading() => LoadState == LoadState.Loading;
        public void SetAdding() =>
            NodeState = NodeState.Adding;
        public void StartPreload() => NodeState = NodeState.Preloading;
        public void EndPreload() => NodeState = NodeState.Preloaded;
        public void SetReady() => NodeState = NodeState.Ready;
        public void StartUnload(bool shouldUnload) =>
            NodeState = shouldUnload ? NodeState.Removing :
                NodeState.Unloading;
        public void EndUnLoad(bool unload) =>
            NodeState = unload ? NodeState.Unloaded :
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
