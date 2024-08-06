using System;
using Cr7Sund.NodeTree.Impl;
using INode = Cr7Sund.NodeTree.Api.INode;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Package.EventBus.Impl;

namespace Cr7Sund.Editor.NodeGraph
{
    public class NodeGraphContext : CrossContext
    {
        private string Channel => "NodeGraph";



        public NodeGraphContext() : base()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }

        public override void AddComponents(INode self)
        {
            var logger = InternalLoggerFactory.Create(Channel);
            var manifest = Activator.CreateInstance(NodeGraphSetting.Instance.LastGraphManifestType.GetSerialType());
            
            InjectionBinder.Bind<IInternalLog>().To(logger).AsCrossContext();
            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsCrossContext().AsSingleton();
            InjectionBinder.Bind<IEventBus>().To<EventBus>().AsCrossContext().AsSingleton(); // same level
            InjectionBinder.Bind<Manifest>().To(manifest).AsCrossContext();
            InjectionBinder.Bind<IPromiseTimer>().To<PromiseTimer>().AsCrossContext().AsSingleton();
        }

        public override void RemoveComponents()
        {
            InjectionBinder.Unbind<IInternalLog>();
            InjectionBinder.Unbind<IPoolBinder>();
            InjectionBinder.Unbind<IEventBus>();
            InjectionBinder.Unbind<Manifest>();
            InjectionBinder.Unbind<IPromiseTimer>();
        }
    }
}