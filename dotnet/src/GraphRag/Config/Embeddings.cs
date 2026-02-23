// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config;

/// <summary>
/// Well-known embedding name constants used across GraphRag.
/// </summary>
public static class Embeddings
{
    /// <summary>Gets the embedding name for entity descriptions.</summary>
    public const string EntityDescription = "entity_description";

    /// <summary>Gets the embedding name for community full content.</summary>
    public const string CommunityFullContent = "community_full_content";

    /// <summary>Gets the embedding name for text unit text.</summary>
    public const string TextUnitText = "text_unit_text";

    /// <summary>Gets all known embedding names.</summary>
    public static readonly IReadOnlyList<string> All = [EntityDescription, CommunityFullContent, TextUnitText];

    /// <summary>Gets the default set of embedding names.</summary>
    public static readonly IReadOnlyList<string> Default = [EntityDescription, CommunityFullContent, TextUnitText];
}
