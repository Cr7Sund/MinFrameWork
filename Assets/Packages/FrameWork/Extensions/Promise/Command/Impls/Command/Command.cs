using Cr7Sund.PackageTest.Api;
namespace Cr7Sund.PackageTest.Impl
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
