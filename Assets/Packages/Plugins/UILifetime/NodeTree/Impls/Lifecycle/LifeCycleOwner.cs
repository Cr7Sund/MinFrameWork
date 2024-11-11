using System.Collections.Generic;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.LifeTime;
namespace Cr7Sund.LifeTime
{
    public abstract class LifeCycleOwner : ILifeCycleOwner
    {
        private LifeCycleState _lifeCycleState;
        private LifeCycleOwner _parent;
        private readonly List<ILifeCycleAwareComponent> _lifeCycles = new();
        private UnsafeCancellationTokenSource _addCancellation;
        private UnsafeCancellationTokenSource _removeCancellation;

        public UnsafeCancellationTokenSource AddCancellation
        {
            get
            {
                return _addCancellation ??= GetNewCancellation();
            }
        }
        public UnsafeCancellationTokenSource RemoveCancellation
        {
            get
            {
                return _removeCancellation ??= GetNewCancellation();
            }
        }

        public LifeCycleState lifeCycleState => _lifeCycleState;


        public async PromiseTask AddLifecycle(ILifeCycleAwareComponent lifeCycle, IRouteArgs fragmentContext)
        {
            bool close = IsCloseStage();
            AssertUtil.IsFalse(close);
            AssertUtil.IsFalse(_lifeCycles.Contains(lifeCycle));

            if (_lifeCycleState == LifeCycleState.Initialized)
            {
            }
            if (_lifeCycleState <= LifeCycleState.Created)
            {
                await lifeCycle.OnCreate(this, AddCancellation.Token, fragmentContext);
            }
            if (_lifeCycleState <= LifeCycleState.Started)
            {
                await lifeCycle.OnStart(this, AddCancellation.Token);
            }
            if (_lifeCycleState <= LifeCycleState.Resumed)
            {
                await lifeCycle.OnResume(this, AddCancellation.Token);
            }

            _lifeCycles.Add(lifeCycle);
        }

        public async PromiseTask RemoveLifecycle(ILifeCycleAwareComponent lifeCycle)
        {
            bool close = IsCloseStage();
            AssertUtil.IsTrue(close);
            AssertUtil.IsTrue(_lifeCycles.Contains(lifeCycle));

            if (_lifeCycleState == LifeCycleState.Initialized || _lifeCycleState == LifeCycleState.Destroyed)
            {
            }

            if (_lifeCycleState >= LifeCycleState.Resumed)
            {
                await lifeCycle.OnPause(this, AddCancellation.Token);
            }
            if (_lifeCycleState >= LifeCycleState.Started)
            {
                await lifeCycle.OnStop(this, AddCancellation.Token);
            }
            if (_lifeCycleState >= LifeCycleState.Created)
            {
                lifeCycle.OnDestroy(this);
            }

            _lifeCycles.Remove(lifeCycle);
        }

        public virtual void Destroy()
        {
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
            foreach (var item in _lifeCycles)
            {
                item.OnDestroy(this);
            }
            _lifeCycleState = LifeCycleState.Destroyed;
        }

        public async PromiseTask MarkState(LifeCycleState targetState, UnsafeCancellationToken cancellation, IRouteArgs fragmentContext = null)
        {
            _lifeCycleState = targetState;

            // Assert
            bool close = IsCloseStage();
            await Do(cancellation, close, fragmentContext);

            async PromiseTask Do(UnsafeCancellationToken cancellationToken, bool isClose = false, IRouteArgs intent = null)
            {
                switch (_lifeCycleState)
                {
                    case LifeCycleState.Initialized:
                        AssertUtil.NotNull(cancellationToken);
                        foreach (var item in _lifeCycles)
                        {
                            await item.OnCreate(this, cancellationToken, intent);
                        }
                        break;
                    case LifeCycleState.Started:
                        foreach (var item in _lifeCycles)
                        {
                            if (!isClose)
                            {
                                await item.OnStart(this, cancellationToken);
                            }
                            else
                            {
                                await item.OnStop(this, cancellationToken);
                            }
                        }
                        break;
                    case LifeCycleState.Resumed:
                        foreach (var item in _lifeCycles)
                        {
                            if (!isClose)
                            {
                                await item.OnResume(this, cancellationToken);
                            }
                            else
                            {
                                await item.OnPause(this, cancellationToken);
                            }
                        }
                        break;
                    case LifeCycleState.Destroyed:
                        break;
                }
            }
        }

        protected abstract bool IsCloseStage();

        private UnsafeCancellationTokenSource GetNewCancellation()
        {
            // var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
            // var cancellationPool = poolBinder.GetOrCreate<UnsafeCancellationTokenSource>();
            // return cancellationPool.GetInstance();
            return UnsafeCancellationTokenSource.Create();
        }

        private void ReturnCancellation(UnsafeCancellationTokenSource cancellation)
        {
            if (cancellation is { IsCancelling: false })
                // var poolBinder = _context.InjectionBinder.GetInstance<IPoolBinder>();
                // var cancellationPool = poolBinder.GetOrCreate<UnsafeCancellationTokenSource>();
                // cancellationToken.Dispose();
                // cancellationPool.ReturnInstance(cancellationToken);
            {
                cancellation.TryReturn();
            }

        }
    }

}
