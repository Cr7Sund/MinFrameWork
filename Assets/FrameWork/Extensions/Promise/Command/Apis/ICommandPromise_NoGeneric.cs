using System;

namespace Cr7Sund.Framework.Api
{
    public interface ICommandPromise : IPromise, ISequence, IPoolable
    {
        ICommandPromise Then<T>() where T : IPromiseCommand, new();
        ICommandPromise Then(ICommandPromise resultPromise, IPromiseCommand promiseCommand);

        void Execute();
        void Progress(float progress);
        void Catch(Exception e);


    }
}