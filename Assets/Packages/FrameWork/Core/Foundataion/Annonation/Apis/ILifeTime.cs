
namespace Cr7Sund
{
    public interface ILifeTime
    {
        bool IsActive { get; set; }

        PromiseTask Enable();
        PromiseTask Disable(bool closeImmediately);

    }
}
