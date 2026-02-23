// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Index.Typing;

/// <summary>
/// Captures performance metrics for a single workflow execution.
/// </summary>
/// <param name="OverallTimeSeconds">Total wall-clock time in seconds for the workflow.</param>
/// <param name="PeakMemoryBytes">Peak working-set memory in bytes during the workflow.</param>
/// <param name="MemoryDeltaBytes">Change in memory usage (bytes) from start to end of the workflow.</param>
public sealed record WorkflowMetrics(
    double OverallTimeSeconds,
    long PeakMemoryBytes,
    long MemoryDeltaBytes);
