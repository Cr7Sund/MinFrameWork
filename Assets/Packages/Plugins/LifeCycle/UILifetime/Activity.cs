using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cr7Sund.AssetContainers;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.IocContainer;
using Cr7Sund.LifeCycle;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.UI.Api;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.UILifeTime
{
    public static class UIDefine
    {
        public static string ActivityNavigation = "ActivityNavigation";
    }

    public interface IUIContext : ICrossContext
    {
        void AddComponents();
        void RemoveComponents();
    }

    public class RootActivityContext : CrossContext, IUIContext
    {
        public RootActivityContext()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }

        public void AddComponents()
        {
            var assetLoader = AssetLoaderFactory.CreateLoader();
            BindInstanceAsCrossContext<IAssetLoader>(assetLoader);
            BindAsCrossAndSingleton<IInstancesContainer, GameInstanceContainer>(UIConfigDefines.GameInstancePool);
            BindAsCrossAndSingleton<Navigation.Navigation, Navigation.Navigation>();
        }

        public void RemoveComponents()
        {
            Unbind<IAssetLoader>();
            Unbind<Navigation.Navigation>();
            Unbind<IInstancesContainer>(UIConfigDefines.GameInstancePool);
        }
    }

    public class ActivityContext : CrossContext, IUIContext
    {
        public void AddComponents()
        {
            BindAsCrossAndSingleton<IUITransitionAnimationContainer, UITransitionAnimationContainer>();
            BindAsCrossAndSingleton<IInstancesContainer, UIPanelContainer>(UIConfigDefines.UIPanelContainer);
            BindAsCrossAndSingleton<IPromiseTimer, PromiseTimer>();
        }

        public void RemoveComponents()
        {
            Unbind<IUITransitionAnimationContainer>();
            Unbind<IInstancesContainer>(UIConfigDefines.UIPanelContainer);
            Unbind<IPromiseTimer>();
        }
    }

    public class RootActivity : Activity
    {
        [Inject(UIConfigDefines.GameInstancePool)] private IInstancesContainer _gameContainer;

        protected override IUIContext CreateContext()
        {
            return new RootActivityContext();
        }

        protected override async PromiseTask OnCreateAsync(UnsafeCancellationToken cancellationToken)
        {
            await _gameContainer.InstantiateAsync<Object>(UIConfigDefines.UIRootAssetKey, UIConfigDefines.UI_ROOT_NAME, cancellationToken);
        }
    }

    public static class TaskStackBuilder
    {
        private static Stack<Activity> _activities = new();
        private static List<Activity> _longLiveList = new();
        private static List<Activity> _appendList = new();
        private static Activity _rootActivity;

        public static T StartActivity<T>(Activity rootActivity, object intent = null) where T : Activity, new()
        {
            var activity = Activator.CreateInstance<T>();
            if (activity.saveStack)
            {
                _activities.Push(activity);
            }

            activity.Init(rootActivity, intent);

            return activity;
        }

        public static void CloseActivity()
        {
            var topActivity = _activities.Pop();
            if (!topActivity.longLive)
            {
                _appendList.Add(topActivity);
            }
        }

        public static async PromiseTask LaunchActivity()
        {
            var activity = StartActivity<RootActivity>(null);
            _rootActivity = activity;

            await activity.PreShow();
            await activity.PostShow();
        }

        public static async PromiseTask PopActivity()
        {
            // let fragment handle transition by themselves via Navigation

            var preActivity = _activities.Pop();
            while (!preActivity.saveStack)
            {
                preActivity = _activities.Pop();
            }
            var destActivity = _activities.Peek();

            await preActivity.PreHide();
            await destActivity.PreShow();
            await preActivity.SwitchNavCtrl(destActivity.ID, false);
            await destActivity.PostShow();
            await preActivity.PostHide();
        }

        public static async PromiseTask<T> PushActivity<T>() where T : Activity, new()
        {
            AssertUtil.Greater(_activities.Count, 0);
            // let fragment handle transition by themselves via Navigation
            var preActivity = _activities.Peek();
            var destActivity = StartActivity<T>(preActivity);

            await preActivity.PreHide();
            await destActivity.PreShow();
            await preActivity.SwitchNavCtrl(destActivity.ID, true);
            await destActivity.PostShow();
            await preActivity.PostHide();

            return destActivity;
        }
    }

    public abstract class Activity : LifeCycleOwner
    {
        public bool longLive;
        public bool saveStack = true;

        private readonly Guid _activityId;
        private IUIContext _context;
        private UnsafeCancellationTokenSource _showCancellation;
        private UnsafeCancellationTokenSource _hideCancellation;

        [Inject]
        protected Navigation.Navigation _navigation;
        private FragmentManager _fragmentManager;
        public FragmentManager FragmentManager
        {
            get
            {
                return _fragmentManager ?? (_fragmentManager = new FragmentManager());
            }
        }
        public Guid ID
        {
            get => _activityId;
        }
        public ICrossContext Context
        {
            get
            {
                return _context;
            }
        }


        public Activity()
        {
            _activityId = Guid.NewGuid();
        }

        public async PromiseTask CloseActivity()
        {
            await PreHide();
            await PostHide();
            _context.RemoveComponents();
            await MarkState(LifeCycleState.Destroyed);
        }

        internal void Init(Activity rootActivity, object intent)
        {
            _context = CreateContext();
            if (rootActivity != null)
            {
                rootActivity._context.AddContext(_context);
            }
            _context.AddComponents();

            _context.Inject(this);

            _navigation.CreateNavController(this);
        }


        protected virtual IUIContext CreateContext()
        {
            return new ActivityContext();
        }

        #region LifeCycle

        protected virtual PromiseTask OnCreateAsync(UnsafeCancellationToken cancellationToken)
        {
            return PromiseTask.CompletedTask;
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
        protected virtual void OnDestroy()
        {

        }
        #endregion

        internal async PromiseTask PreHide()
        {
            _hideCancellation = UnsafeCancellationTokenSource.Create();

            try
            {
                if (_state > LifeCycleState.Started)
                {
                    OnPause();
                    await MarkState(LifeCycleState.Started);
                }
            }
            catch
            {
                // ignored
            }
        }

        internal async PromiseTask PostHide()
        {
            try
            {
                if (_state > LifeCycleState.Created)
                {
                    OnStop();
                    await MarkState(LifeCycleState.Created);
                }
            }
            finally
            {
                if (!_hideCancellation.IsRecycled)
                {
                    _hideCancellation.TryReturn();
                }
            }
        }

        internal async PromiseTask PreShow()
        {
            _showCancellation = UnsafeCancellationTokenSource.Create();

            try
            {
                if (_state < LifeCycleState.Initialized)
                {
                    await MarkState(LifeCycleState.Initialized, false, _showCancellation);
                }
                if (_state < LifeCycleState.Created)
                {
                    await OnCreateAsync(_showCancellation.Token);
                    await MarkState(LifeCycleState.Created);
                }
                if (_state < LifeCycleState.Started)
                {
                    OnStart();
                    await MarkState(LifeCycleState.Started);
                }
            }
            catch
            {
                // ignored
            }
        }

        internal async PromiseTask PostShow()
        {
            try
            {
                if (_state < LifeCycleState.Resumed)
                {
                    OnResume();
                    await MarkState(LifeCycleState.Resumed);
                }
            }
            finally
            {
                if (!_showCancellation.IsRecycled)
                {
                    _showCancellation.TryReturn();
                }
            }
        }

        internal async PromiseTask SwitchNavCtrl(Guid destActivityId, bool push)
        {
            var relativeNavController = _navigation.FindNavController(destActivityId);
            var navController = _navigation.FindNavController(_activityId);
            await navController.SwitchNavCtrl(relativeNavController, push);
        }

    }

}
