using Cr7Sund.Package.Impl;
using UnityEngine;
namespace Cr7Sund.LifeTime
{
    public class AnimationTransition : MonoBehaviour, ITransition
    {
        [SerializeField] private Animation _animation;
        public async PromiseTask Show(IRouteKey targetPage, string partnerPage, bool push)
        {
            _animation.Play();
        }
        public async PromiseTask Hide(IRouteKey targetPage, string partnerPage, bool push)
        {
            _animation.Stop();
        }
    }
}
