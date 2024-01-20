using UnityEngine.EventSystems;

namespace Cr7Sund.UGUI.Apis
{
    public class UIPanel : UIBehaviour, IUIPanel
    {
        public bool IsInit => throw new System.NotImplementedException();

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public T GetUIComponent<T>(string key) where T : UIBehaviour
        {
            throw new System.NotImplementedException();
        }

        public void Init()
        {
            throw new System.NotImplementedException();
        }
    }
}