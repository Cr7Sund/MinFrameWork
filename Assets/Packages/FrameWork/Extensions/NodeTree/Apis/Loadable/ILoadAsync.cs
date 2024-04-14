using System;
namespace Cr7Sund.NodeTree.Api
{
    public interface ILoadAsync : IDisposable
    {
        LoadState LoadState { get; }
    }
}
