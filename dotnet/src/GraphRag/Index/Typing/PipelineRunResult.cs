// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Index.Typing;

/// <summary>
/// Represents the result of executing a single workflow within a pipeline run.
/// </summary>
/// <param name="Workflow">The name of the workflow that produced this result.</param>
/// <param name="Result">The result object, or <c>null</c> if the workflow produced no output.</param>
/// <param name="State">The pipeline state after this workflow completed.</param>
/// <param name="Error">The exception that occurred during execution, or <c>null</c> on success.</param>
public sealed record PipelineRunResult(
    string Workflow,
    object? Result,
    Dictionary<string, object?> State,
    Exception? Error);
