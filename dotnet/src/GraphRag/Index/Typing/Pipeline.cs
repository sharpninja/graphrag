// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Models;

namespace GraphRag.Index.Typing;

/// <summary>
/// Represents an ordered collection of named workflow functions that form an indexing pipeline.
/// </summary>
public class Pipeline
{
    private readonly List<(string Name, Func<GraphRagConfig, PipelineRunContext, Task<WorkflowFunctionOutput>> Function)> _workflows;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pipeline"/> class.
    /// </summary>
    /// <param name="workflows">The ordered list of named workflow functions.</param>
    public Pipeline(List<(string Name, Func<GraphRagConfig, PipelineRunContext, Task<WorkflowFunctionOutput>> Function)> workflows)
    {
        _workflows = workflows ?? throw new ArgumentNullException(nameof(workflows));
    }

    /// <summary>
    /// Gets the names of all workflows in this pipeline.
    /// </summary>
    public IReadOnlyList<string> Names => _workflows.Select(w => w.Name).ToList().AsReadOnly();

    /// <summary>
    /// Enumerates the workflows in pipeline order.
    /// </summary>
    /// <returns>An enumerable of named workflow functions.</returns>
    public IEnumerable<(string Name, Func<GraphRagConfig, PipelineRunContext, Task<WorkflowFunctionOutput>> Function)> Run()
    {
        foreach (var workflow in _workflows)
        {
            yield return workflow;
        }
    }

    /// <summary>
    /// Removes a workflow by name.
    /// </summary>
    /// <param name="name">The name of the workflow to remove.</param>
    public void Remove(string name)
    {
        _workflows.RemoveAll(w => string.Equals(w.Name, name, StringComparison.Ordinal));
    }
}
