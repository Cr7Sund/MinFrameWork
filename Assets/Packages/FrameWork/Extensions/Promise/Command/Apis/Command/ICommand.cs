namespace Cr7Sund.Package.Api
{
    /// <summary>
    /// Represents a basic command without any parameter or return value.
    /// </summary>
    public interface ICommand : IBaseCommand
    {
        /// <summary>
        /// Executes the command without any parameters or return value.
        /// </summary>
        void OnExecute();
    }

    /// <summary>
    /// Represents a command with a promised parameter and return value.
    /// </summary>
    /// <typeparam name="PromisedT">The type of the promised parameter and return value.</typeparam>
    public interface ICommand<PromisedT> : IBaseCommand
    {
        /// <summary>
        /// Executes the command with a promised parameter and returns a promised value.
        /// </summary>
        /// <param name="value">The promised parameter value.</param>
        /// <returns>The promised return value.</returns>
        PromisedT OnExecute(PromisedT value);
    }

    /// <summary>
    /// Represents a command with a promised input parameter and a converted return value.
    /// </summary>
    /// <typeparam name="PromisedT">The type of the promised input parameter.</typeparam>
    /// <typeparam name="ConvertedT">The type of the converted return value.</typeparam>
    public interface ICommand<in PromisedT, out ConvertedT> : IBaseCommand
    {
        /// <summary>
        /// Executes the command with a promised input parameter and returns a converted value.
        /// </summary>
        /// <param name="value">The promised input parameter value.</param>
        /// <returns>The converted return value.</returns>
        ConvertedT OnExecute(PromisedT value);
    }
}
