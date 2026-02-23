// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Query.StructuredSearch.Drift;

/// <summary>
/// Represents a single action taken during a DRIFT search iteration.
/// </summary>
/// <param name="Query">The sub-query for this action.</param>
/// <param name="Answer">The optional answer produced by this action.</param>
/// <param name="Score">The optional relevance score for this action.</param>
public sealed record DriftAction(
    string Query,
    string? Answer = null,
    double? Score = null);
