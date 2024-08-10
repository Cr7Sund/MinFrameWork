using System;

namespace Cr7Sund
{
    public interface IInitialize : IDisposable
    {
        bool IsInit { get; }

        void Init();
    }
}
