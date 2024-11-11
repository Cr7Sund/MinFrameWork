using System.Collections.Generic;
using UnityEngine;
namespace Cr7Sund.LifeTime
{
    public class TransConfig : ScriptableObject
    {
        public List<TransitionCfgInfo> transitionConfigs;

        public TransitionCfgInfo GetConfig(string targetPage, string partnerPage)
        {
            return transitionConfigs.Find(config => config.ownerPage == targetPage
                                                    && config.relativePage == partnerPage);
        }
    }
}