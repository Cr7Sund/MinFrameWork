namespace Cr7Sund.PackageTest.Api
{
    /// <summary>
    /// Represents a binder for command promises, extending the generic IBinder interface with additional command-specific functionalities.
    /// </summary>
    public interface ICommandPromiseBinder : IBinder
    {
        /// <summary>
        /// Reacts to a specified trigger without providing any data.
        /// </summary>
        /// <param name="trigger">The trigger to react to.</param>
        void ReactTo(object trigger);

        /// <summary>
        /// Binds a new command promise to the specified trigger.
        /// </summary>
        /// <param name="trigger">The trigger to bind to.</param>
        /// <returns>The command promise binding.</returns>
        new ICommandPromiseBinding Bind(object trigger);

        /// <summary>
        /// Gets the command promise binding associated with the specified key.
        /// </summary>
        /// <param name="key">The key associated with the binding.</param>
        /// <returns>The command promise binding.</returns>
        new ICommandPromiseBinding GetBinding(object key);

        /// <summary>
        /// Gets the command promise binding associated with the specified key and name.
        /// </summary>
        /// <param name="key">The key associated with the binding.</param>
        /// <param name="name">The name associated with the binding.</param>
        /// <returns>The command promise binding.</returns>
        new ICommandPromiseBinding GetBinding(object key, object name);
    }
}
