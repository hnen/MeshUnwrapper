#define ENABLE_PROFILER

using System;

namespace MeshUnwrapper.Unity
{
    
    static class Profiler
    {
        class ProfilerRegion : IDisposable
        {
            string name;

            public ProfilerRegion(string name)
            {
                this.name = name;
                UnityEngine.Profiling.Profiler.BeginSample(name);
            }


            public void Dispose()
            {
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }


        public static IDisposable Profile(string name) {
            return new ProfilerRegion(name);
        }
    }
}
