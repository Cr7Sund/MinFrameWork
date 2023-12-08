using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace TestMono
{

    public class TestPromise : MonoBehaviour
    {
        private const int Count = 100;
        private ICommandPromiseBinder _commandPromiseBinder;
        private ICommandPromise commandPromise;
        private IInjectionBinder injectionBinder;
        private IPoolBinder poolBinder;
        private IPromise promise;

        private void Start()
        {
            SetUp();
        }

        private float ReturnsFloat()
        {
            throw new NotImplementedException();
        }

        // Update is called once per frame
        private void Update()
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



        public void SetUp()
        {
            injectionBinder = new InjectionBinder();
            poolBinder = new PoolBinder();

            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);
            injectionBinder.Bind<ICommandBinder>().To<CommandBinder>().AsSingleton();
            _commandPromiseBinder = new CommandPromiseBinder();
            injectionBinder.Injector.Inject(_commandPromiseBinder);

            commandPromise = new CommandPromise();
            promise = new Promise();
        }

        #region Usual Case - react often
        private void TestPromiseCommand()
        {
            var binder = new CommandPromiseBinder();
            binder.UsePooling = true;
            injectionBinder.Injector.Inject(binder);

            for (int a = 0; a < Count; a++)
            {
                binder.Bind(a).AsOnce()
                    .Then<SimpleCommandOne>()
                    .Then<SimpleCommandTwo>();

                binder.ReactTo(a);

                binder.Unbind(a);
            }
        }

        private void TestDelegatePromise()
        {
            var binder = new CommandPromiseBinder();
            injectionBinder.Injector.Inject(binder);


            binder.Bind("we")
                .Then<SimpleCommandOne>()
                .Then<SimpleCommandTwo>();
            for (int a = 0; a < Count; a++)
            {
                binder.ReactTo("we");
            }
        }

        #endregion
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
