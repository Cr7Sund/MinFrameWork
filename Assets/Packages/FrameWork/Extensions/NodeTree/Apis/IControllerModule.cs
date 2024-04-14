namespace Cr7Sund.NodeTree.Api
{
    /// <summary>
    /// Defines the interface for the controller module.
    /// </summary>
    public interface IControllerModule : IUpdatable, ILateUpdate, IController, ILoadAsync, IInjectable
    {
        /// <summary>
        /// Adds an controller of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the controller.</typeparam>
        /// <returns>True if the addition is successful; otherwise, false.</returns>
        PromiseTask AddController<T>() where T : IController;

        /// <summary>
        /// Adds the specified controller.
        /// </summary>
        /// <param name="controller">The controller to add.</param>
        /// <returns>True if the addition is successful; otherwise, false.</returns>
        PromiseTask AddController(IController controller);

        /// <summary>
        /// Removes the controller of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the controller to remove.</typeparam>
        /// <returns>True if the removal is successful; otherwise, false.</returns>
        PromiseTask RemoveController<T>() where T : IController;

        /// <summary>
        /// Removes the specified controller.
        /// </summary>
        /// <param name="controller">The controller to remove.</param>
        /// <returns>True if the removal is successful; otherwise, false.</returns>
        PromiseTask RemoveController(IController controller);
    }
}
