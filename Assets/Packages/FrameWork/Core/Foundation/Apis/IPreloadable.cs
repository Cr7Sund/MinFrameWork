namespace Cr7Sund
{
    public interface IPreloadable
    {
        PromiseTask Prepare(UnsafeCancellationToken cancellation);
    }
}
