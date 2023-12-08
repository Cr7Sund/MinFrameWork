namespace Cr7Sund.NodeTree.Api
{
    public interface IInitializable
    {
        bool IsInit { get; }

        
        void Init();
        void Dispose();
    }
}
