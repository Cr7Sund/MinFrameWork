


using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Framework.Api;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.Server.UI.Api
{
    public interface IUIModule
    {

        /// <summary>
        /// 当前聚焦UI
        /// </summary>
        INode FocusUI { get; }

        /// <summary>
        /// 当前父节点下正在运行的UINode数量(非多实例)
        /// </summary>
        byte OperateNum { get; }


        void OpenModal(IAssetKey uiKey);
        void OpenTab(IAssetKey uiKey);
        /// <summary>
        /// 回退到上一个Open的Panel,注意回退到上一个Open的界面，对于Close的界面将不会回退
        /// </summary>
        void Back();
        void Back(int popCount = 1);
        void Back(IAssetKey popKey);

        /// <summary>
        /// 关闭一个UI
        /// </summary>
        /// <param name="uiKey">需要关闭的UIKey</param>
        void Close(IAssetKey uiKey);

        /// <summary>
        /// 判断一个UI是否显隐
        /// </summary>
        /// <param name="uiKey">需要查询的UIKey</param>
        /// <returns></returns>
        bool IsVisible(IAssetKey uiKey);


        // Modal : When it is displayed, all interactions except for the foreground modal will be blocked.

        // IPromise<int> Show(UIKey key, bool skipStack);
        // IPromise<int> ShowAsync(UIKey key, bool skipStack);
        // IPromise<int> ShowModal(UIKey key);
        // IPromise<int> ShowPopup(UIKey key);
        // void Hide(UIKey key);

        void Freeze();
        void UnFreeze();

        IUIView GetLastView();
    }
}
