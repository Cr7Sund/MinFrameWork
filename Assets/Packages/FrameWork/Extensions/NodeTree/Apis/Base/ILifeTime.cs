namespace Cr7Sund.NodeTree.Api
{
    public interface ILifeTime
    {
        bool IsActive { get; }

        void Enable();
        void Disable();
        // void SetActive(bool active);
    }
}
