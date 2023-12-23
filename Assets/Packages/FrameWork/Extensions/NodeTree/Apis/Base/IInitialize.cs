namespace Cr7Sund.NodeTree.Api
{
    public interface IInitialize
    {
        bool IsInit { get; }

        
        void Init();
        void Dispose();
    }
}
