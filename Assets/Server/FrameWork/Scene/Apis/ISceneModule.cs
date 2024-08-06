using Cr7Sund.Package.EventBus.Api;
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
        /// Unload a scene from the module.
        /// </summary>
        /// <param name="key">The key of the scene to remove.</param>
        PromiseTask UnloadScene(IAssetKey key);

        /// <summary>
        /// Asynchronously preloads a scene in the background.
        /// </summary>
        /// <param name="key">The key of the scene to preload.</param>
        /// <returns>An asynchronous operation representing the preloading process.</returns>
        PromiseTask PreLoadScene(IAssetKey key);

        /// <summary>
        /// Runs a scene, unloading all previously loaded scenes when additive
        /// </summary>
        /// <param name="key">The key of the scene to run.</param>
        PromiseTask SwitchScene(IAssetKey key);

        /// <summary>
        /// Adds and runs a scene without unloading previously loaded scenes.
        /// </summary>
        /// <param name="key">The key of the scene to add and run.</param>
        PromiseTask AddScene(IAssetKey key);

        void TimeOut(long milliseconds);
    }
}
