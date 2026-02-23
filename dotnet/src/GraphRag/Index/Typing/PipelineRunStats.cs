// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Index.Typing;

/// <summary>
/// Aggregated statistics for a complete pipeline run.
/// </summary>
/// <param name="TotalRuntime">Total wall-clock time in seconds for the entire pipeline.</param>
/// <param name="NumDocuments">Total number of documents processed.</param>
/// <param name="UpdateDocuments">Number of documents that were updated.</param>
/// <param name="InputLoadTime">Time in seconds spent loading input data.</param>
/// <param name="Workflows">Per-workflow metrics keyed by workflow name.</param>
public sealed record PipelineRunStats(
    double TotalRuntime,
    int NumDocuments,
    int UpdateDocuments,
    double InputLoadTime,
    Dictionary<string, WorkflowMetrics> Workflows);
