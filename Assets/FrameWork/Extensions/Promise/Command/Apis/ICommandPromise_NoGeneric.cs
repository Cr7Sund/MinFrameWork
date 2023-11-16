using System;
using System.Collections.Generic;

namespace Cr7Sund.Framework.Api
{
    public interface ICommandPromise : IPromise, ISequence, IPoolable
    {
        ICommandPromise Then<T>() where T : IPromiseCommand, new();
        ICommandPromise Then(ICommandPromise resultPromise, IPromiseCommand promiseCommand);
        ICommandPromise ThenAll(IEnumerable<ICommandPromise> promises, IEnumerable<IPromiseCommand> commands);
        ICommandPromise ThenAny(IEnumerable<ICommandPromise> promises, IEnumerable<IPromiseCommand> commands);
        ICommandPromise ThenRace(IEnumerable<ICommandPromise> promises, IEnumerable<IPromiseCommand> commands);
        void Execute();
        void Progress(float progress);
        void Catch(Exception e);


    }
}