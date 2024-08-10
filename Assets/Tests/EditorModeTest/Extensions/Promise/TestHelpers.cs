using NUnit.Framework;
using System;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
namespace Cr7Sund.PackageTest.PromiseTest
{
    public static class TestHelpers
    {
        /// <summary>
        ///     Helper function that checks that the given action doesn't trigger the
        ///     unhandled exception handler.
        /// </summary>
        public static void VerifyDoesntThrowUnhandledException(Action testAction)
        {
            Exception unhandledException = null;
            EventHandler<ExceptionEventArgs> handler =
                (sender, args) => unhandledException = args.Exception;
            Promise.UnhandledException += handler;

            try
            {
                testAction();

                Assert.Null(unhandledException);
            }
            finally
            {
                Promise.UnhandledException -= handler;
            }
        }
    }
}
