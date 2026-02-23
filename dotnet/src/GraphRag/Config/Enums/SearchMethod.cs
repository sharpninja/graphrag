// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Enums;

/// <summary>
/// Available search method types.
/// </summary>
public static class SearchMethod
{
    /// <summary>Gets the identifier for local search.</summary>
    public const string Local = "local";

    /// <summary>Gets the identifier for global search.</summary>
    public const string Global = "global";

    /// <summary>Gets the identifier for DRIFT search.</summary>
    public const string Drift = "drift";

    /// <summary>Gets the identifier for basic search.</summary>
    public const string Basic = "basic";
}
