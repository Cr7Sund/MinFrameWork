using System;

namespace Cr7Sund.Framework.Api
{
    public interface IPromiseCommand : IPromise, IPoolable
    {
        IPromiseCommand Then<T>() where T : IPromiseCommand, new();
        IPromiseCommand Then(IPromiseCommand promise);

        void Execute();
        void Progress(float progress);
        void Catch(Exception e);

        float SliceLength {get; set;}
        int SequenceID { get; set; }
    }
}