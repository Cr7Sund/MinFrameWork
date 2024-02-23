namespace Cr7Sund
{
    public interface ILifeTime
    {
        bool IsActive { get; }

        void Enable();
        void Disable();
        // void SetActive(bool active);
    }
}
