namespace Cr7Sund.Editor
{
    public interface IPlayModeChange
    {
        bool IsActive { get; set; }
        void Enable();
        void Disable();
    }
}
