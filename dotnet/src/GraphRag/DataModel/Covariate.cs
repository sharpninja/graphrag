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

    /// <summary>
    /// Creates a <see cref="Covariate"/> from a dictionary of values.
    /// </summary>
    /// <param name="data">The dictionary containing covariate data.</param>
    /// <returns>A new <see cref="Covariate"/> instance.</returns>
    public static Covariate FromDictionary(Dictionary<string, object?> data)
    {
        return new Covariate
        {
            Id = data.TryGetValue("id", out var id) ? id?.ToString() ?? string.Empty : string.Empty,
            ShortId = data.TryGetValue("short_id", out var shortId) ? shortId?.ToString() : null,
            SubjectId = data.TryGetValue("subject_id", out var sid) ? sid?.ToString() ?? string.Empty : string.Empty,
            SubjectType = data.TryGetValue("subject_type", out var st) ? st?.ToString() ?? "entity" : "entity",
            CovariateType = data.TryGetValue("covariate_type", out var ct) ? ct?.ToString() ?? "claim" : "claim",
            TextUnitIds = data.TryGetValue("text_unit_ids", out var tuIds) ? tuIds as IReadOnlyList<string> : null,
            Attributes = data.TryGetValue("attributes", out var attrs) ? attrs as Dictionary<string, object?> : null,
        };
    }
}
