using Cr7Sund.Package.Api;
using Cr7Sund.UGUI.Apis;
using UnityEngine.EventSystems;

namespace Cr7Sund.Vfx.Api
{
    public interface IVfxPlayer
    {
        /// <summary>
        /// 加载播放UIFX
        /// </summary>
        /// <param name="addressableName">资源Key</param>
        /// <param name="parent">父物体</param>
        /// <param name="callback">异步加载回调</param>
        IPromise<IVfxElement> PlayFX(string addressableName, UIBehaviour parent);
        /// <summary>
        /// 加载播放UIMotion
        /// </summary>
        /// <param name="addressableName">资源Key</param>
        /// <param name="parent">父物体</param>
        /// <param name="callback">异步加载回调</param>
        IPromise<IVfxElement> PlayMotion(string addressableName, UIBehaviour parent);
        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="playable"></param>
        void Recycle(IVfxElement playable);
    }
}