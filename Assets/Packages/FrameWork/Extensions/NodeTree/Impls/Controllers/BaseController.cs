﻿using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class BaseController : IController
    {
        public bool IsStarted { get; private set; }
        public bool IsActive { get; private set; }


        // No preload
        // since it should be existed one, but preload the node is still no created

        public void Start()
        {
            IsStarted = true;
            if (MacroDefine.NoCatchMode)
            {
                OnStart();
            }
            else
            {
                try
                {
                    OnStart();
                }
                catch (System.Exception e)
                {
                    Console.Error($"{GetType().FullName}.OnStart Error: \n", e);
                }
            }
        }

        public void Stop()
        {
            if (MacroDefine.NoCatchMode)
            {
                OnStop();
            }
            else
            {
                try
                {
                    OnStop();
                }
                catch (System.Exception e)
                {
                    Console.Error($"{GetType().FullName}.OnStop Error: \n", e);
                }
            }

            IsStarted = false;
        }

        public void Enable()
        {
            IsActive = true;

            if (MacroDefine.NoCatchMode)
            {
                OnEnable();
            }
            else
            {
                try
                {
                    OnEnable();
                }
                catch (System.Exception e)
                {
                    Console.Error($"{GetType().FullName}.OnEnable Error: \n", e);
                }
            }
        }

        public void Disable()
        {
            if (MacroDefine.NoCatchMode)
            {
                OnDisable();
            }
            else
            {
                try
                {
                    OnDisable();
                }
                catch (System.Exception e)
                {
                    Console.Error($"{GetType().FullName}.OnDisable Error: \n", e);
                }
            }
            IsActive = false;
        }


        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

    }
}
