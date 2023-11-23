namespace Cr7Sund.Framework.Api
{
    public interface ICommand : IBaseCommand
    {
        void OnExecute();
    }

    public interface ICommand<PromisedT> : IBaseCommand
    {
        PromisedT OnExecute(PromisedT value);
    }

    public interface ICommand<in PromisedT, out ConvertedT> : IBaseCommand
    {
        ConvertedT OnExecute(PromisedT value);
    }


}
