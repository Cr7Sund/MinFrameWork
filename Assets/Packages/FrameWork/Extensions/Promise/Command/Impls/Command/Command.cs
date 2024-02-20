using Cr7Sund.Package.Api;
namespace Cr7Sund.Package.Impl
{
    public abstract class Command : BaseCommand, ICommand
    {
        public abstract void OnExecute();
    }
    public abstract class Command<PromisedT> : BaseCommand, ICommand<PromisedT>
    {
        public abstract PromisedT OnExecute(PromisedT value);
    }

    public abstract class Command<PromisedT, ConvertedT> : BaseCommand, ICommand<PromisedT, ConvertedT>
    {
        public abstract ConvertedT OnExecute(PromisedT value);
    }
}
