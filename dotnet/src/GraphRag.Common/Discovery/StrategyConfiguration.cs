// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Common.Discovery;

/// <summary>
/// Configuration for strategy resolution including defaults, assembly list, and per-key overrides.
/// </summary>
public sealed record StrategyConfiguration
{
    /// <summary>
    /// Gets the default strategy key for each interface category.
    /// </summary>
    public Dictionary<string, string> Defaults { get; init; } = new();

    /// <summary>
    /// Gets the list of assemblies to scan for strategies.
    /// </summary>
    public IReadOnlyList<string> Assemblies { get; init; } = [];

    /// <summary>
    /// Gets per-key overrides with explicit assembly and type.
    /// </summary>
    public Dictionary<string, StrategyOverride> Overrides { get; init; } = new();
}

/// <summary>
/// Override for a specific strategy key with explicit assembly and type name.
/// </summary>
public sealed record StrategyOverride
{
    /// <summary>
    /// Gets the assembly name containing the implementation.
    /// </summary>
    public string? Assembly { get; init; }

    /// <summary>
    /// Gets the fully-qualified type name of the implementation.
    /// </summary>
    public string? Type { get; init; }
}
