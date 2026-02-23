// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Diagnostics;

using GraphRag.Index.Typing;

namespace GraphRag.Index.Run;

/// <summary>
/// Measures elapsed time and memory usage for a single workflow execution.
/// </summary>
public sealed class WorkflowProfiler : IDisposable
{
    private readonly Stopwatch _stopwatch;
    private readonly long _startMemory;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowProfiler"/> class and starts measuring.
    /// </summary>
    public WorkflowProfiler()
    {
        _startMemory = GC.GetTotalMemory(forceFullCollection: false);
        _stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Gets the collected metrics. Only valid after <see cref="Dispose"/> is called.
    /// </summary>
    public WorkflowMetrics? Metrics { get; private set; }

    /// <summary>
    /// Stops measurement and captures the final metrics.
    /// </summary>
    public void Dispose()
    {
        if (Metrics is not null)
        {
            return;
        }

        _stopwatch.Stop();
        var endMemory = GC.GetTotalMemory(forceFullCollection: false);
        var peakMemory = Math.Max(_startMemory, endMemory);

        Metrics = new WorkflowMetrics(
            OverallTimeSeconds: _stopwatch.Elapsed.TotalSeconds,
            PeakMemoryBytes: peakMemory,
            MemoryDeltaBytes: endMemory - _startMemory);
    }
}
