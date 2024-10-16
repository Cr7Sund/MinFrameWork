using System;
using Cr7Sund.AssetContainers;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.IocContainer;
using Cr7Sund.LifeCycle;
using UnityEngine;

namespace Cr7Sund.UILifeTime
{
    public class FragmentContext : CrossContext
    {

    }

    public abstract class Fragment : LifeCycleOwner
    {
        // need config activity
        private readonly ICrossContext _context;

        private Activity _hostActivity;
        private Fragment _parentFragment;

        private UnsafeCancellationTokenSource _showCancellationTokenSource;
        private UnsafeCancellationTokenSource _hideCancellationTokenSource;
        // [Inject(ServerBindDefine.UILogger)] protected IInternalLog _log;
        [Inject(UIConfigDefines.GameInstancePool)] private IInstancesContainer _gameContainer;

        private FragmentManager _fragmentManager;
        public FragmentManager FragmentManager
        {
            get
            {
                return _fragmentManager ?? (_fragmentManager = new FragmentManager());
            }
        }
        public Guid ID { get; private set; }
        public Activity HostActivity
        {
            get
            {
                return _hostActivity;
            }
        }

        public IUIView UIView
        {
            get;
            private set;
        }

        private IFragmentTransition _transition
        {
            get
            {
                throw new NotImplementedException();
                // return UIView.IsValid() ? UIView : null;
            }
        }

        public Fragment()
        {
            _context = new FragmentContext();
        }

        public void Init(Guid guid, UIView uiView, Activity hostActivity, Fragment parentFragment)
        {
            ID = guid;
            UIView = uiView;
            _parentFragment = parentFragment;
            _hostActivity = hostActivity;
            // PLAN support nested Navigation
            _hostActivity.Context.AddContext(_context);

            _context.Inject(this);
            _context.Inject(UIView);
        }

        #region  Fragment Manager
        public async PromiseTask Attach(object sendIntent)
        {
            ThrowIfCancel(_showCancellationTokenSource);
            if (_state >= LifeCycleState.Initialized)
            {
                AssertUtil.IsFalse(_state == LifeCycleState.Destroyed);
                return;
            }

            await MarkState(LifeCycleState.Initialized, false, _showCancellationTokenSource);
            OnAttach(sendIntent);
            await OnCreate(_showCancellationTokenSource.Token);
        }

        public async PromiseTask Load()
        {
            ThrowIfCancel(_showCancellationTokenSource);
            if (_state >= LifeCycleState.Created)
            {
                return;
            }

            RectTransform findAttachPoint = GetAttachPoint();

            await UIView.Load(
                ID.ToString(),
                _showCancellationTokenSource.Token,
                findAttachPoint);

            OnCreateView();

            await MarkState(LifeCycleState.Created);
        }

        private RectTransform GetAttachPoint()
        {
            if (_parentFragment != null)
            {
                return _parentFragment.UIView.FindAttachPoint(ID.ToString());
            }
            else
            {
                return _gameContainer.GetInstance(
                    UIConfigDefines.UIRootAssetKey, UIConfigDefines.UI_ROOT_NAME).transform as RectTransform;
            }
        }

        public async PromiseTask Visible()
        {
            ThrowIfCancel(_showCancellationTokenSource);
            if (_state == LifeCycleState.Started)
            {
                return;
            }

            OnViewCreated();
            OnViewBind();
            OnStart();
            await MarkState(LifeCycleState.Started);
        }

        public async PromiseTask Show(string partnerPage, bool push)
        {
            ThrowIfCancel(_showCancellationTokenSource);

            if (_state == LifeCycleState.Resumed)
            {
                return;
            }

            var transition = _transition;
            if (push)
            {
                await transition.push_enter(partnerPage, _showCancellationTokenSource.Token);
            }
            else
            {
                await transition.pop_enter(partnerPage, _showCancellationTokenSource.Token);
            }
            OnResume();
            await MarkState(LifeCycleState.Resumed);
        }

        public async PromiseTask Invisible()
        {
            ThrowIfCancel(_hideCancellationTokenSource);
            if (_state == LifeCycleState.Started)
            {
                return;
            }

            OnPause();
            await MarkState(LifeCycleState.Started, true);
        }

        public async PromiseTask Hide(string partnerPage, bool push, bool skipAnimation = false)
        {
            ThrowIfCancel(_hideCancellationTokenSource);
            if (_state == LifeCycleState.Created)
            {
                return;
            }

            if (skipAnimation)
            {
                if (push)
                {
                    await _transition.pop_exit(partnerPage, _hideCancellationTokenSource.Token);
                }
                else
                {
                    await _transition.push_exit(partnerPage, _hideCancellationTokenSource.Token);
                }
            }
            OnStop();
            OnViewUnBind();
            await MarkState(LifeCycleState.Created, true);
        }

        public async PromiseTask Destroy(bool push)
        {
            ThrowIfCancel(_hideCancellationTokenSource);

            if (_state > LifeCycleState.Created)
            {
                await Hide(string.Empty, push, true);
            }
            _hostActivity = null;

            if (_state == LifeCycleState.Destroyed)
            {
                return;
            }
            OnDestroyView();
            OnDestroy();
            OnDetach();
            await MarkState(LifeCycleState.Destroyed);
        }

        public void CancelRemove()
        {
            if (_hideCancellationTokenSource != null &&
                !_hideCancellationTokenSource.IsCancellationRequested)
            {
                _hideCancellationTokenSource.Cancel();
            }
        }

        public void CancelAdd()
        {
            if (_showCancellationTokenSource != null &&
                !_showCancellationTokenSource.IsCancellationRequested)
            {
                _showCancellationTokenSource.Cancel();
            }
        }

        public void StartAdd()
        {
            //TODO handle already start task
            AssertUtil.IsNull(_showCancellationTokenSource);

            _showCancellationTokenSource = UnsafeCancellationTokenSource.Create();
        }

        public void EndAdd()
        {
            _showCancellationTokenSource.TryReturn();
            _showCancellationTokenSource = null;
        }

        public void StartRemove()
        {
            AssertUtil.IsNull(_hideCancellationTokenSource);

            _hideCancellationTokenSource = UnsafeCancellationTokenSource.Create();
        }

        public void EndRemove()
        {
            _hideCancellationTokenSource.TryReturn();
            _hideCancellationTokenSource = null;
        }

        #endregion

        #region  Lifecycle
        protected virtual void OnAttach(object context)
        {
        }
        protected virtual PromiseTask OnCreate(UnsafeCancellationToken cancellationToken)
        {
            return PromiseTask.CompletedTask;
        }
        protected virtual void OnCreateView()
        {
        }
        protected virtual void OnViewCreated()
        {

        }
        protected virtual void OnViewBind()
        {

        }

        protected virtual void OnStart()
        {

        }
        protected virtual void OnResume()
        {

        }
        protected virtual void OnPause()
        {

        }
        protected virtual void OnStop()
        {

        }

        protected virtual void OnViewUnBind()
        {
        }

        protected virtual void OnDestroyView()
        {

        }
        protected virtual void OnDestroy()
        {

        }
        protected virtual void OnDetach()
        {

        }

        #endregion

        private void ThrowIfCancel(UnsafeCancellationTokenSource cancellationTokenSource)
        {
            if (cancellationTokenSource.IsCancelling)
            {
                // throw new PromiseTaskCancelException(cancellationTokenSource);
                throw new OperationCanceledException();
            }
        }

    }

}
