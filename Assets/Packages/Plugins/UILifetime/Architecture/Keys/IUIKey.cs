using System;
namespace Cr7Sund.LifeTime
{
    public interface IUIKey : IRouteKey
    {
        bool UniqueInstance
        {
            get;
        }
        
        string PanelID
        {
            get;
        }
    }
}