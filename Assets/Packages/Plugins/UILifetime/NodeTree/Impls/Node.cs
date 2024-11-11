using System.Collections.Generic;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using System;
using System.Threading.Tasks;
using Cr7Sund.LifeTime;

namespace Cr7Sund.LifeTime
{
    public abstract class Node : LifeCycleOwner, INode
    {
        private  IRouteKey _key;
        private List<INode> _childNodes;
        private INode _parent;
        private INodeContext _context;
        private Promise _addStatus;
        private Promise _removeStatus;


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
            set;
        }
        public IRouteKey Key => _key;
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
        private INodeContext Context
        {
            get
            {
                if (_context == null)
                {
                    return ((Node)Parent).Context;
                }

                return _context;
            }
        }

        
        public void Init(IRouteKey assetKey)
        {
            _key = assetKey;
        }

        #region LifeCycle
        #region Add
        public void Init()
        {
            if (IsInit)
            {
                return;
            }

            IsInit = true;
        }

        public async PromiseTask StartCreate(IRouteArgs fragmentContext, INode parentNode)
        {
            if (IsInit)
            {
                AssertUtil.IsNull(_addStatus);
                _addStatus = Promise.Create();
                return;
            }

            try
            {
                await Create(fragmentContext, parentNode, AddCancellation);
            }
            catch (Exception e)
            {
                AddFail(e);
                throw;
            }

            async Task Create(IRouteArgs intent, INode node, UnsafeCancellationTokenSource cancellationToken)
            {
                var unsafeCancellationToken = cancellationToken.Token;
                await OnCreateNode(unsafeCancellationToken, intent);

                if (node != null)
                {
                    node.AddChild(this);
                }
                if (!IsInjected)
                {
                    Inject(fragmentContext);
                }
                Init();

                MarkNodeState(NodeState.Adding, unsafeCancellationToken, intent);
                await MarkState(LifeCycleState.Initialized, unsafeCancellationToken, intent);

                if (cancellationToken.IsCancelling)
                {
                    return;
                }

                await OnNodeCreated(unsafeCancellationToken, intent);
            }
        }

        public async PromiseTask PreloadView(IRouteArgs fragmentContext)
        {
            if (AddCancellation.IsCancelling)
            {
                return;
            }

            try
            {
                await OnLoadAsync(AddCancellation.Token, fragmentContext);
            }
            catch (Exception e)
            {
                AddFail(e);
                throw;
            }
        }

        public PromiseTask EndLoad()
        {
            return MarkState(LifeCycleState.Created, AddCancellation.Token);
        }

        public async PromiseTask Start(UnsafeCancellationToken cancellationToken)
        {
            if (lifeCycleState != LifeCycleState.Created)
            {
                return;
            }

            await Traversal(true,
                (node) => node.lifeCycleState == LifeCycleState.Created,
                async (node, token) =>
                {
                    node.IsStarted = true;
                    await node.OnStart(token);
                    // node.NodeState = NodeState.Started;
                    await node.MarkState(LifeCycleState.Started, token);
                });
        }

        public async PromiseTask AppendTransition()
        {
            if (AddCancellation.IsCancelling)
            {
                return;
            }

            try
            {
                await OnTransition(AddCancellation.Token);
            }
            catch (Exception e)
            {
                AddFail(e);
                throw;
            }
        }

        public async PromiseTask Enable()
        {
            if (lifeCycleState != LifeCycleState.Started)
            {
                return;
            }

            await Traversal(true,
                (node) => node.lifeCycleState == LifeCycleState.Started,
                async (node, token) =>
                {
                    node.IsActive = true;
                    await node.OnEnable(token);
                    await node.MarkState(LifeCycleState.Resumed, token);
                });
        }

        public void EndCreate()
        {
            AssertUtil.NotNull(AddStatus);

            TryReturnPromise(AddStatus);
            AddStatus = null;

            MarkNodeState(NodeState.Ready, AddCancellation.Token);
        }
   #endregion

        #region Remove
        public void StartDestroy(bool isRemove)
        {
            AssertUtil.IsNull(RemoveStatus);

            RemoveStatus = Promise.Create();
            MarkNodeState(isRemove ? NodeState.Removing : NodeState.Unloading, RemoveCancellation.Token);
        }

        public async PromiseTask Disable()
        {
            if (!IsStarted || !IsActive)
            {
                return;
            }

            await Traversal(false,
                (node) => node.lifeCycleState == LifeCycleState.Resumed,
                async (node, token) =>
                {
                    node.IsActive = false;
                    await node.OnDisable(token);
                    await node.MarkState(LifeCycleState.Started, token);
                });

        }

        public async PromiseTask Stop(UnsafeCancellationToken cancellationToken)
        {
            if (!IsStarted)
            {
                return;
            }
            await Traversal(false,
                (node) => node.lifeCycleState == LifeCycleState.Started, async (node, token) =>
                {
                    node.IsStarted = false;
                    await node.OnStop(token);
                    await node.MarkState(LifeCycleState.Created, token);
                });
        }

        public sealed override void Destroy()
        {
            base.Destroy();

            IsInit = false;
            _childNodes = null;
            _parent = null;

            OnDestroy();
            Deject();

            if (Parent != null)
            {
                Parent.RemoveChild(this);

                if (_context != null)
                {
                    var parentContext = ((Node)Parent).Context;
                    if (parentContext != null)
                    {
                        parentContext.RemoveContext(_context);
                    }

                    _context.Dispose();
                    _context = null;
                }
            }

            MarkNodeState(NodeState.Unloaded, RemoveCancellation.Token);
        }

        public void EndDestroy(bool isRemoved = false)
        {
            // state - deinjected
            AssertUtil.NotNull(RemoveStatus);

            TryReturnPromise(RemoveStatus);
            RemoveStatus = null;

            MarkNodeState(isRemoved ? NodeState.Removed : NodeState.Unloaded, AddCancellation.Token);
        }
          #endregion

        #region Callbacks
        protected virtual PromiseTask OnCreateNode(UnsafeCancellationToken cancellation, IRouteArgs fragmentContext) { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnNodeCreated(UnsafeCancellationToken cancellation, IRouteArgs fragmentContext) { return PromiseTask.CompletedTask; }

        protected virtual PromiseTask OnLoadAsync(UnsafeCancellationToken cancellation, IRouteArgs fragmentContext) { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnStart(UnsafeCancellationToken cancellation) { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnTransition(UnsafeCancellationToken cancellationToken) { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnEnable(UnsafeCancellationToken cancellation) { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnDisable(UnsafeCancellationToken cancellation) { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnStop(UnsafeCancellationToken cancellation) { return PromiseTask.CompletedTask; }
        protected virtual void OnDestroy() { }
  #endregion
  #endregion

        #region Private Methods
        // https://gist.github.com/liuxinjia/09b12b5f582faeff0a88ad32b56e31c7
        // no gc
        private async PromiseTask Traversal(
            bool isAdd,
            Func<Node, bool> isValidFunc,
            Func<Node, UnsafeCancellationToken, PromiseTask> action)
        {
            bool isParallel = Key.ParallelLoad;
            UnsafeCancellationToken cancellationToken = isAdd ? AddCancellation.Token : RemoveCancellation.Token;

            try
            {
                if (isAdd)
                {
                    if (isParallel)
                    {
                        await PreOrderParallelTraversal(cancellationToken, isValidFunc, action);
                    }
                    else
                    {
                        await PreOrderTraversal(cancellationToken, isValidFunc, action);
                    }
                }
                else
                {
                    if (isParallel)
                    {
                        await PostOrderParallelTraversal(cancellationToken, isValidFunc, action);
                    }
                    else
                    {
                        await PostOrderTraversal(cancellationToken, isValidFunc, action);
                    }
                }
            }
            catch (Exception e)
            {
                if (isAdd)
                {
                    AddFail(e);
                }
                else
                {
                    RemoveFail(e);
                }
                throw;
            }
        }


        private async PromiseTask PostOrderParallelTraversal(UnsafeCancellationToken cancellation,
            Func<Node, bool> isValidFunc,
            Func<Node, UnsafeCancellationToken, PromiseTask> action)
        {
            if (cancellation.IsCancellationRequested)
            {
                return;
            }

            var poolBinder = Context.GetInstance<IPoolBinder>();
            var tasks = poolBinder.AutoCreate<List<PromiseTask>>();
            PostOrder(this, cancellation, action, isValidFunc, tasks);
            await PromiseTask.WhenAll(tasks);
            poolBinder.Return(tasks);

            void PostOrder(Node node, UnsafeCancellationToken unsafeCancellationToken,
                Func<Node, UnsafeCancellationToken, PromiseTask> iterateAction,
                Func<Node, bool> validFunc, List<PromiseTask> promiseTasks)
            {
                // litter difference : we want to iterate from right child since we iterate from left child when preorder
                for (int i = node.ChildCount - 1; i >= 0; i--)
                {
                    var child = node.GetChild(i) as Node;
                    bool isValid = validFunc.Invoke(child);
                    if (isValid)
                    {
                        PostOrder(child, unsafeCancellationToken,
                            iterateAction, validFunc, tasks);
                    }
                }

                promiseTasks.Add(iterateAction.Invoke(node, unsafeCancellationToken));
            }
        }

        private async PromiseTask PreOrderParallelTraversal(UnsafeCancellationToken cancellation,
            Func<Node, bool> isValidFunc,
            Func<Node, UnsafeCancellationToken, PromiseTask> action)
        {
            if (cancellation.IsCancellationRequested)
            {
                return;
            }

            var poolBinder = Context.GetInstance<IPoolBinder>();
            var tasks = poolBinder.AutoCreate<List<PromiseTask>>();
            PreOrder(this, cancellation, action, isValidFunc, tasks);
            await PromiseTask.WhenAll(tasks);
            poolBinder.Return(tasks);

            void PreOrder(Node node, UnsafeCancellationToken unsafeCancellationToken,
                Func<Node, UnsafeCancellationToken, PromiseTask> iterateAction,
                Func<Node, bool> validFunc, List<PromiseTask> promiseTasks)
            {
                promiseTasks.Add(iterateAction.Invoke(node, unsafeCancellationToken));

                for (int i = 0; i < node.ChildCount; i++)
                {
                    var child = node.GetChild(i) as Node;
                    bool isValid = validFunc.Invoke(child);
                    if (isValid)
                    {
                        PreOrder(child, unsafeCancellationToken,
                            iterateAction, validFunc, tasks);
                    }
                }
            }
        }


        private async PromiseTask PostOrderTraversal(UnsafeCancellationToken cancellation,
            Func<Node, bool> isValidFunc,
            Func<Node, UnsafeCancellationToken, PromiseTask> action)
        {
            if (cancellation.IsCancellationRequested)
            {
                return;
            }

            await PostOrder(this, cancellation, action, isValidFunc);

            async PromiseTask PostOrder(Node node, UnsafeCancellationToken unsafeCancellationToken,
                Func<Node, UnsafeCancellationToken, PromiseTask> iterateAction,
                Func<Node, bool> validFunc)
            {
                // litter difference : we want to iterate from right child since we iterate from left child when preorder
                for (int i = node.ChildCount - 1; i >= 0; i--)
                {
                    var child = node.GetChild(i)as Node;
                    bool isValid = validFunc.Invoke(child);
                    if (isValid)
                    {
                        await PostOrder(child, unsafeCancellationToken,
                            iterateAction, validFunc);
                    }
                }
                await iterateAction.Invoke(node, unsafeCancellationToken);
            }
        }

        private async PromiseTask PreOrderTraversal(UnsafeCancellationToken cancellation,
            Func<Node, bool> isValidFunc,
            Func<Node, UnsafeCancellationToken, PromiseTask> action)
        {
            if (cancellation.IsCancellationRequested)
            {
                return;
            }

            await PreOrder(this, cancellation, action, isValidFunc);

            async PromiseTask PreOrder(Node node, UnsafeCancellationToken unsafeCancellationToken,
                Func<Node, UnsafeCancellationToken, PromiseTask> iterateAction,
                Func<Node, bool> validFunc)
            {
                await iterateAction.Invoke(node, unsafeCancellationToken);

                // litter difference : we want to iterate from right child since we iterate from left child when preorder
                for (int i = node.ChildCount - 1; i >= 0; i--)
                {
                    var child = node.GetChild(i)as Node;
                    bool isValid = validFunc.Invoke(child);
                    if (isValid)
                    {
                        await PreOrder(child, unsafeCancellationToken,
                            iterateAction, validFunc);
                    }
                }
            }
        }

        private void AddFail(Exception ex)
        {
            AssertUtil.NotNull(AddStatus);

            if (AddStatus.CurState == PromiseState.Pending
                && ex != null)
            {
                AddStatus.RejectWithoutDebug(ex);
            }

            TryReturnPromise(AddStatus);
            AddStatus = null;

            MarkNodeState(NodeState.Failed, AddCancellation.Token);
        }

        private void RemoveFail(Exception ex)
        {
            AssertUtil.NotNull(RemoveStatus);

            if (RemoveStatus.CurState == PromiseState.Pending
                && ex != null)
            {
                RemoveStatus.RejectWithoutDebug(ex);
            }

            TryReturnPromise(RemoveStatus);
            RemoveStatus = null;

            MarkNodeState(NodeState.Failed, AddCancellation.Token);
        }

        private void TryReturnPromise(IPromise promise)
        {
            promise.TryReturn(false);
        }

        private void MarkNodeState(NodeState targetState,
            UnsafeCancellationToken cancellation, IRouteArgs fragmentContext = null)
        {
            NodeState = targetState;
        }

        protected sealed override bool IsCloseStage()
        {
            bool close = true;
            switch (NodeState)
            {
                case NodeState.Adding:
                case NodeState.Ready:
                    close = false;
                    break;
            }
            return close;
        }
#endregion

        #region Node Base
        // don't await cancel even the action is not end
        // in case of waiting loop
        public void CancelAddNode()
        {
            if (NodeState == NodeState.Ready)
            {
                Console.Warn("try to cancel an not adding node: {NodeState}", NodeState);
                return;
            }
            if (AddCancellation.Token.IsCancellationRequested)
            {
                Console.Warn($"try to cancel an already cancel load task: {NodeState}", NodeState);
                return;
            }
            AddCancellation.Cancel();
        }

        public void CancelUnLoadNode()
        {
            if (NodeState != NodeState.Removing || NodeState != NodeState.Unloading)
            {
                Console.Warn($"try to cancel an not adding node: {NodeState}", NodeState);
                return;
            }
            if (RemoveCancellation.Token.IsCancellationRequested)
            {
                Console.Warn($"try to cancel an already cancel unload task: {NodeState}", NodeState);
                return;
            }

            RemoveCancellation.Cancel();
        }

        public void Dispose()
        {
            // NodeState = NodeState.Default;
            AssertUtil.IsTrue(_childNodes == null || _childNodes.Count <= 0);
            AssertUtil.IsFalse(IsInit, NodeTreeExceptionType.dispose_not_int);
            AssertUtil.IsNull(_addStatus);
            AssertUtil.IsNull(_removeStatus);
            OnDispose();
        }

        protected virtual void OnDispose() { }

        public void AddChild(INode child)
        {
            var childContext = ((Node)child)._context;
            if (childContext != null)
            {
                Context.AddContext(childContext);
            }
            this._childNodes.Add(child);
            child.Parent = this;
        }

        public void RemoveChild(INode child)
        {
            child.Parent = null;
            this._childNodes.Remove(child);

            var childContext = ((Node)child)._context;
            if (Context != null
                && childContext != null)
            {
                Context.RemoveContext(childContext);
            }
        }

        public INode GetChild(int index)
        {
            AssertUtil.Greater(index, -1);
            AssertUtil.Greater(ChildNodes.Count, 0);

            return ChildNodes[index];
        }
        public INode GetChild(IAssetKey assetKey)
        {
            foreach (var child in ChildNodes)
            {
                if (Equals(child.Key, assetKey))
                {
                    return child;
                }
            }
            return null;
        }

          #region Inject Config
        public void Inject(IRouteArgs fragmentContext)
        {
            if (IsInjected)
                return;

            IsInjected = true;

            _context = OnAssignContext();
            Context.AddComponents(this, fragmentContext);
            Context.Inject(this);

            OnInject();
        }

        public void Deject()
        {
            if (!IsInjected)
                return;
            IsInjected = false;

            OnDeject();
            Context.Deject(this);
            Context.RemoveComponents();
        }
        /// <summary>
        /// will be called after inject, 
        /// do something lik inject other elements
        /// </summary>
        protected void OnInject()
        {

        }
        /// <summary>
        /// will be called before Deject, 
        /// do something lik deject other elements
        /// </summary>
        protected void OnDeject()
        {

        }
        private INodeContext OnAssignContext()
        {
            var context = Key.CreateContext();
            return context;
        }
        #endregion
#endregion


        public override string ToString()
        {
            if (Key == null) return string.Empty;
            return Key.ToString();
        }

    }
}
