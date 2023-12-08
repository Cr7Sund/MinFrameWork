namespace Cr7Sund.Framework.Api
{
    /// <summary>
    /// Represents a binder for command promises with a specified promised type, extending the generic IBinder interface with additional command-specific functionalities.
    /// </summary>
    /// <typeparam name="PromisedT">The type of the promised value.</typeparam>
    public interface ICommandPromiseBinder<PromisedT> : IBinder
    {
        /// <summary>
        /// Reacts to a specified trigger and provides the promised data.
        /// </summary>
        /// <param name="trigger">The trigger to react to.</param>
        /// <param name="data">The promised data.</param>
        void ReactTo(object trigger, PromisedT data);

        /// <summary>
        /// Binds a new command promise to the specified trigger.
        /// </summary>
        /// <param name="trigger">The trigger to bind to.</param>
        /// <returns>The command promise binding.</returns>
        new ICommandPromiseBinding<PromisedT> Bind(object trigger);

        /// <summary>
        /// Gets the command promise binding associated with the specified key.
        /// </summary>
        /// <param name="key">The key associated with the binding.</param>
        /// <returns>The command promise binding.</returns>
        new ICommandPromiseBinding<PromisedT> GetBinding(object key);

        /// <summary>
        /// Gets the command promise binding associated with the specified key and name.
        /// </summary>
        /// <param name="key">The key associated with the binding.</param>
        /// <param name="name">The name associated with the binding.</param>
        /// <returns>The command promise binding.</returns>
        new ICommandPromiseBinding<PromisedT> GetBinding(object key, object name);
    }
}
