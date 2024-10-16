using System;
using Cr7Sund.UILifeTime;
namespace Cr7Sund.Navigation
{
    public interface ITransaction
    {
        void StartTransition(string relativeUI);
        void AddRoute(Fragment fragment, object intent);
        void RemoveRoute(Fragment fragment);
        void AddRoute(Fragment fragment);
        PromiseTask Commit(bool parallelAnimation = true);
        PromiseTask PopBack(bool parallelAnimation = true);
    }
}
