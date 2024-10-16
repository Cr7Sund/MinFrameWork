using System.Collections.Generic;
using Cr7Sund.FrameWork.Util;
namespace Cr7Sund.LifeCycle
{
    public interface ILifeCycleOwner
    {
        LifeCycleState State { get; }
    }

    public class LifeCycleOwner : ILifeCycleOwner
    {
        protected LifeCycleState _state;
        private LifeCycleOwner _parent;
        private List<ILifeCycle> _lifeCycles = new();

        public LifeCycleState State => _state;

        private async PromiseTask AddLifecycle(ILifeCycle lifeCycle, UnsafeCancellationToken cancellationToken)
        {
            if (_state == LifeCycleState.Initialized)
            {
            }
            if (_state == LifeCycleState.Created)
            {
                await lifeCycle.OnCreateAsync(this,cancellationToken);
            }
            if (_state == LifeCycleState.Started)
            {
                lifeCycle.OnStart(this);
            }

            if (_state == LifeCycleState.Resumed)
            {
                lifeCycle.OnResume(this);
            }
        }

        protected async PromiseTask MarkState(LifeCycleState state, bool close = false,UnsafeCancellationTokenSource cancellation = null)
        {
            // Assert
            _state = state;
            await Do(close,cancellation);
        }

        private async PromiseTask Do(bool close, UnsafeCancellationTokenSource cancellation = null)
        {
            switch (_state)
            {
                case LifeCycleState.Initialized:
                    AssertUtil.NotNull(cancellation);
                    foreach (var item in _lifeCycles)
                    {
                        await item.OnCreateAsync(this, cancellation.Token);
                    }
                    break;
                case LifeCycleState.Started:
                    foreach (var item in _lifeCycles)
                    {
                        if (!close)
                        {
                            item.OnStart(this);
                        }
                        else
                        {
                            item.OnStop(this);
                        }
                    }
                    break;
                case LifeCycleState.Resumed:
                    foreach (var item in _lifeCycles)
                    {
                        if (!close)
                        {
                            item.OnResume(this);
                        }
                        else
                        {
                            item.OnPause(this);
                        }
                    }
                    break;
                case LifeCycleState.Destroyed:
                    foreach (var item in _lifeCycles)
                    {
                        item.OnDestroy(this);
                    }
                    break;
            }
        }
    }

}
