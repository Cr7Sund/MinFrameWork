using Cr7Sund.Framework.Api;
namespace Cr7Sund.NodeTree.Api
{
    public interface ILoadAsync
    {
        LoadState State { get; }
        IPromise LoadStatus { get; }
        IPromise UnloadStatus { get; }
        
        
        IPromise LoadAsync();
        IPromise UnloadAsync();
    }
}
