namespace Cr7Sund
{
    public interface IInjectable
    {
        bool IsInjected { get; }
        
        
        void Inject();
        void Deject();
    }
}
