// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Models;
using GraphRag.Index.Typing;
using GraphRag.Index.Workflows;

namespace GraphRag.Index;

/// <summary>
/// Factory for creating pre-configured indexing pipelines.
/// </summary>
public static class PipelineFactory
{
    /// <summary>
    /// Creates the standard (full) indexing pipeline.
    /// </summary>
    /// <returns>A <see cref="Pipeline"/> with all standard workflows.</returns>
    public static Pipeline CreateStandard()
    {
        var workflows = BuildWorkflowList(
            WorkflowNames.LoadInputDocuments,
            WorkflowNames.CreateBaseTextUnits,
            WorkflowNames.CreateFinalDocuments,
            WorkflowNames.ExtractGraph,
            WorkflowNames.FinalizeGraph,
            WorkflowNames.PruneGraph,
            WorkflowNames.ExtractCovariates,
            WorkflowNames.CreateCommunities,
            WorkflowNames.CreateFinalTextUnits,
            WorkflowNames.CreateCommunityReports,
            WorkflowNames.GenerateTextEmbeddings);

        return new Pipeline(workflows);
    }

    /// <summary>
    /// Creates a fast indexing pipeline that uses NLP-based graph extraction.
    /// </summary>
    /// <returns>A <see cref="Pipeline"/> with the fast workflow sequence.</returns>
    public static Pipeline CreateFast()
    {
        var workflows = BuildWorkflowList(
            WorkflowNames.LoadInputDocuments,
            WorkflowNames.CreateBaseTextUnits,
            WorkflowNames.CreateFinalDocuments,
            WorkflowNames.ExtractGraphNlp,
            WorkflowNames.FinalizeGraph,
            WorkflowNames.PruneGraph,
            WorkflowNames.CreateCommunities,
            WorkflowNames.CreateFinalTextUnits,
            WorkflowNames.CreateCommunityReportsText,
            WorkflowNames.GenerateTextEmbeddings);

        return new Pipeline(workflows);
    }

    /// <summary>
    /// Creates an incremental update pipeline.
    /// </summary>
    /// <returns>A <see cref="Pipeline"/> with the update workflow sequence.</returns>
    public static Pipeline CreateUpdate()
    {
        var workflows = BuildWorkflowList(
            WorkflowNames.LoadUpdateDocuments,
            WorkflowNames.UpdateFinalDocuments,
            WorkflowNames.UpdateTextUnits,
            WorkflowNames.UpdateEntitiesRelationships,
            WorkflowNames.UpdateCommunities,
            WorkflowNames.UpdateCommunityReports,
            WorkflowNames.UpdateCovariates,
            WorkflowNames.UpdateTextEmbeddings,
            WorkflowNames.UpdateCleanState);

        return new Pipeline(workflows);
    }

    private static List<(string Name, Func<GraphRagConfig, PipelineRunContext, Task<WorkflowFunctionOutput>> Function)> BuildWorkflowList(
        params string[] names)
    {
        return names
            .Select(name => (name, WorkflowRegistry.Get(name)))
            .ToList();
    }
}
