namespace TestMono
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Cr7Sund.Framework.Api;
    using Cr7Sund.Framework.Impl;
    using UnityEngine;
    using UnityEngine.Profiling;

    public class TestPromise : MonoBehaviour
    {
        private const int Count = 100;
        private IInjectionBinder injectionBinder;
        private IPoolBinder poolBinder;
        private ICommandPromiseBinder _commandPromiseBinder;
        private ICommandPromise commandPromise;
        private IPromise promise;


        
        public void SetUp()
        {
            injectionBinder = new InjectionBinder();
            poolBinder = new PoolBinder();

            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);

            _commandPromiseBinder = new CommandPromiseBinder();
            injectionBinder.Injector.Inject(_commandPromiseBinder);

            commandPromise = new CommandPromise();
            promise = new Promise();
        }
        
        void  TestPromiseCommand()
        {
            for (int a = 0; a < Count; a++)
            {
                var promiseBinding = new CommandPromise();

                promiseBinding
                 .Then<SimpleCommandOne>()
                 .Then<SimpleCommandTwo>();
                promiseBinding.Resolve();
            }
        }

        void TestDelegatePromise()
        {
            for (int a = 0; a < Count; a++)
            {
                var binder = new CommandPromiseBinder();
                injectionBinder.Injector.Inject(binder);

                binder.Bind("SomeEnum.TWO")
                 .Then<SimpleCommandOne>()
                 .Then<SimpleCommandTwo>();

                binder.ReactTo("SomeEnum.TWO");
            }
        }

        void Start()
        {
            SetUp();


        }

        // Update is called once per frame
        void Update()
        {
            Profiler.BeginSample("Promise Command");
            {
                TestPromiseCommand();
            }
            Profiler.EndSample();


            Profiler.BeginSample("Promise Delegate");
            {
                TestDelegatePromise();
            }
            Profiler.EndSample();
        }
    }

    public class TestSimpleCommandOne : Command
    {
        public override void OnCatch(Exception e)
        {

        }

        public override void OnExecute()
        {
        }

        public override void OnProgress(float progress)
        {
        }
    }
}