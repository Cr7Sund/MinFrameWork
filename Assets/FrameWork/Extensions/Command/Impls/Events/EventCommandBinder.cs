using System;
using System.Dynamic;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class EventCommandBinder : CommandBinder
    {
        protected override ICommand CreateCommand(Type cmdType, object data)
        {
            InjectionBinder.Bind<ICommand>().To(cmdType);
            if(data is IEvent @event){
                InjectionBinder.Bind<IEvent>().ToValue(@event).ToInject(false);
            }

            var command = InjectionBinder.GetInstance<ICommand>();
            if (command ==null)
            {
                string msg = "A Command ";
                if (data is IEvent)
                {
                    IEvent evt = (IEvent)data;
                    msg += "tied to event " + evt.Type;
                }
                msg += " could not be instantiated.\nThis might be caused by a null pointer during instantiation or failing to override Execute (generally you shouldn't have constructor code in Commands).";
                throw new CommandException(msg, CommandExceptionType.BAD_CONSTRUCTOR);
            }

            command.data = data;
            if (data is IEvent)
            {
                InjectionBinder.Unbind<IEvent>();
            }
            InjectionBinder.Unbind<ICommand>();

            return command;
        }

        protected override void DisposeSequenceData(object data)
        {
            if (data is IPoolable poolable)
            {
                poolable.Release();
            }
        }
    }
}