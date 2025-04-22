using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LoopBenchmarks
{
    class Program
    {
        // Adjusted parameters for more pronounced performance differences
        private const int CollectionSize = 1_000_000;
        private const int IterationsPerTest = 5;
        private const bool UseCpuIntensiveOperation = true;
        private const bool UseMemoryIntensiveOperation = true;
        private const int InnerLoopIterations = 100; // Controls CPU intensity
        private const int MemoryChunkSize = 4096;   // Controls memory intensity
        
        static void Main(string[] args)
        {
            Console.WriteLine("Preparing benchmark data...");
            var data = Enumerable.Range(1, CollectionSize).ToList();
            
            Console.WriteLine($"Benchmarking with collection size: {CollectionSize:N0}");
            Console.WriteLine($"CPU intensive operations: {UseCpuIntensiveOperation} (Iterations: {InnerLoopIterations})");
            Console.WriteLine($"Memory intensive operations: {UseMemoryIntensiveOperation} (Chunk size: {MemoryChunkSize} bytes)");
            Console.WriteLine($"Running each test {IterationsPerTest} times\n");
            
            // Run multiple iterations of each test to get more accurate results
            var forResults = new List<long>();
            var foreachResults = new List<long>();
            var linqResults = new List<long>(); 
            var foreachMethodResults = new List<long>();
            var parallelResults = new List<long>();
            var plinqResults = new List<long>();
            
            Console.WriteLine("Running benchmarks...");
            
            for (int i = 0; i < IterationsPerTest; i++)
            {
                Console.WriteLine($"\nIteration {i + 1} of {IterationsPerTest}:");
                
                // Clear the results of previous operations to avoid memory impact on subsequent tests
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(500);
                
                // Benchmark For loop
                var forTime = BenchmarkForLoop(data);
                forResults.Add(forTime);
                Console.WriteLine($"  For loop:           {forTime,10:N0} ms");
                
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(500);
                
                // Benchmark ForEach loop
                var foreachTime = BenchmarkForEachLoop(data);
                foreachResults.Add(foreachTime);
                Console.WriteLine($"  ForEach loop:       {foreachTime,10:N0} ms");
                
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(500);
                
                // Benchmark LINQ
                var linqTime = BenchmarkLinq(data);
                linqResults.Add(linqTime);
                Console.WriteLine($"  LINQ:               {linqTime,10:N0} ms");
                
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(500);
                
                // Benchmark List.ForEach method
                var foreachMethodTime = BenchmarkForEachMethod(data);
                foreachMethodResults.Add(foreachMethodTime);
                Console.WriteLine($"  List.ForEach:       {foreachMethodTime,10:N0} ms");
                
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(500);
                
                // Benchmark Parallel.ForEach loop
                var parallelTime = BenchmarkParallelForEachLoop(data);
                parallelResults.Add(parallelTime);
                Console.WriteLine($"  Parallel.ForEach:   {parallelTime,10:N0} ms");
                
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(500);
                
                // Benchmark PLINQ
                var plinqTime = BenchmarkPLinq(data);
                plinqResults.Add(plinqTime);
                Console.WriteLine($"  PLINQ:              {plinqTime,10:N0} ms");
            }
            
            // Find the baseline (slowest approach) for relative comparisons
            double baselineTime = new[] { 
                forResults.Average(), 
                foreachResults.Average(), 
                linqResults.Average(),
                foreachMethodResults.Average(),
                parallelResults.Average(),
                plinqResults.Average()
            }.Max();
            
            // Calculate and display average results
            Console.WriteLine("\n=== RESULTS SUMMARY ===");
            DisplayResults("For loop", forResults, baselineTime);
            DisplayResults("ForEach loop", foreachResults, baselineTime);
            DisplayResults("LINQ", linqResults, baselineTime);
            DisplayResults("List.ForEach", foreachMethodResults, baselineTime);
            DisplayResults("Parallel.ForEach", parallelResults, baselineTime);
            DisplayResults("PLINQ", plinqResults, baselineTime);
            
            // Calculate speedup ratios between approaches
            Console.WriteLine("\n=== SPEED COMPARISONS ===");
            ComputeSpeedupRatio("For loop", forResults.Average(), "Parallel.ForEach", parallelResults.Average());
            ComputeSpeedupRatio("ForEach loop", foreachResults.Average(), "Parallel.ForEach", parallelResults.Average());
            ComputeSpeedupRatio("LINQ", linqResults.Average(), "PLINQ", plinqResults.Average());
            ComputeSpeedupRatio("For loop", forResults.Average(), "ForEach loop", foreachResults.Average());
            
            // Calculate and display hardware utilization efficiency
            Console.WriteLine("\n=== HARDWARE UTILIZATION ===");
            int processorCount = Environment.ProcessorCount;
            double theoreticalMaxSpeedup = processorCount;
            double actualParallelSpeedup = forResults.Average() / parallelResults.Average();
            double parallelEfficiency = (actualParallelSpeedup / theoreticalMaxSpeedup) * 100;
            
            Console.WriteLine($"Processor count: {processorCount}");
            Console.WriteLine($"Theoretical maximum speedup: {theoreticalMaxSpeedup:F2}x");
            Console.WriteLine($"Actual Parallel.ForEach speedup: {actualParallelSpeedup:F2}x");
            Console.WriteLine($"Parallel efficiency: {parallelEfficiency:F2}% of theoretical maximum");
            
            Console.WriteLine("\nBenchmark complete. Press any key to exit.");
            Console.ReadKey();
        }
        
        static void DisplayResults(string testName, List<long> results, double baselineTime)
        {
            double avg = results.Average();
            double relativePerformance = (baselineTime / avg) * 100;
            Console.WriteLine($"{testName,-18} {avg,10:N0} ms average ({string.Join(", ", results)} ms) - {relativePerformance:F2}% relative performance");
        }
        
        static void ComputeSpeedupRatio(string slower, double slowerTime, string faster, double fasterTime)
        {
            double ratio = slowerTime / fasterTime;
            if (ratio < 1)
            {
                // If the "slower" is actually faster
                Console.WriteLine($"{slower} is {1/ratio:F2}x faster than {faster}");
            }
            else
            {
                Console.WriteLine($"{faster} is {ratio:F2}x faster than {slower}");
            }
        }
        
        static long BenchmarkForLoop(List<int> data)
        {
            var stopwatch = Stopwatch.StartNew();
            long result = 0;
            
            for (int i = 0; i < data.Count; i++)
            {
                result += ProcessItem(data[i]);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        static long BenchmarkForEachLoop(List<int> data)
        {
            var stopwatch = Stopwatch.StartNew();
            long result = 0;
            
            foreach (var item in data)
            {
                result += ProcessItem(item);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        static long BenchmarkLinq(List<int> data)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var result = data.Select(item => ProcessItem(item)).Sum();
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        static long BenchmarkForEachMethod(List<int> data)
        {
            var stopwatch = Stopwatch.StartNew();
            long result = 0;
            
            data.ForEach(item => {
                result += ProcessItem(item);
            });
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        static long BenchmarkParallelForEachLoop(List<int> data)
        {
            var stopwatch = Stopwatch.StartNew();
            long result = 0;
            object lockObj = new object();
            
            Parallel.ForEach(data, item =>
            {
                long localResult = ProcessItem(item);
                
                // Thread-safe aggregation of results
                if (localResult != 0)
                {
                    lock (lockObj)
                    {
                        result += localResult;
                    }
                }
            });
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        static long BenchmarkPLinq(List<int> data)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var result = data.AsParallel().Select(item => ProcessItem(item)).Sum();
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        static long ProcessItem(int item)
        {
            long result = item;
            
            // CPU-bound operation
            if (UseCpuIntensiveOperation)
            {
                // More intensive CPU work
                for (int i = 0; i < InnerLoopIterations; i++)
                {
                    result = (result * 31 + i) % 997;
                    // Add more complex calculation to better simulate real CPU work
                    double sinResult = Math.Sin(result * 0.01);
                    result += (long)(sinResult * 10);
                }
            }
            
            // Memory-bound operation
            if (UseMemoryIntensiveOperation)
            {
                // More realistic memory-intensive work
                byte[] buffer = new byte[MemoryChunkSize];
                for (int i = 0; i < Math.Min(buffer.Length, 1000); i++)
                {
                    buffer[i] = (byte)(result & 0xFF);
                    result = (result * 7 + 1) % int.MaxValue;
                }
                
                // Perform some operations on the buffer to prevent JIT optimization
                for (int i = 0; i < Math.Min(buffer.Length, 100); i++)
                {
                    if (i % 10 == 0)
                    {
                        result ^= buffer[i];
                    }
                }
            }
            
            return result;
        }
    }
}
