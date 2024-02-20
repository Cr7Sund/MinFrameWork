namespace Cr7Sund.Package.Api
{
    /// <summary>
    /// Represents an interface for objects that participate in a sequence.
    /// </summary>
    public interface ISequence
    {
        /// <summary>
        /// Gets or sets the length of the slice in the sequence.
        /// </summary>
        float SliceLength { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the sequence.
        /// </summary>
        int SequenceID { get; set; }
    }
}
