using Cr7Sund.PackageTest.Api;
namespace Cr7Sund.NodeTree.Api
{
    public interface ILoadAsync<T>
    {
        LoadState LoadState { get; }
        IPromise<T> LoadStatus { get; }
        IPromise<T> UnloadStatus { get; }


        IPromise<T> LoadAsync(T value);
        IPromise<T> PreLoadAsync(T value);
        IPromise<T> UnloadAsync(T value);
        IPromise<T> CancelLoad();
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
