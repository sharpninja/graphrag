// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Query.StructuredSearch.Drift;

/// <summary>
/// Tracks the state of a DRIFT search across iterations.
/// </summary>
public class QueryState
{
    private readonly List<DriftAction> actions = [];

    /// <summary>
    /// Gets the read-only list of actions taken during the search.
    /// </summary>
    public IReadOnlyList<DriftAction> Actions => actions;

    /// <summary>
    /// Adds an action to the search state.
    /// </summary>
    /// <param name="action">The action to add.</param>
    public void AddAction(DriftAction action)
    {
        actions.Add(action);
    }
}
