// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.DataModel;

namespace GraphRag.SearchApp.Models;

/// <summary>
/// Holds the fully loaded knowledge model data for a dataset.
/// </summary>
public sealed class KnowledgeModel
{
    /// <summary>
    /// Gets the entities extracted from the dataset.
    /// </summary>
    public IReadOnlyList<Entity> Entities { get; init; } = [];

    /// <summary>
    /// Gets the relationships between entities.
    /// </summary>
    public IReadOnlyList<Relationship> Relationships { get; init; } = [];

    /// <summary>
    /// Gets the communities detected in the knowledge graph.
    /// </summary>
    public IReadOnlyList<Community> Communities { get; init; } = [];

    /// <summary>
    /// Gets the community reports.
    /// </summary>
    public IReadOnlyList<CommunityReport> CommunityReports { get; init; } = [];

    /// <summary>
    /// Gets the text units (chunks) from the source documents.
    /// </summary>
    public IReadOnlyList<TextUnit> TextUnits { get; init; } = [];

    /// <summary>
    /// Gets the covariates extracted from the dataset.
    /// </summary>
    public IReadOnlyList<Covariate> Covariates { get; init; } = [];
}
