// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Discovery;

namespace GraphRag.Startup;

/// <summary>
/// Wires up strategy discovery at application startup by scanning
/// for strategy assemblies and registering discovered implementations.
/// </summary>
public static class StrategyRegistration
{
    /// <summary>
    /// Perform strategy discovery from the application directory and optional configuration.
    /// </summary>
    /// <param name="config">Optional strategy configuration with explicit assembly list.</param>
    /// <returns>The <see cref="StrategyDiscovery"/> instance with all discovered strategies.</returns>
    public static StrategyDiscovery DiscoverStrategies(StrategyConfiguration? config = null)
    {
        var discovery = new StrategyDiscovery();

        // Auto-discover from application directory (GraphRag.*.dll convention).
        discovery.DiscoverFromApplicationDirectory();

        // Also load explicitly configured assemblies.
        if (config is not null)
        {
            discovery.RegisterFromConfiguration(config);
        }

        return discovery;
    }
}
