using Cr7Sund.Framework.Api;


namespace Cr7Sund.Framework.Impl
{
    public abstract class PromiseCommand : BaseCommand, IPromiseCommand
    {
        public abstract void OnExecute();

    }
    public abstract class PromiseCommand<PromisedT> : BaseCommand, IPromiseCommand<PromisedT>
    {
        public abstract PromisedT OnExecute(PromisedT value);

    }

    public abstract class PromiseCommand<PromisedT, ConvertedT> : BaseCommand, IPromiseCommand<PromisedT, ConvertedT>
    {
        public abstract ConvertedT OnExecute(PromisedT value);

    }
}