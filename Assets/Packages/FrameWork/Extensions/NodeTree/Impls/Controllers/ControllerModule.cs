using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using System.Threading;

namespace Cr7Sund.NodeTree.Impl
{
    public class ControllerModule : AsyncLoadable, IControllerModule
    {
        protected IContext _context;
        protected List<IController> _childControllers;
        protected List<IUpdatable> _childUpdates;
        protected List<ILateUpdate> _childLateUpdates;

        public bool IsInjected
        {
            get;
            private set;
        }
        public bool IsActive
        {
            get;
            set;
        }
        public bool IsStarted
        {
            get;
            set;
        }


        public ControllerModule()
        {
            _childControllers = new List<IController>();
            _childLateUpdates = new List<ILateUpdate>();
            _childUpdates = new List<IUpdatable>();
        }


        #region IControllerModule Implementation
        public async PromiseTask AddController<T>(UnsafeCancellationToken cancellation = default) where T : IController
        {
            CheckBusiness<T>();
            await AddController(Activator.CreateInstance<T>(), cancellation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="cancellation">it can cancel load and unload</param>
        /// <returns></returns>
        public async PromiseTask AddController(IController controller, UnsafeCancellationToken cancellation)
        {
            AssertUtil.NotNull(controller, NodeTreeExceptionType.EMPTY_CONTROLLER_ADD);
            CheckController(controller);

            if (IsStarted)
            {
                _context.InjectionBinder.Injector.Inject(controller);

                if (controller is AsyncLoadable load)
                {
                    try
                    {
                        await load.LoadAsync(cancellation);
                    }
                    catch
                    {
                        await load.UnloadAsync(cancellation);
                        throw;
                    }
                }

                try
                {
                    await controller.Start(cancellation);
                }
                catch
                {
                    await controller.Stop();
                    throw;
                }

                try
                {
                    await controller.Enable();
                }
                catch
                {
                    await controller.Disable(false);
                    throw;
                }

            }

            _childControllers.Add(controller);
            AddChild(controller);
        }

        public async PromiseTask RemoveController<T>() where T : IController
        {
            for (int i = 0, length = _childControllers.Count; i < length; i++)
            {
                var controller = _childControllers[i];
                if (controller.GetType() == typeof(T))
                {
                    await RemoveController(controller, UnsafeCancellationToken.None);
                }
            }
        }

        public async PromiseTask RemoveController(IController controller, UnsafeCancellationToken cancellation)
        {
            AssertUtil.NotNull(controller, NodeTreeExceptionType.EMPTY_CONTROLLER_REMOVE);
            CheckController(controller);

            if (controller.IsActive)
            {
                await controller.Disable(false);
            }

            if (controller.IsStarted)
            {
                await controller.Stop();
            }

            if (controller is AsyncLoadable load)
            {
                await load.UnloadAsync(cancellation);
            }

            _context.InjectionBinder.Injector.Deject(controller);
            RemoveChild(controller);
            _childControllers.Remove(controller);
        }

        [Conditional(MacroDefine.UNITY_EDITOR)]
        private void CheckBusiness<T>() where T : IController
        {
            CheckType(typeof(T));
        }


        [Conditional(MacroDefine.UNITY_EDITOR)]
        private void CheckController(IController controller)
        {
            CheckType(controller.GetType());
        }

        private void CheckType(Type type)
        {
            for (int i = 0; i < _childControllers.Count; i++)
            {
                if (_childControllers[i].GetType() == type)
                {
                    throw new MyException($"Duplicate controller have been added : {type}");
                }
            }
        }

        private void AddChild(IController controller)
        {
            if (controller == null)
            {
                return;
            }
            if (controller is IUpdatable update)
            {
                if (!_childUpdates.Contains(update))
                    _childUpdates.Add(update);
            }

            if (controller is ILateUpdate lateUpdate)
            {
                if (!_childLateUpdates.Contains(lateUpdate))
                    _childLateUpdates.Add(lateUpdate);
            }
        }

        private void RemoveChild(IController controller)
        {
            if (controller == null)
            {
                return;
            }
            if (controller is IUpdatable update)
            {
                if (_childUpdates.Contains(update))
                    _childUpdates.Remove(update);
            }

            if (controller is ILateUpdate lateUpdate)
            {
                if (_childLateUpdates.Contains(lateUpdate))
                    _childLateUpdates.Remove(lateUpdate);
            }
        }

        #endregion

        #region LifeCycles
        public async PromiseTask Start(UnsafeCancellationToken cancellation)
        {
            if (IsStarted) return;

            IsStarted = true;
            int count = _childControllers.Count;
            for (int i = 0; i < count; i++)
            {
                await _childControllers[i].Start(cancellation);
            }
        }
        public async PromiseTask Enable()
        {
            if (!IsStarted || IsActive) return;

            IsActive = true;
            // PLAN Aggregate exceptions 
            int count = _childControllers.Count;
            for (int i = 0; i < count; i++)
            {
                await _childControllers[i].Enable();
            }
        }
        public void Update(int millisecond)
        {
            for (int i = 0, length = _childUpdates.Count; i < length; i++)
            {
                var update = _childUpdates[i];
                if (update is IController controller &&
                     controller.IsStarted && controller.IsActive)
                {
                    update.Update(millisecond);
                }
            }
        }
        public void LateUpdate(int millisecond)
        {
            for (int i = 0, length = _childLateUpdates.Count; i < length; i++)
            {
                var update = _childLateUpdates[i];

                if (update is IController controller &&
                   controller.IsStarted && controller.IsActive)
                {
                    update.LateUpdate(millisecond);
                }
            }
        }
        public async PromiseTask Stop()
        {
            if (!IsStarted) return;

            int count = _childControllers.Count;
            for (int i = 0; i < count; i++)
            {
                await _childControllers[i].Stop();
            }
            IsStarted = false;
        }
        public async PromiseTask Disable(bool closeImmediately)
        {
            if (!IsStarted || !IsActive) return;

            int count = _childControllers.Count;
            for (int i = 0; i < count; i++)
            {
                await _childControllers[i].Disable(closeImmediately);
            }
            IsActive = false;
        }

        protected override async PromiseTask OnPreloadAsync(UnsafeCancellationToken cancellation)
        {
            int count = _childControllers.Count;
            for (int i = 0; i < count; i++)
            {
                if (_childControllers[i] is IPreloadable preloadable)
                {
                    await preloadable.Prepare(cancellation);
                }
            }
            await base.OnPreloadAsync(cancellation);
        }
        
        #endregion

        #region Inject Config
        public void Inject()
        {
            if (IsInjected)
                return;

            IsInjected = true;

            foreach (var ctrl in _childControllers)
            {
                _context.InjectionBinder.Injector.Inject(ctrl);
            }
        }

        public void Deject()
        {
            if (!IsInjected)
                return;

            IsInjected = false;

            foreach (var ctrl in _childControllers)
            {
                _context.InjectionBinder.Injector.Deject(ctrl);
            }
        }

        public void AssignContext(IContext context)
        {
            _context = context;
        }


        #endregion
    }
}