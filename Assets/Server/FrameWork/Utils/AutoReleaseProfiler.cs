using System;
using UnityEngine.Profiling;

namespace Cr7Sund.Server.Utils
{
    public class AutoReleaseProfiler : IDisposable
    {
        public AutoReleaseProfiler(string name)
        {
            Profiler.BeginSample(name);
        }
        public void Dispose()
        {
            Profiler.EndSample();
        }
    }
}