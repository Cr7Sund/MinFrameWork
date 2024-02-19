namespace Cr7Sund.NodeTree.Api
{
    // modify node state internally
    public enum NodeState
    {
        Default,
        Preload,
        Preloaded,
        Adding,
        Ready,
        Removing,
        Unloading,
        Removed,
        Unloaded,
    }
}
