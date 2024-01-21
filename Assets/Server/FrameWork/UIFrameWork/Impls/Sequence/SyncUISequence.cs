using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class SyncUISequence : IUISequence
    {
        public UIModule Module;


        public IPromise Open(ViewContent content)
        {
            var openUiKey = content.uiKey;

            return Module.AddNode(openUiKey)
                        .Then((node) =>
                            {
                                var enterPage = node as UINode;
                                var exitPage = Module.GetLastView();
                                enterPage.AssignViewContent(content);
                                return OpenSequence(enterPage, exitPage);
                            });
        }


        private IPromise OpenSequence(IUIView enterPage, IUIView exitPage)
        {
            var handlers = new List<Func<IPromise>>();

            Func<IPromise> beforeEnterHandler = () => enterPage.BeforeEnter(true, exitPage);
            Func<IPromise> beforeExitHandler = null;
            if (exitPage != null)
            {
                beforeExitHandler = () =>
                    exitPage.BeforeExit(true, enterPage);
            }
            Func<IPromise> exitHandler = () => exitPage.Exit(true, enterPage);
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
            if (exitPage != null)
            {
                handlers.Add(beforeExitHandler);
            }
            handlers.Add(exitHandler);
            handlers.Add(enterHandler);
            if (exitPage != null)
            {
                handlers.Add(afterExitHandler);
            }
            handlers.Add(afterEnterHandler);

            return Promise.Sequence(handlers);
        }

    }
}