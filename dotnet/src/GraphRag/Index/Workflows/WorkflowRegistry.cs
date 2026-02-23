// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Models;
using GraphRag.Index.Typing;

namespace GraphRag.Index.Workflows;

/// <summary>
/// Maps workflow names to their implementation functions.
/// </summary>
public static class WorkflowRegistry
{
    private static readonly Dictionary<string, Func<GraphRagConfig, PipelineRunContext, Task<WorkflowFunctionOutput>>> Registry = new()
    {
        [WorkflowNames.LoadInputDocuments] = StubWorkflowAsync,
        [WorkflowNames.CreateBaseTextUnits] = StubWorkflowAsync,
        [WorkflowNames.CreateFinalDocuments] = StubWorkflowAsync,
        [WorkflowNames.ExtractGraph] = StubWorkflowAsync,
        [WorkflowNames.ExtractGraphNlp] = StubWorkflowAsync,
        [WorkflowNames.FinalizeGraph] = StubWorkflowAsync,
        [WorkflowNames.PruneGraph] = StubWorkflowAsync,
        [WorkflowNames.ExtractCovariates] = StubWorkflowAsync,
        [WorkflowNames.CreateCommunities] = StubWorkflowAsync,
        [WorkflowNames.CreateFinalTextUnits] = StubWorkflowAsync,
        [WorkflowNames.CreateCommunityReports] = StubWorkflowAsync,
        [WorkflowNames.CreateCommunityReportsText] = StubWorkflowAsync,
        [WorkflowNames.GenerateTextEmbeddings] = StubWorkflowAsync,
        [WorkflowNames.LoadUpdateDocuments] = StubWorkflowAsync,
        [WorkflowNames.UpdateEntitiesRelationships] = StubWorkflowAsync,
        [WorkflowNames.UpdateCommunities] = StubWorkflowAsync,
        [WorkflowNames.UpdateCommunityReports] = StubWorkflowAsync,
        [WorkflowNames.UpdateCovariates] = StubWorkflowAsync,
        [WorkflowNames.UpdateFinalDocuments] = StubWorkflowAsync,
        [WorkflowNames.UpdateTextEmbeddings] = StubWorkflowAsync,
        [WorkflowNames.UpdateTextUnits] = StubWorkflowAsync,
        [WorkflowNames.UpdateCleanState] = StubWorkflowAsync,
    };

    /// <summary>
    /// Gets the workflow function for the specified workflow name.
    /// </summary>
    /// <param name="name">The workflow name.</param>
    /// <returns>The workflow function.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the workflow name is not registered.</exception>
    public static Func<GraphRagConfig, PipelineRunContext, Task<WorkflowFunctionOutput>> Get(string name) =>
        Registry.TryGetValue(name, out var func)
            ? func
            : throw new KeyNotFoundException($"Workflow '{name}' is not registered.");

    /// <summary>
    /// Gets all registered workflow names.
    /// </summary>
    public static IReadOnlyCollection<string> Names => Registry.Keys;

    private static Task<WorkflowFunctionOutput> StubWorkflowAsync(GraphRagConfig config, PipelineRunContext context) =>
        Task.FromResult(new WorkflowFunctionOutput(Result: null));
}
