using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.NodeTree.Impl
{
    public class ControllerModule : AsyncLoadable<INode>, IControllerModule
    {
        protected IContext _context;
        protected List<IController> _lsControllers;
        protected List<IUpdate> _lsUpdates;
        protected List<ILateUpdate> _lsLateUpdates;

        public bool IsInjected
        {
            get;
            private set;
        }
        public bool IsActive
        {
            get;
            private set;
        }
        public bool IsStarted
        {
            get;
            private set;
        }
        public bool IsInit
        {
            get;
            private set;
        }


        public ControllerModule()
        {
            _lsControllers = new List<IController>();
            _lsLateUpdates = new List<ILateUpdate>();
            _lsUpdates = new List<IUpdate>();
        }


        #region IControllerModule Implementation
        public bool AddController<T>() where T : IController
        {
            CheckBusiness<T>();
            return AddController(Activator.CreateInstance<T>());
        }

        public bool AddController(IController controller)
        {
            AssertUtil.NotNull(controller, NodeTreeExceptionType.EMPTY_CONTROLLER_ADD);
            CheckController(controller);

            if (IsStarted)
            {
                _context.InjectionBinder.Injector.Inject(controller);

                if (controller is ILoadAsync load)
                {
                    load.LoadAsync().Then(() =>
                    {
                        controller.Start();
                        controller.Enable();
                        AddChild(controller);
                        _lsControllers.Add(controller);
                    });
                    return true;
                }
                else
                {
                    controller.Start();
                    controller.Enable();
                }
            }

            _lsControllers.Add(controller);
            AddChild(controller);

            return true;
        }

        public bool RemoveController<T>() where T : IController
        {
            for (int i = 0, length = _lsControllers.Count; i < length; i++)
            {
                var controller = _lsControllers[i];
                if (controller.GetType() == typeof(T))
                {
                    return RemoveController(controller);
                }
            }

            return false;
        }

        public bool RemoveController(IController controller)
        {
            AssertUtil.NotNull(controller, NodeTreeExceptionType.EMPTY_CONTROLLER_REMOVE);
            CheckController(controller);

            if (controller.IsActive)
            {
                RemoveChild(controller);

                controller.Disable();

                if (controller is ILoadAsync load)
                {
                    load.UnloadAsync().Then(() =>
                    {
                        if (controller.IsStarted)
                            controller.Stop();
                    });
                }
                else
                {
                    if (controller.IsStarted)
                        controller.Stop();
                }
            }

            _context.InjectionBinder.Injector.Uninject(controller);
            return _lsControllers.Remove(controller);
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
            for (int i = 0; i < _lsControllers.Count; i++)
            {
                if (_lsControllers[i].GetType() == type)
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
            if (controller is IUpdate update)
            {
                if (!_lsUpdates.Contains(update))
                    _lsUpdates.Add(update);
            }

            if (controller is ILateUpdate lateUpdate)
            {
                if (!_lsLateUpdates.Contains(lateUpdate))
                    _lsLateUpdates.Add(lateUpdate);
            }
        }

        private void RemoveChild(IController controller)
        {
            if (controller == null)
            {
                return;
            }
            if (controller is IUpdate update)
            {
                if (_lsUpdates.Contains(update))
                    _lsUpdates.Remove(update);
            }

            if (controller is ILateUpdate lateUpdate)
            {
                if (_lsLateUpdates.Contains(lateUpdate))
                    _lsLateUpdates.Remove(lateUpdate);
            }
        }

        #endregion

        #region LifeCycles
        public void Start()
        {
            if (IsStarted) return;

            IsStarted = true;
            for (int i = 0; i < _lsControllers.Count; i++)
            {
                _lsControllers[i].Start();
            }
        }
        public void Enable()
        {
            if (!IsStarted || IsActive) return;

            IsActive = true;
            for (int i = 0; i < _lsControllers.Count; i++)
            {
                _lsControllers[i].Enable();
            }
        }
        public void Update(int millisecond)
        {
            for (int i = 0, length = _lsUpdates.Count; i < length; i++)
            {
                var update = _lsUpdates[i];
                if (update.IsStarted && update.IsActive)
                {
                    update.Update(millisecond);
                }
            }
        }
        public void LateUpdate(int millisecond)
        {
            for (int i = 0, length = _lsLateUpdates.Count; i < length; i++)
            {
                var update = _lsLateUpdates[i];

                if (update.IsStarted && update.IsActive)
                    update.LateUpdate(millisecond);
            }
        }
        public void Stop()
        {
            if (!IsStarted) return;

            for (int i = 0; i < _lsControllers.Count; i++)
            {
                _lsControllers[i].Stop();
            }
            IsStarted = false;
        }
        public void Disable()
        {
            if (!IsStarted || !IsActive) return;

            for (int i = 0; i < _lsControllers.Count; i++)
            {
                _lsControllers[i].Disable();
            }
            IsActive = false;
        }


        #endregion

        #region Inject Config
        public void Inject()
        {
            if (IsInjected)
                return;

            IsInjected = true;

            foreach (var ctrl in _lsControllers)
            {
                _context.InjectionBinder.Injector.Inject(ctrl);
            }
        }

        public void DeInject()
        {
            if (!IsInjected)
                return;

            IsInjected = false;

            foreach (var ctrl in _lsControllers)
            {
                _context.InjectionBinder.Injector.Uninject(ctrl);
            }
        }

        public void AssignContext(IContext context)
        {
            _context = context;
        }
        #endregion


        protected override IPromise<INode> OnLoadAsync(INode content)
        {
            return Promise<INode>.Resolved(content);
        }

        protected override IPromise<INode> OnUnloadAsync(INode content)
        {
            return Promise<INode>.Resolved(content);
        }
    }
}