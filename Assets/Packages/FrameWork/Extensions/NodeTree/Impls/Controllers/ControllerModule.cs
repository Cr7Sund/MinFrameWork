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
        public bool IsInit
        {
            get;
            private set;
        }
        public CancellationTokenSource AddCancellation { get; }
        public CancellationTokenSource RemoveCancellation { get; }


        public ControllerModule()
        {
            _childControllers = new List<IController>();
            _childLateUpdates = new List<ILateUpdate>();
            _childUpdates = new List<IUpdatable>();
        }


        #region IControllerModule Implementation
        public async PromiseTask AddController<T>() where T : IController
        {
            CheckBusiness<T>();
            await AddController(Activator.CreateInstance<T>());
        }

        public async PromiseTask AddController(IController controller)
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
                        await load.LoadAsync();
                    }
                    catch
                    {
                        await load.UnloadAsync();
                        throw;
                    }
                }

                try
                {
                    await controller.Start();
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
                    await controller.Disable();
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
                    await RemoveController(controller);
                }
            }
        }

        public async PromiseTask RemoveController(IController controller)
        {
            AssertUtil.NotNull(controller, NodeTreeExceptionType.EMPTY_CONTROLLER_REMOVE);
            CheckController(controller);

            if (controller.IsActive)
            {
                await controller.Disable();
            }

            if (controller.IsStarted)
            {
                await controller.Stop();
            }

            if (controller is AsyncLoadable load)
            {
                await load.UnloadAsync();
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
        public async PromiseTask Start()
        {
            if (IsStarted) return;

            IsStarted = true;
            int count = _childControllers.Count;
            for (int i = 0; i < count; i++)
            {
                await _childControllers[i].Start();
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
        public async PromiseTask Disable()
        {
            if (!IsStarted || !IsActive) return;

            int count = _childControllers.Count;
            for (int i = 0; i < count; i++)
            {
                await _childControllers[i].Disable();
            }
            IsActive = false;
        }

        public void RegisterAddTask(CancellationToken cancellationToken)
        {
            int count = _childControllers.Count;
            for (int i = 0; i < count; i++)
            {
                _childControllers[i].RegisterAddTask(cancellationToken);
            }
        }
        public void RegisterRemoveTask(CancellationToken cancellationToken)
        {
            for (int i = 0; i < _childControllers.Count; i++)
            {
                _childControllers[i].RegisterAddTask(cancellationToken);
            }
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