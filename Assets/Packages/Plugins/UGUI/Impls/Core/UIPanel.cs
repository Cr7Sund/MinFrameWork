using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
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

        }

        public void Hide(bool push)
        {

        }

        public IPromise Animate(bool push)
        {
            // why we dont animate the view in controller, such as tween
            // we want separate view from controller
            // which means the controller don't depends on specific view
            // check this link 
            // https://www.notion.so/UI-FrameWork-89d22f2f2c894171bf023b056be177ce?d=be45aab0adcc4151965d8524c2445efa&pvs=4#39a09f1482394541b255c4b97f741174
            return Promise.Resolved();
        }

        public void Show(bool push)
        {

        }
    }
}