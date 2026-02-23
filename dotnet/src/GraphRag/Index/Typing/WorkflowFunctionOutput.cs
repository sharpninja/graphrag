// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Index.Typing;

/// <summary>
/// Represents the output of a single workflow function execution.
/// </summary>
/// <param name="Result">The result produced by the workflow, or <c>null</c> if none.</param>
/// <param name="Stop">If <c>true</c>, the pipeline should stop after this workflow.</param>
public sealed record WorkflowFunctionOutput(
    object? Result,
    bool Stop = false);
