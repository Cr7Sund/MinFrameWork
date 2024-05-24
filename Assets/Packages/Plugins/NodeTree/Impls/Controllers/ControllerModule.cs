using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using System.Threading;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;

namespace Cr7Sund.NodeTree.Impl
{
    public class ControllerModule : AsyncLoadable, IControllerModule
    {
        protected IContext _context;
        protected List<IController> _childControllers;
        protected List<IUpdatable> _childUpdates;
        protected List<ILateUpdate> _childLateUpdates;
        [Inject] protected IPoolBinder _poolBinder;

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

            var tmpList = _poolBinder.AutoCreate<List<IController>>();
            for (int i = 0; i < _childControllers.Count; i++)
            {
                tmpList.Add(_childControllers[i]);
            }
            foreach (var ctrl in tmpList)
            {
                await ctrl.Start(cancellation);
            }
            _poolBinder.Return<List<IController>, IController>(tmpList);
        }
        public async PromiseTask Enable()
        {
            if (!IsStarted || IsActive) return;

            IsActive = true;

            var tmpList = _poolBinder.AutoCreate<List<IController>>();
            for (int i = 0; i < _childControllers.Count; i++)
            {
                tmpList.Add(_childControllers[i]);
            }
            foreach (var ctrl in tmpList)
            {
                await ctrl.Enable();
            }
            _poolBinder.Return<List<IController>, IController>(tmpList);
        }
        public void Update(int millisecond)
        {
            var tmpList = _poolBinder.AutoCreate<List<IUpdatable>>();
            for (int i = 0; i < _childUpdates.Count; i++)
            {
                if (_childUpdates[i] is IController controller &&
                      controller.IsStarted && controller.IsActive)
                {
                    tmpList.Add(_childUpdates[i]);
                }
            }
            foreach (var ctrl in tmpList)
            {
                ctrl.Update(millisecond);
            }
            _poolBinder.Return<List<IUpdatable>, IUpdatable>(tmpList);
        }
        public void LateUpdate(int millisecond)
        {
            var tmpList = _poolBinder.AutoCreate<List<ILateUpdate>>();
            for (int i = 0; i < _childLateUpdates.Count; i++)
            {
                if (_childLateUpdates[i] is IController controller &&
                      controller.IsStarted && controller.IsActive)
                {
                    tmpList.Add(_childLateUpdates[i]);
                }
            }
            foreach (var ctrl in tmpList)
            {
                ctrl.LateUpdate(millisecond);
            }
            _poolBinder.Return<List<ILateUpdate>, ILateUpdate>(tmpList);
        }

        public async PromiseTask Stop()
        {
            if (!IsStarted) return;


            var tmpList = _poolBinder.AutoCreate<List<IController>>();
            for (int i = 0; i < _childControllers.Count; i++)
            {
                tmpList.Add(_childControllers[i]);
            }
            foreach (var ctrl in tmpList)
            {
                await ctrl.Stop();
            }
            _poolBinder.Return<List<IController>, IController>(tmpList);

            IsStarted = false;
        }
        public async PromiseTask Disable(bool closeImmediately)
        {
            if (!IsStarted || !IsActive) return;

            var tmpList = _poolBinder.AutoCreate<List<IController>>();
            for (int i = 0; i < _childControllers.Count; i++)
            {
                tmpList.Add(_childControllers[i]);
            }
            foreach (var ctrl in tmpList)
            {
                await ctrl.Disable(closeImmediately);
            }
            _poolBinder.Return<List<IController>, IController>(tmpList);

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

            _context.InjectionBinder.Injector.Inject(this);
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