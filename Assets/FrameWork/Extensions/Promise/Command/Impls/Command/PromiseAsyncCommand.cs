﻿using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;

namespace Cr7Sund.Framework.Impl
{
    public abstract class PromiseAsyncCommand<PromisedT> : PromiseCommand<PromisedT>, IPromiseAsyncCommand<PromisedT>
    {
        public sealed override PromisedT OnExecute(PromisedT value)
        {
            throw new NotImplementedException();
        }

        public virtual IPromise<PromisedT> OnCatchAsync(Exception ex) { return null; }

        public abstract IPromise<PromisedT> OnExecuteAsync(PromisedT value);

    }

    public abstract class PromiseAsyncCommand<PromisedT, ConvertedT> : PromiseCommand<PromisedT, ConvertedT>, IPromiseAsyncCommand<PromisedT, ConvertedT>
    {
        public sealed override ConvertedT OnExecute(PromisedT value)
        {
            throw new NotImplementedException();
        }

        public abstract IPromise<ConvertedT> OnExecuteAsync(PromisedT value);
        public virtual IPromise<ConvertedT> OnCatchAsync(Exception ex) { return null; }

    }

}