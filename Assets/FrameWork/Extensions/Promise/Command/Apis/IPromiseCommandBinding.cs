namespace Cr7Sund.Framework.Api
{
    public interface IPromiseCommandBinding : IBinding
    {
        bool UsePooling { get; set; }

        IPromiseCommandBinding Then<T>() where T : class, IPromiseCommand, new();

        new IPromiseCommandBinding ToName(object name);
    }
}