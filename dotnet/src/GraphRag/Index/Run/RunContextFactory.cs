// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Cache;
using GraphRag.Callbacks;
using GraphRag.Index.Typing;
using GraphRag.Storage;
using GraphRag.Storage.Tables;

namespace GraphRag.Index.Run;

/// <summary>
/// Factory for creating <see cref="PipelineRunContext"/> instances from configuration and services.
/// </summary>
public static class RunContextFactory
{
    /// <summary>
    /// Creates a new <see cref="PipelineRunContext"/> with the specified services.
    /// </summary>
    /// <param name="storage">The storage backend.</param>
    /// <param name="cache">The cache backend.</param>
    /// <param name="callbacks">The workflow callbacks.</param>
    /// <param name="outputTableProvider">Optional table provider for output tables.</param>
    /// <param name="previousTableProvider">Optional table provider for tables from a previous run.</param>
    /// <returns>A new <see cref="PipelineRunContext"/>.</returns>
    public static PipelineRunContext Create(
        IStorage storage,
        ICache cache,
        IWorkflowCallbacks callbacks,
        ITableProvider? outputTableProvider = null,
        ITableProvider? previousTableProvider = null)
    {
        return new PipelineRunContext
        {
            Stats = new PipelineRunStats(
                TotalRuntime: 0,
                NumDocuments: 0,
                UpdateDocuments: 0,
                InputLoadTime: 0,
                Workflows: new Dictionary<string, WorkflowMetrics>()),
            Storage = storage,
            Cache = cache,
            Callbacks = callbacks,
            State = PipelineState.Create(),
            OutputTableProvider = outputTableProvider,
            PreviousTableProvider = previousTableProvider,
        };
    }
}
