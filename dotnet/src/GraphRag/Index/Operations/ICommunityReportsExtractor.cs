// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.DataModel;

namespace GraphRag.Index.Operations;

/// <summary>
/// Generates a report for a community based on its entities and relationships.
/// </summary>
public interface ICommunityReportsExtractor
{
    /// <summary>
    /// Extracts a community report for the given community.
    /// </summary>
    /// <param name="community">The community to generate a report for.</param>
    /// <param name="entities">The entities belonging to the community.</param>
    /// <param name="relationships">The relationships within the community.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A <see cref="CommunityReport"/> for the community.</returns>
    Task<CommunityReport> ExtractAsync(
        Community community,
        IReadOnlyList<Entity> entities,
        IReadOnlyList<Relationship> relationships,
        CancellationToken ct);
}
