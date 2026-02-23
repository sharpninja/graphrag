// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Enums;

/// <summary>
/// Available indexing method types.
/// </summary>
public static class IndexingMethod
{
    /// <summary>Gets the identifier for standard indexing.</summary>
    public const string Standard = "standard";

    /// <summary>Gets the identifier for fast indexing.</summary>
    public const string Fast = "fast";

    /// <summary>Gets the identifier for standard update indexing.</summary>
    public const string StandardUpdate = "standard-update";

    /// <summary>Gets the identifier for fast update indexing.</summary>
    public const string FastUpdate = "fast-update";
}
