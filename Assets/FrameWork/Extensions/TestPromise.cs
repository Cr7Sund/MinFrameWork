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
        private IPromiseCommandBinder promiseBinder;
        private ICommandPromise commandPromise;
        private IPromise promise;


        
        public void SetUp()
        {
            injectionBinder = new InjectionBinder();
            poolBinder = new PoolBinder();

            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);

            promiseBinder = new PromiseCommandBinder();
            injectionBinder.Injector.Inject(promiseBinder);

            commandPromise = new CommandPromise();
            promise = new Promise();
        }
        // Start is called before the first frame update
        void  TestPromiseCommand()
        {
            for (int a = 0; a < Count; a++)
            {
                var promiseBinding = new CommandPromise();
                SimplePromise.simulatePromise = new Promise();

                promiseBinding
                 .Then<SimplePromiseCommandTwo>()
                 .Then<SimpleAsyncPromiseCommandOne>()
                 .Then<SimplePromiseCommandOne>();

                promiseBinding.Resolve();
                SimplePromise.simulatePromise.Resolve();
            }
        }

        void TestDelegatePromise()
        {
            for (int a = 0; a < Count; a++)
            {
                var promise = new Promise();
                SimplePromise.simulatePromise = new Promise();
                SimplePromiseCommandTwo command1 = new SimplePromiseCommandTwo();
                SimpleAsyncPromiseCommandOne command2 = new SimpleAsyncPromiseCommandOne();
                SimplePromiseCommandOne command3 = new SimplePromiseCommandOne();

                promise
                        .Then(command1.OnExecute, command1.OnCatch, command1.OnProgress)
                        .Then(command2.OnExecute, command2.OnCatch, command2.OnProgress)
                        .Then(command3.OnExecute, command3.OnCatch, command3.OnProgress);


                promise.Resolve();

                SimplePromise.simulatePromise.Resolve();
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

    public class TestSimplePromiseCommandOne : PromiseCommand
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