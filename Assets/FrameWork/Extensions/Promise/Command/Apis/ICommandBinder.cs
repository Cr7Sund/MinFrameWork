namespace Cr7Sund.Framework.Api
{
    public interface ICommandBinder
    {
        public T GetOrCreate<T>() where T : class, IBaseCommand;
        public T Get<T>() where T : class, IBaseCommand;
    }
}
