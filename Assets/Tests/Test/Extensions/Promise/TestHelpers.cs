using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using NUnit.Framework;
using System;
namespace Cr7Sund.Framework.PromiseTest
{
    internal static class TestHelpers
    {
        /// <summary>
        ///     Helper function that checks that the given action doesn't trigger the
        ///     unhandled exception handler.
        /// </summary>
        internal static void VerifyDoesntThrowUnhandledException(Action testAction)
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
