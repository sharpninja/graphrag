// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.DataModel;

/// <summary>
/// A covariate associated with a subject.
/// </summary>
public sealed record Covariate : Identified
{
    /// <summary>
    /// Gets the subject identifier of the covariate.
    /// </summary>
    public required string SubjectId { get; init; }

    /// <summary>
    /// Gets the type of the subject.
    /// </summary>
    public string SubjectType { get; init; } = "entity";

    /// <summary>
    /// Gets the type of the covariate.
    /// </summary>
    public string CovariateType { get; init; } = "claim";

    /// <summary>
    /// Gets the text unit identifiers associated with this covariate.
    /// </summary>
    public IReadOnlyList<string>? TextUnitIds { get; init; }

    /// <summary>
    /// Gets the additional attributes of the covariate.
    /// </summary>
    public Dictionary<string, object?>? Attributes { get; init; }
}
