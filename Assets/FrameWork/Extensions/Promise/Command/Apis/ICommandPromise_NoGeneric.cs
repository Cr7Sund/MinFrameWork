using System;
using System.Collections.Generic;

namespace Cr7Sund.Framework.Api
{
    public interface ICommandPromise : IPromise, ISequence, IPoolable
    {
        ICommandPromise Then<T>() where T : ICommand, new();
        ICommandPromise Then(ICommandPromise resultPromise, ICommand command);
        ICommandPromise ThenAll(IEnumerable<ICommandPromise> promises, IEnumerable<ICommand> commands);
        ICommandPromise ThenAny(IEnumerable<ICommandPromise> promises, IEnumerable<ICommand> commands);
        ICommandPromise ThenRace(IEnumerable<ICommandPromise> promises, IEnumerable<ICommand> commands);
        void Execute();
        void Progress(float progress);
        void Catch(Exception e);


    }
}