// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Logger;

namespace GraphRag.Callbacks;

/// <summary>
/// A no-op implementation of <see cref="IWorkflowCallbacks"/> that discards all events.
/// </summary>
public sealed class NoopWorkflowCallbacks : IWorkflowCallbacks
{
    /// <inheritdoc />
    public void OnPipelineStart(IReadOnlyList<string> names)
    {
    }

    /// <inheritdoc />
    public void OnPipelineEnd(IReadOnlyList<object> results)
    {
    }

    /// <inheritdoc />
    public void OnWorkflowStart(string name, object instance)
    {
    }

    /// <inheritdoc />
    public void OnWorkflowEnd(string name, object instance)
    {
    }

    /// <inheritdoc />
    public void OnProgress(ProgressInfo progress)
    {
    }

    /// <inheritdoc />
    public void OnPipelineError(Exception exception)
    {
    }
}
