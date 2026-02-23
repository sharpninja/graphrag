// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.SearchApp.Models;

/// <summary>
/// The type of search to perform.
/// </summary>
public enum SearchType
{
    /// <summary>
    /// Basic RAG search.
    /// </summary>
    Basic,

    /// <summary>
    /// Local context search.
    /// </summary>
    Local,

    /// <summary>
    /// Global community-based search.
    /// </summary>
    Global,

    /// <summary>
    /// DRIFT (dynamic retrieval with iterative feedback) search.
    /// </summary>
    Drift,
}
