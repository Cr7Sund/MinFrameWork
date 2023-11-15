using System;

namespace Cr7Sund.Framework.Api
{
    public interface IBaseCommand
    {
        void OnCatch(Exception e);
        void OnProgress(float progress);

    }

}