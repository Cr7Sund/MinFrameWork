namespace Cr7Sund.Framework.Api
{
    public interface IPromiseCommand : IBaseCommand
    {
        void OnExecute();
    }
    
    public interface IPromiseCommand<PromisedT> : IBaseCommand
    {
        PromisedT OnExecute(PromisedT value);
    }

    public interface IPromiseCommand<PromisedT, ConvertedT> : IBaseCommand
    {
        ConvertedT OnExecute(PromisedT value);
    }


}