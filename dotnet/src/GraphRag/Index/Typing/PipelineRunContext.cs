// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Cache;
using GraphRag.Callbacks;
using GraphRag.Storage;
using GraphRag.Storage.Tables;

namespace GraphRag.Index.Typing;

/// <summary>
/// Provides the runtime context available to every workflow during a pipeline run.
/// </summary>
public sealed class PipelineRunContext
{
    /// <summary>
    /// Gets or sets the aggregated run statistics.
    /// </summary>
    public required PipelineRunStats Stats { get; set; }

    /// <summary>
    /// Gets the storage backend for persisting artifacts.
    /// </summary>
    public required IStorage Storage { get; init; }

    /// <summary>
    /// Gets the cache backend for intermediate results.
    /// </summary>
    public required ICache Cache { get; init; }

    /// <summary>
    /// Gets the callbacks for reporting pipeline lifecycle events.
    /// </summary>
    public required IWorkflowCallbacks Callbacks { get; init; }

    /// <summary>
    /// Gets or sets the mutable pipeline state dictionary.
    /// </summary>
    public required Dictionary<string, object?> State { get; set; }

    /// <summary>
    /// Gets or sets the optional table provider for writing output tables.
    /// </summary>
    public ITableProvider? OutputTableProvider { get; set; }

    /// <summary>
    /// Gets or sets the optional table provider for reading tables from a previous run.
    /// </summary>
    public ITableProvider? PreviousTableProvider { get; set; }
}
