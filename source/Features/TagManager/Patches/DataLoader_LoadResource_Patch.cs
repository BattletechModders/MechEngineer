using Harmony;
using HBS.Data;
using System;
using System.Diagnostics;

namespace MechEngineer.Features.TagManager.Patches
{
    //[HarmonyPatch(typeof(DataLoader), nameof(DataLoader.LoadResource), typeof(string), typeof(Action<string>))]
    public static class DataLoader_LoadResource_string_Patch
    {
        public static readonly Benchmark benchmark = new Benchmark(typeof(DataLoader_LoadResource_string_Patch));

        public static void Prefix()
        {
            benchmark.Prefix();
        }
        public static void Postfix()
        {
             benchmark.Postfix();
        }
    }

    public class Benchmark
    {
        readonly Stopwatch stopwatch = new Stopwatch();
        int count = 0;

        private Type type;
        internal Benchmark(Type type)
        {
            this.type = type;
        }

        internal void Prefix()
        {
            ++count;
            stopwatch.Start();
        }
        internal void Postfix()
        {
            stopwatch.Stop();
        }

        private void Print()
        {
            var timeMS = stopwatch.ElapsedMilliseconds;
            var avgMS = timeMS / count;
            Control.mod.Logger.LogError($"BENCHMARK {type} time={timeMS}ms count={count} avg={avgMS}");
        }

        public static void PrintBenchmarks()
        {
            DataLoader_LoadResource_string_Patch.benchmark.Print();
        }
    }
}
