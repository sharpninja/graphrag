// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Logger;

namespace GraphRag.Callbacks;

/// <summary>
/// Defines callbacks for workflow pipeline lifecycle events.
/// </summary>
public interface IWorkflowCallbacks
{
    /// <summary>
    /// Called when the pipeline starts.
    /// </summary>
    /// <param name="names">The names of the workflows in the pipeline.</param>
    void OnPipelineStart(IReadOnlyList<string> names);

    /// <summary>
    /// Called when the pipeline ends.
    /// </summary>
    /// <param name="results">The results produced by the pipeline.</param>
    void OnPipelineEnd(IReadOnlyList<object> results);

    /// <summary>
    /// Called when an individual workflow starts.
    /// </summary>
    /// <param name="name">The name of the workflow.</param>
    /// <param name="instance">The workflow instance.</param>
    void OnWorkflowStart(string name, object instance);

    /// <summary>
    /// Called when an individual workflow ends.
    /// </summary>
    /// <param name="name">The name of the workflow.</param>
    /// <param name="instance">The workflow instance.</param>
    void OnWorkflowEnd(string name, object instance);

    /// <summary>
    /// Called to report progress during a workflow.
    /// </summary>
    /// <param name="progress">The progress information.</param>
    void OnProgress(ProgressInfo progress);

    /// <summary>
    /// Called when a pipeline error occurs.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    void OnPipelineError(Exception exception);
}
