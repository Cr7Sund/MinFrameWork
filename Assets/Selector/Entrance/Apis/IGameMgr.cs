using Cr7Sund.Package.Api;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.Selector.Api
{
    internal interface IGameMgr
    {
        void Start();
        PromiseTask Close();
        PromiseTask Restart();
    }
}