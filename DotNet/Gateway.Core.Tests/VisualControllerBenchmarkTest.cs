using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace Gateway.Core.Tests
{
    // A mock version of the Unity classes to benchmark the dictionary vs list lookup
    // and the cost of property name hashing that we are trying to optimize.

    public class VisualControllerBenchmarkTest
    {
        private readonly ITestOutputHelper output;

        public VisualControllerBenchmarkTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void BenchmarkStringVsIntPropertyLookup()
        {
            // Simulate the lookup operations
            var stringLookup = new Dictionary<string, float>();
            var intLookup = new Dictionary<int, float>();

            // In Unity, Shader.PropertyToID takes a string and gives an int (which is just a hash)
            // Material.SetFloat(string, float) internally does the hash every time
            // Material.SetFloat(int, float) avoids the hash

            string propName = "_Glossiness";
            int propId = propName.GetHashCode(); // Simulate Shader.PropertyToID

            int iterations = 10000000;

            // Warmup
            SimulateStringLookup(propName, stringLookup);
            SimulateIntLookup(propId, intLookup);

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                SimulateStringLookup(propName, stringLookup);
            }
            sw.Stop();
            long stringTime = sw.ElapsedMilliseconds;

            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                SimulateIntLookup(propId, intLookup);
            }
            sw.Stop();
            long intTime = sw.ElapsedMilliseconds;

            output.WriteLine($"String lookup: {stringTime} ms");
            output.WriteLine($"Int lookup: {intTime} ms");
            output.WriteLine($"Improvement: {stringTime - intTime} ms ({(double)(stringTime - intTime) / stringTime * 100:F2}%)");
        }

        private void SimulateStringLookup(string key, Dictionary<string, float> dict)
        {
            // Simulate what Unity does: hash the string, then lookup
            int hash = key.GetHashCode();
            dict[key] = 1.0f;
        }

        private void SimulateIntLookup(int key, Dictionary<int, float> dict)
        {
            // Unity already has the int, just look it up
            dict[key] = 1.0f;
        }
    }
}
