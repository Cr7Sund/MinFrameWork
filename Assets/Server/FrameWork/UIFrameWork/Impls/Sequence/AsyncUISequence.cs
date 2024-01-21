using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class AsyncUISequence : IUISequence
    {
        public UIModule Module;

        private IPromise HideFirst(IUIView exitPage)
        {
            if (exitPage != null)
            {
                Promise.Resolved();
            }

            IUIView enterPage = null;
            // IUIView enterPage=  UINode.CreateBlackScreeen();

            var handlers = new List<Func<IPromise>>();

            Func<IPromise> beforeExitHandler = () =>
                    exitPage.BeforeExit(true, enterPage);
            Func<IPromise> exitHandler = () => exitPage.Exit(true, enterPage);

            handlers.Add(beforeExitHandler);
            handlers.Add(exitHandler);
            return Promise.Sequence(handlers);
        }

        private IPromise ShowAfter(IUIView enterPage, IUIView exitPage)
        {
            var handlers = new List<Func<IPromise>>();

            Func<IPromise> beforeEnterHandler = () => enterPage.BeforeEnter(true, exitPage);
            Func<IPromise> enterHandler = () => enterPage.Enter(true, exitPage);
            Func<IPromise> afterExitHandler = null;
            if (exitPage != null)
            {
                afterExitHandler = () =>
                     exitPage.AfterExit(true, enterPage);
            }
            Func<IPromise> afterEnterHandler = () =>
                   enterPage.AfterEnter(true, exitPage);

            handlers.Add(beforeEnterHandler);
            handlers.Add(enterHandler);
            if (exitPage != null)
            {
                handlers.Add(afterExitHandler);
            }
            handlers.Add(afterEnterHandler);

            return Promise.Sequence(handlers);
        }
        public IPromise OpenAndNotCloseOthers(ViewContent content)
        {
            var openUiKey = content.uiKey;
            var exitPage = Module.GetLastView();
            
            return Module.AddNode(openUiKey)
                                            .Then((node) =>
                                                        {
                                                            var uiNode = node as UINode;
                                                            uiNode.AssignViewContent(content);
                                                            return ShowAfter(uiNode, exitPage);
                                                        });
        }
        public IPromise Open(ViewContent content)
        {
            var openUiKey = content.uiKey;
            var exitPage = Module.GetLastView();

            var sceHandler = Module.AddNode(openUiKey)
                                .Then((node) =>
                                            {
                                                var uiNode = node as UINode;
                                                uiNode.AssignViewContent(content);
                                                return ShowAfter(uiNode, exitPage);
                                            });
            var firstHandler = HideFirst(exitPage);

            return Promise.All(firstHandler, sceHandler);
        }


    }
}