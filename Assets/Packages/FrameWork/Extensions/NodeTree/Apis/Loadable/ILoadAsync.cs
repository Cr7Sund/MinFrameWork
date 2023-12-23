using Cr7Sund.Framework.Api;
namespace Cr7Sund.NodeTree.Api
{
    public interface ILoadAsync<T>
    {
        LoadState State { get; }
        IPromise<T> LoadStatus { get; }
        IPromise<T> UnloadStatus { get; }


        IPromise<T> LoadAsync(T value);
        IPromise<T> UnloadAsync(T value);
    }

    public interface ILoadAsync
    {
        LoadState State { get; }
        IPromise LoadStatus { get; }
        IPromise UnloadStatus { get; }


        IPromise LoadAsync();
        IPromise UnloadAsync();
    }
}
