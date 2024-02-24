using System;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Package.Api;
using Cr7Sund.UGUI.Apis;
using UnityEngine;

/**
 * @class Cr7Sund.Framework.Api.IView
 *
 * Monobehaviours must implement this interface in order to be injectable.
 *
 * To contact the Context, the View must be able to find it. View handles this
 * with bubbling.
 */
namespace Cr7Sund.Server.UI.Api
{
    public interface IUIView : IDisposable, IUpdatable
    {
        int SortingOrder { get; }
        RectTransform RectTransform { get; }
        /// <summary>
        ///     Progress of the transition animation.
        /// </summary>
        public float TransitionAnimationProgress { get; }

        void Start(UnityEngine.Object asset, INode parent);
        void Enable(INode parent);
        void BeforeEnter();
        IPromise EnterRoutine(bool push, IUINode partnerView, bool playAnimation);
        void AfterEnter();
        void BeforeExit();
        IPromise ExitRoutine(bool push, IUINode partnerView, bool playAnimation);
        void AfterExit();
        void Stop();
    }
}
