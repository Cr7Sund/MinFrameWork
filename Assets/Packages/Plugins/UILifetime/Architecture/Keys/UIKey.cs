using System;
namespace Cr7Sund.LifeTime
{
    public class UIKey : RouteKey, IUIKey
    {
        public bool UniqueInstance
        {
            get;
            private set;
        }
        public string PanelID
        {
            get;
            private set;
        }


        public UIKey(string uiAssetKey, Type uiCtrlType, Type uiContextType = null):base(uiAssetKey, uiCtrlType,uiContextType)
        {
  
        }
    }
}
