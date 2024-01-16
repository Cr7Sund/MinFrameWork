using Cr7Sund.EventBus.Api;
using Cr7Sund.Framework.Api;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;
namespace Cr7Sund.Server.Apis
{
    /// <summary>
    /// Defines the interface for a scene management module that allows running, removing, preloading, and observing scenes.
    /// </summary>
    public interface ISceneModule : IObservable
    {
        /// <summary>
        /// Gets the currently focused scene.
        /// </summary>
        SceneNode FocusScene { get; }

        /// <summary>
        /// Runs a scene, unloading all previously loaded scenes.
        /// </summary>
        /// <param name="key">The key of the scene to run.</param>
        IPromise<INode> SwitchScene(SceneKey key);

        /// <summary>
        /// Removes a scene from the module.
        /// </summary>
        /// <param name="key">The key of the scene to remove.</param>
        IPromise<INode> RemoveScene(SceneKey key);

        /// <summary>
        /// Unload a scene from the module.
        /// </summary>
        /// <param name="key">The key of the scene to remove.</param>
        IPromise<INode> UnloadScene(SceneKey key);

        /// <summary>
        /// Asynchronously preloads a scene in the background.
        /// </summary>
        /// <param name="key">The key of the scene to preload.</param>
        /// <returns>An asynchronous operation representing the preloading process.</returns>
        IPromise<INode> PreLoadScene(SceneKey key);

        /// <summary>
        /// Adds and runs a scene without unloading previously loaded scenes.
        /// </summary>
        /// <param name="key">The key of the scene to add and run.</param>
        IPromise<INode> AddScene(SceneKey key);
    }
}
