using Cr7Sund.UGUI.Apis;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Cr7Sund.UGUI.Impls
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public partial class UIPanel : UIBehaviour, IUIPanel
    {
        [SerializeField] private UITransitionAnimation _pushEnterAnimations;
        [SerializeField] private UITransitionAnimation _pushExitAnimations;
        [SerializeField] private UITransitionAnimation _popEnterAnimations;
        [SerializeField] private UITransitionAnimation _popExitAnimations;
        [SerializeField] private StringBehaviourDictionary _componentContainers;


        public bool IsInit { get; }
        public StringBehaviourDictionary ComponentContainers { get => _componentContainers; }

        public void Dispose()
        {

            throw new System.NotImplementedException();
        }

        public T GetUIComponent<T>(string key) where T : Behaviour
        {
            if (!_componentContainers.ContainsKey(key))
            {
                Console.Error($"Panel {name} don't bind {key}");
                return default;
            }
            return _componentContainers[key] as T;
        }

        public UITransitionAnimation GetAnimation(bool push, bool enter, IAssetKey partnerTransitionUI)
        {
            UITransitionAnimation animation = null;
            if (push)
            {
                animation = enter ? _pushEnterAnimations : _pushExitAnimations;
            }

            animation = enter ? _popEnterAnimations : _popExitAnimations;

            if (!animation.IsValid(partnerTransitionUI))
            {
                return null;
            }

            return animation;
        }

        public void Init()
        {

        }

    }
}