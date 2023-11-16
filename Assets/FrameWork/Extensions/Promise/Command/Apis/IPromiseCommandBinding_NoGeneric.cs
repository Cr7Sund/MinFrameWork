using System.Collections.Generic;

namespace Cr7Sund.Framework.Api
{
    public interface IPromiseCommandBinding : IBinding
    {
        bool UsePooling { get; set; }

        IPromiseCommandBinding Then<T>() where T : class, IPromiseCommand, new();


        IPromiseCommandBinding ThenAny(params IPromiseCommand[] commands);
        IPromiseCommandBinding ThenAny<T1, T2>()
             where T1 : class, IPromiseCommand, new()
             where T2 : class, IPromiseCommand, new();
        IPromiseCommandBinding ThenAny<T1, T2, T3>()
             where T1 : class, IPromiseCommand, new()
             where T2 : class, IPromiseCommand, new()
             where T3 : class, IPromiseCommand, new();

        IPromiseCommandBinding ThenRace(params IPromiseCommand[] commands);
        IPromiseCommandBinding ThenRace<T1, T2>()
             where T1 : class, IPromiseCommand, new()
             where T2 : class, IPromiseCommand, new();
        IPromiseCommandBinding ThenRace<T1, T2, T3>()
             where T1 : class, IPromiseCommand, new()
             where T2 : class, IPromiseCommand, new()
             where T3 : class, IPromiseCommand, new();
    }
}