using System;

namespace Cr7Sund.Framework.Api
{
    public interface IBaseCommand : IPoolable
    {
        void OnCatch(Exception e);
        void OnProgress(float progress);

    }

}