using System;
using System.Collections.Generic;
using System.Threading;
using Elastic.CommonSchema;

namespace Cr7Sund
{
    public struct UnsafeCancellationToken
    {
        public static UnsafeCancellationToken None => default;

        private UnsafeCancellationTokenSource _source;
        private int _version;

        public bool IsCancellationRequested
        {
            get
            {
                return Validate() ? _source.IsCancellationRequested : false;
            }
        }
        // if change to CancellationToken
        // skip it
        public bool IsValid => _source != null && _version == _source.Version;


        public UnsafeCancellationToken(UnsafeCancellationTokenSource source, int version)
        {
            _source = source;
            _version = version;
        }

        public void Register(Action action)
        {
            if (_source == null)
            {
                throw new System.Exception("try to use default token");
            }

            if (Validate())
            {
                _source.Register(action);
            }
        }

        private bool Validate()
        {
            if (_source == null)
            {
                return false;
            }

            if (_source.Version != _version)
            {
                throw new System.Exception("try to use old version token");
            }

            return true;
        }
    }

    public class UnsafeCancellationTokenSource : IDisposable
    {
        private enum State
        {
            NotCanceledState,
            NotifyingState,
            NotifyingCompleteState,
        }

        private List<Action> _registrations;
        private int _version;
        private State _state;

        internal bool IsCancellationRequested => _state != State.NotCanceledState;
        internal int Version => _version;
        public UnsafeCancellationToken Token
        {
            get
            {
                return new UnsafeCancellationToken(this, _version);
            }
        }

        public void Cancel()
        {
            ThrowIfCancel();
            ExecuteCallbackHandlers();
        }

        public void Register(Action action)
        {
            ThrowIfCancel();

            if (_registrations == null)
            {
                _registrations = new();
            }
            _registrations.Add(action);
        }

        public void Dispose()
        {
            // reset
            if (_state == State.NotifyingState) return;

            _registrations?.Clear();
            _state = State.NotCanceledState;
            _version++;
        }

        public CancellationTokenSource Join()
        {
            var cancellation = new CancellationTokenSource();

            Token.Register(cancellation.Cancel);
            return cancellation;
        }

        private void ExecuteCallbackHandlers()
        {
            _state = State.NotifyingState;

            if (_registrations != null)
            {
                foreach (var callback in _registrations)
                {
                    try
                    {
                        callback.Invoke();
                    }
                    catch (System.Exception ex)
                    {
                        Console.Error(ex);
                    }
                }
 
            }
            _state = State.NotifyingCompleteState;
        }

        private void ThrowIfCancel()
        {
            if (IsCancellationRequested)
            {
                throw new System.Exception("token is already canceled or cancelling )");
            }
        }
    }
}