## Loop Benchmark
A C# benchmarking tool for comparing the performance of different looping constructs and parallel processing techniques.

### Overview
Benchemark provides quantitative measurements of various C# iteration techniques, including traditional loops, LINQ operations, and parallel processing approaches. It runs CPU and memory-intensive operations on large datasets to produce meaningful performance comparisons.

### Features
- Benchmarks 6 different looping constructs:
  - Traditional for loop
  - foreach loop
  - LINQ queries
  - List.ForEach method
  - Parallel.ForEach loop
  - PLINQ (Parallel LINQ)
- Configurable test parameters
- CPU and memory intensive operations
- Detailed performance metrics
- Hardware utilization analysis

### Configuration Options

The benchmark behavior can be customized by modifying these constants in the Program.cs file:
```chsarp
private const int CollectionSize = 1_000_000;        // Number of items to process
private const int IterationsPerTest = 5;             // How many times each test runs
private const bool UseCpuIntensiveOperation = true;  // Enable CPU-intensive operations
private const bool UseMemoryIntensiveOperation = true; // Enable memory-intensive operations
private const int InnerLoopIterations = 100;         // Controls CPU workload intensity
private const int MemoryChunkSize = 4096;   
```

### How It Works
- The application creates a test collection of integers
- Each looping construct processes the collection multiple times
- The time taken for each iteration is measured using Stopwatch
- Between tests, garbage collection and pauses are used to minimize interference
- Results are aggregated and analyzed
- Output Metrics

### Benchemark provides detailed performance analysis:

- Average execution time for each approach
- Relative performance compared to the slowest method
- Speed comparisons between approaches (e.g., "PLINQ is 3.25x faster than LINQ")
- Hardware utilization efficiency
- Theoretical vs. actual parallel speedup
- Running the Benchmark

### To run the benchmark:

- Open the solution in Visual Studio
- Build the project using the Debug or Release configuration
- Run the application
- Review the results in the console output

### Requirements
- .NET 9.0
- Visual Studio 2022 or later or vs code

### Interpreting Results
- The results will vary based on your hardware configuration and system load. The parallel approaches (Parallel.ForEach and PLINQ) typically show the best performance on multi-core systems with highly parallelizable workloads.
- The "Hardware Utilization" section shows how efficiently your parallel code is utilizing the available CPU cores.
  
### Result
This is an example of the results the code will return :
```text
Preparing benchmark data...
Benchmarking with collection size: 1,000,000
CPU intensive operations: True (Iterations: 100)
Memory intensive operations: True (Chunk size: 4096 bytes)
Running each test 5 times

Running benchmarks...

Iteration 1 of 5:
  For loop:               14,117 ms
  ForEach loop:           13,394 ms
  LINQ:                   13,440 ms
  List.ForEach:           13,407 ms
  Parallel.ForEach:        8,574 ms
  PLINQ:                   8,001 ms

Iteration 2 of 5:
  For loop:               13,418 ms
  ForEach loop:           13,551 ms
  LINQ:                   13,328 ms
  List.ForEach:           13,388 ms
  Parallel.ForEach:        8,076 ms
  PLINQ:                   7,993 ms

Iteration 3 of 5:
  For loop:               13,451 ms
  ForEach loop:           13,355 ms
  LINQ:                   13,743 ms
  List.ForEach:           13,373 ms
  Parallel.ForEach:       12,886 ms
  PLINQ:                   7,974 ms

Iteration 4 of 5:
  For loop:               13,462 ms
  ForEach loop:           13,335 ms
  LINQ:                   13,339 ms
  List.ForEach:           13,383 ms
  Parallel.ForEach:       13,077 ms
  PLINQ:                   8,464 ms

Iteration 5 of 5:
  For loop:               13,438 ms
  ForEach loop:           13,402 ms
  LINQ:                   13,360 ms
  List.ForEach:           13,394 ms
  Parallel.ForEach:       10,336 ms
  PLINQ:                   8,037 ms

=== RESULTS SUMMARY ===
For loop               13,577 ms average (14117, 13418, 13451, 13462, 13438 ms) - 100.00% relative performance
ForEach loop           13,407 ms average (13394, 13551, 13355, 13335, 13402 ms) - 101.27% relative performance
LINQ                   13,442 ms average (13440, 13328, 13743, 13339, 13360 ms) - 101.01% relative performance
List.ForEach           13,389 ms average (13407, 13388, 13373, 13383, 13394 ms) - 101.41% relative performance
Parallel.ForEach       10,590 ms average (8574, 8076, 12886, 13077, 10336 ms) - 128.21% relative performance
PLINQ                   8,094 ms average (8001, 7993, 7974, 8464, 8037 ms) - 167.75% relative performance

=== SPEED COMPARISONS ===
Parallel.ForEach is 1.28x faster than For loop
Parallel.ForEach is 1.27x faster than ForEach loop
PLINQ is 1.66x faster than LINQ
ForEach loop is 1.01x faster than For loop

=== HARDWARE UTILIZATION ===
Processor count: 2
Theoretical maximum speedup: 2.00x
Actual Parallel.ForEach speedup: 1.28x
Parallel efficiency: 64.11% of theoretical maximum

Benchmark complete. 
```
