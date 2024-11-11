using UnityEngine;
namespace Cr7Sund.LifeTime
{
    [System.Serializable]
    public struct TransitionCfgInfo
    {
        [SerializeField] public string ownerPage;
        [SerializeField] public string relativePage;
        [SerializeField] public string pushEnterAnimations;
        [SerializeField] public string pushExitAnimations;
        [SerializeField] public string popEnterAnimations;
        [SerializeField] public string popExitAnimations;

    }

}
