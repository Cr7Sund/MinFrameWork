using UnityEngine.EventSystems;

namespace Cr7Sund.UGUI.Apis
{
    public interface IUIComponentGetter
    {
        T GetUIComponent<T>(string key) where T : UIBehaviour;
    }

}