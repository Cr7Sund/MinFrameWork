using Cr7Sund.Package.Api;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.Selector.Apis
{
    public interface IGameLogic
    {
        void Init();
        void Start();
        void Update(int millisecond);
        void LateUpdate(int millisecond);
        IPromise<INode> Destroy();
    }
}