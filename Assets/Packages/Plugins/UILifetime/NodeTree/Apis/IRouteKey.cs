using Cr7Sund.LifeTime;
namespace Cr7Sund.LifeTime
{
    public interface IRouteKey : IAssetKey
    {
        IRouteKey ParentKey
        {
            get;
        }
        bool OverwriteTask
        {
            get;
            set;
        }
        bool IsInStack
        {
            get;
            set;
        }
        bool SkipHideAnimation
        {
            get;
            set;
        }
        bool ParallelLoad
        {
            get;
            set;
        }
        // the hide and show animation will begin at the same time
        bool ParallelTransition
        {
            get;
            set;
        }
        INode CreateNode();
        INodeContext CreateContext();
    }
}
