using BattleTech.UI;
using Harmony;
using HBS.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using System.IO.Compression;
//using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace MechEngineer.Features.TagManager.Patches
{
    //[HarmonyPatch(typeof(MainMenu), "ReceiveButtonPress")]
    public static class MainMenu_ReceiveButtonPress_Patch
    {
        public static bool Prefix(string button)
        {
            if (button == "Credits")
            {
                DataLoader_LoadResource_string_Patch.Save();
                Benchmark.PrintBenchmarks();
                return false;
            }
            return true;
        }
    }

    //[HarmonyPatch(typeof(DataLoader), nameof(DataLoader.LoadResource), typeof(string), typeof(Action<string>))]
    public static class DataLoader_LoadResource_string_Patch
    {
        public static bool Prefix(string path, ref Action<string> handler)
        {
            try
            {
                if (GetCache(path, out var text))
                {
                    handler(text);
                    return false;
                }
                else
                {
                    var originalHandler = handler;
                    handler = delegate (string result)
                    {
                        SetCache(path, result);
                        originalHandler(result);
                    };
                    return true;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return true;
        }

        public static readonly Benchmark benchmarkd = new Benchmark("Decompress");
        private static bool GetCache(string key, out string text)
        {
            try
            {
                if (cache.TryGetValue(key, out text))
                {
                    benchmarkd.Prefix();
                    //text = Decompress(data);
                    benchmarkd.Postfix();
                    return true;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            text = null;
            return false;
        }
        
        public static readonly Benchmark benchmarkc = new Benchmark("Compress");
        private static void SetCache(string key, string text)
        {
            try
            {
                benchmarkc.Prefix();
                cache[key] = text; //Compress(text);
                benchmarkc.Postfix();
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
        
        static DataLoader_LoadResource_string_Patch()
        {
            if (File.Exists(CacheFilePath))
            {
                using (var fileStream = new FileStream("D:\\test.tmp", FileMode.Open))
                {
                    cache = null; // formatter.Deserialize(fileStream) as Dictionary<string, string>;
                }
            }
            else
            {
                cache = new Dictionary<string, string>();
                Save();
            }
        }

        private static readonly string CacheFilePath = "D:\\test.tmp";
        private static readonly Dictionary<string, string> cache;
        //private static readonly BinaryFormatter formatter = new BinaryFormatter();
        public static void Save()
        {
            using (var fileStream = new FileStream(CacheFilePath, FileMode.Create))
            {
                //formatter.Serialize(fileStream, cache);
            }
        }

        public static byte[] Compress(string text)
        {
            return Encoding.UTF8.GetBytes(text);

            //// Deflate
            //using (var textByteStream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            //{
            //    using (var compressedStream = new MemoryStream((int)textByteStream.Length))
            //    {
            //        using (var compressingStream = new DeflateStream(compressedStream, CompressionLevel.Fastest))
            //        {
            //            textByteStream.CopyTo(compressedStream);
            //        }
            //        return compressedStream.ToArray();
            //    }
            //}

            //// LZ4
            //var textBytes = Encoding.UTF8.GetBytes(text);
            //var compressedBufferBytesLength = LZ4Codec.MaximumOutputSize(textBytes.Length);
            //var compressedBufferBytes = new byte[compressedBufferBytesLength];

            //var compressedLength = LZ4Codec.Encode(textBytes.AsSpan(), compressedBufferBytes.AsSpan(), LZ4Level.L09_HC);
            
            //var textLengthBytes = BitConverter.GetBytes(textBytes.Length);

            //// lengthBytes has to be 4!
            //var data = new byte[4 + compressedLength];

            //Buffer.BlockCopy(textLengthBytes, 0, data, 0, 4);
            //Buffer.BlockCopy(compressedBufferBytes, 0, data, 4, compressedLength);

            //return data;
        }

        private static string Decompress(byte[] data)
        {
            return Encoding.UTF8.GetString(data);

            //// Deflate
            //using (var compressedStream = new MemoryStream(data))
            //{
            //    using (var decompressingStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            //    {
            //        using (var textByteStream = new MemoryStream(2 * (int)compressedStream.Length))
            //        {
            //            decompressingStream.CopyTo(textByteStream);
            //            return Encoding.UTF8.GetString(textByteStream.ToArray());
            //        }
            //    }
            //}

            //// LZ4
            //var textLength = BitConverter.ToInt32(data, 0);
            //var textBytes = new byte[textLength];
            //LZ4Codec.Decode(data.AsSpan(4), textBytes.AsSpan());

            //var text = Encoding.UTF8.GetString(textBytes);

            //return text;
        }
    }

    //[HarmonyPatch(typeof(DataLoader), nameof(DataLoader.LoadResource), typeof(string), typeof(Action<string>))]
    public static class DataLoader_LoadResource_string_Benchmark
    {
        public static readonly Benchmark benchmark = new Benchmark(nameof(DataLoader_LoadResource_string_Patch));
        
        [HarmonyPriority(Priority.First)]
        public static void Prefix()
        {
            benchmark.Prefix();
        }

        [HarmonyPriority(Priority.Last)]
        public static void Postfix()
        {
            benchmark.Postfix();
            Benchmark.PrintBenchmarks();
        }
    }

    public class Benchmark
    {
        readonly Stopwatch stopwatch = new Stopwatch();
        int count = 0;

        private string id;
        internal Benchmark(string id)
        {
            this.id = id;
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
            var avgMS = count == 0 ? "-" : (timeMS / count).ToString();
            Control.mod.Logger.LogError($"BENCHMARK id={id} time={timeMS}ms count={count} avg={avgMS}");
        }

        public static void PrintBenchmarks()
        {
            DataLoader_LoadResource_string_Patch.benchmarkc.Print();
            DataLoader_LoadResource_string_Patch.benchmarkd.Print();
            DataLoader_LoadResource_string_Benchmark.benchmark.Print();
        }
    }
}
