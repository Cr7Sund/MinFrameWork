using System;
using System.Text.RegularExpressions;
using Cr7Sund.UGUI.Apis;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.UGUI.Impls
{
    [Serializable]
    public class UITransitionAnimation
    {
        [SerializeField] private string _partnerPageIdentifierRegex;

        [SerializeField] private UIAnimationAssetType _assetType;

        [SerializeField]
        [EnabledIf(nameof(_assetType), (int)UIAnimationAssetType.MonoBehaviour)]
        private UITransitionAnimationBehaviour _animationBehaviour;

        [SerializeField]
        [EnabledIf(nameof(_assetType), (int)UIAnimationAssetType.ScriptableObject)]
        private AssetKey _animationAsset;

        private Regex _partnerSheetIdentifierRegexCache;

        public string PartnerPageIdentifierRegex
        {
            get => _partnerPageIdentifierRegex;
            set => _partnerPageIdentifierRegex = value;
        }

        public UIAnimationAssetType AssetType
        {
            get => _assetType;
            set => _assetType = value;
        }
        public UITransitionAnimationBehaviour AnimationBehaviour { get => _animationBehaviour;  }
        public AssetKey AnimationAsset { get => _animationAsset;  }

        public bool IsValid(IAssetKey partnerUI)
        {
            if (HasConfig())
            {
                return false;
            }

            // If the partner identifier is not registered, the animation is always valid.
            if (string.IsNullOrEmpty(_partnerPageIdentifierRegex))
            {
                return true;
            }

            if (string.IsNullOrEmpty(partnerUI.Key))
            {
                return false;
            }

            if (_partnerSheetIdentifierRegexCache == null)
            {
                _partnerSheetIdentifierRegexCache = new Regex(_partnerPageIdentifierRegex);
            }

            return _partnerSheetIdentifierRegexCache.IsMatch(partnerUI.Key);
        }

        private bool HasConfig()
        {
            switch (_assetType)
            {
                case UIAnimationAssetType.MonoBehaviour:
                    return _animationBehaviour !=null;
                case UIAnimationAssetType.ScriptableObject:
                    return _animationAsset.IsValid();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}