using Cr7Sund;
using UnityEngine;
namespace Cr7Sund.UILifeTime
{
    public interface IUIView 
    {
        PromiseTask Load(string panelID, UnsafeCancellationToken cancellationToken, RectTransform attachPoint);
        RectTransform FindAttachPoint(string name);
        bool IsValid();
    }

}
