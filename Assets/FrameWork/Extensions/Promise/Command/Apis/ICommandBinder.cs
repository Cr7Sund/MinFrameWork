namespace Cr7Sund.Framework.Api
{
    public interface ICommandBinder
    {
        public T Get<T>() where T : class, IBaseCommand;
    }
}