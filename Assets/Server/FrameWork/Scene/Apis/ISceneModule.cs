using Cr7Sund.Package.EventBus.Api;
using Cr7Sund.Package.Api;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.Server.Scene.Apis
{
    /// <summary>
    /// Defines the interface for a scene management module that allows running, removing, preloading, and observing scenes.
    /// </summary>
    public interface ISceneModule : IObservable
    {
        /// <summary>
        /// Gets the currently focused scene.
        /// </summary>
        INode FocusScene { get; }

        /// <summary>
        /// Runs a scene, unloading all previously loaded scenes.
        /// </summary>
        /// <param name="key">The key of the scene to run.</param>
        IPromise<INode> SwitchScene(IAssetKey key);

        /// <summary>
        /// Removes a scene from the module.
        /// </summary>
        /// <param name="key">The key of the scene to remove.</param>
        IPromise<INode> RemoveScene(IAssetKey key);

        /// <summary>
        /// Unload a scene from the module.
        /// </summary>
        /// <param name="key">The key of the scene to remove.</param>
        IPromise<INode> UnloadScene(IAssetKey key);

        /// <summary>
        /// Asynchronously preloads a scene in the background.
        /// </summary>
        /// <param name="key">The key of the scene to preload.</param>
        /// <returns>An asynchronous operation representing the preloading process.</returns>
        IPromise<INode> PreLoadScene(IAssetKey key);

        /// <summary>
        /// Adds and runs a scene without unloading previously loaded scenes.
        /// </summary>
        /// <param name="key">The key of the scene to add and run.</param>
        IPromise<INode> AddScene(IAssetKey key);
    }
}
