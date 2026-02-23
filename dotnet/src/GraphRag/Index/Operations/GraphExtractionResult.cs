// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.DataModel;

namespace GraphRag.Index.Operations;

/// <summary>
/// The result of a graph extraction operation.
/// </summary>
/// <param name="Entities">The entities extracted from the text.</param>
/// <param name="Relationships">The relationships extracted from the text.</param>
public sealed record GraphExtractionResult(
    IReadOnlyList<Entity> Entities,
    IReadOnlyList<Relationship> Relationships);
