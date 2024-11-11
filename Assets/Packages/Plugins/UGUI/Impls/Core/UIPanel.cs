using System;
using Cr7Sund.UGUI.Apis;
using Cr7Sund.UGUI.Impls.Cr7Sund.UGUI.Impls;
using UnityEngine;
namespace Cr7Sund.UGUI.Impls
{
    namespace Cr7Sund.UGUI.Impls
    {
        [Serializable]
        public class BindBehaviourDict : SerializableDictionary<string, Behaviour>
        {

        }
    }
    
    public class UIPanel : MonoBehaviour, IUIComponentGetter
    {
        [SerializeField] private  BindBehaviourDict _componentContainers;

        public BindBehaviourDict ComponentContainers { get => _componentContainers; }

        public T GetUIComponent<T>(string key) where T : Component
        {
            if (!_componentContainers.ContainsKey(key))
            {
                Console.Error("Panel {Name} don't bind {Key}", name, key);
                return default;
            }
            return _componentContainers[key] as T;
        }
    }
}
