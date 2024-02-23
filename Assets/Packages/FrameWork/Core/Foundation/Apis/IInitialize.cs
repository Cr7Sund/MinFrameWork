namespace Cr7Sund
{
    public interface IInitialize
    {
        bool IsInit { get; }

        
        void Init();
        void Dispose();
    }
}
