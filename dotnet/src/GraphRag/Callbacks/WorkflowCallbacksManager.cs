// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Logger;

namespace GraphRag.Callbacks;

/// <summary>
/// A composite <see cref="IWorkflowCallbacks"/> that delegates all calls to a list of registered callbacks.
/// </summary>
public sealed class WorkflowCallbacksManager : IWorkflowCallbacks
{
    private readonly List<IWorkflowCallbacks> _callbacks = [];

    /// <summary>
    /// Registers a callback to receive workflow events.
    /// </summary>
    /// <param name="callbacks">The callback instance to register.</param>
    public void Register(IWorkflowCallbacks callbacks)
    {
        ArgumentNullException.ThrowIfNull(callbacks);
        _callbacks.Add(callbacks);
    }

    /// <inheritdoc />
    public void OnPipelineStart(IReadOnlyList<string> names)
    {
        foreach (var cb in _callbacks)
        {
            cb.OnPipelineStart(names);
        }
    }

    /// <inheritdoc />
    public void OnPipelineEnd(IReadOnlyList<object> results)
    {
        foreach (var cb in _callbacks)
        {
            cb.OnPipelineEnd(results);
        }
    }

    /// <inheritdoc />
    public void OnWorkflowStart(string name, object instance)
    {
        foreach (var cb in _callbacks)
        {
            cb.OnWorkflowStart(name, instance);
        }
    }

    /// <inheritdoc />
    public void OnWorkflowEnd(string name, object instance)
    {
        foreach (var cb in _callbacks)
        {
            cb.OnWorkflowEnd(name, instance);
        }
    }

    /// <inheritdoc />
    public void OnProgress(ProgressInfo progress)
    {
        foreach (var cb in _callbacks)
        {
            cb.OnProgress(progress);
        }
    }

    /// <inheritdoc />
    public void OnPipelineError(Exception exception)
    {
        foreach (var cb in _callbacks)
        {
            cb.OnPipelineError(exception);
        }
    }
}
