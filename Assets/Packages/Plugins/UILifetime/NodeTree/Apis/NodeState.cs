namespace Cr7Sund.LifeTime
{
    // modify node state internally
    public enum NodeState
    {
        Default,
        Adding,
        Ready,
        Removing,
        Unloading,
        Removed,
        Unloaded,
        Failed,
    }
}
