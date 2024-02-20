namespace Cr7Sund.Package.Api
{
    /// <summary>
    /// Represents a binder for commands, providing methods to get or create instances of specific command types.
    /// </summary>
    public interface ICommandBinder
    {
        /// <summary>
        /// Gets or creates an instance of a specified command type.
        /// </summary>
        /// <typeparam name="T">The type of the command to get or create.</typeparam>
        /// <returns>An instance of the specified command type.</returns>
        /// <remarks>
        /// The method ensures that a command instance of the specified type is available, creating one if necessary.
        /// </remarks>
        T GetOrCreate<T>() where T : class, IBaseCommand;

        /// <summary>
        /// Gets an instance of a specified command type.
        /// </summary>
        /// <typeparam name="T">The type of the command to get.</typeparam>
        /// <returns>An instance of the specified command type, or null if not found.</returns>
        /// <remarks>
        /// The method attempts to get an existing instance of the specified command type without creating a new one.
        /// </remarks>
        T Get<T>() where T : class, IBaseCommand;
    }
}
