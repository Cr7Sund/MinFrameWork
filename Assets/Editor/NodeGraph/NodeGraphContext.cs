using System;
using Cr7Sund.NodeTree.Impl;
using INode = Cr7Sund.NodeTree.Api.INode;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Package.EventBus.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.IocContainer;
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund.Editor.NodeGraph
{
    public class NodeGraphContext : CrossContext, INodeContext
    {
        private string Channel => "NodeGraph";

        public NodeGraphContext() : base()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }


        public void AddComponents(INode self)
        {
            var logger = InternalLoggerFactory.Create(Channel);
            var manifest = Activator.CreateInstance(NodeGraphSetting.Instance.LastGraphManifestType.GetSerialType());

            BindInstanceAsCrossContext<IInternalLog>(logger);
            BindAsCrossAndSingleton<IPoolBinder, PoolBinder>();
            BindAsCrossAndSingleton<IEventBus, EventBus>();
            BindInstanceAsCrossContext<Manifest>(manifest);
            BindAsCrossAndSingleton<IPromiseTimer, PromiseTimer>();
        }

        public void RemoveComponents()
        {
            Unbind<IInternalLog>();
            Unbind<IPoolBinder>();
            Unbind<IEventBus>();
            Unbind<Manifest>();
            Unbind<IPromiseTimer>();
        }
    }
}
