// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;
using GraphRag.Llm.Config;
using GraphRag.Llm.Metrics;

namespace GraphRag.Llm.Factories;

/// <summary>
/// Factory for creating <see cref="IMetricsStore"/> instances by strategy name.
/// </summary>
public sealed class MetricsStoreFactory : ServiceFactory<IMetricsStore>
{
    private bool _builtinsRegistered;

    /// <summary>
    /// Registers the built-in metrics stores if not already registered.
    /// </summary>
    public void EnsureBuiltins()
    {
        if (_builtinsRegistered)
        {
            return;
        }

        Register(MetricsStoreType.Memory, _ => new MemoryMetricsStore(), ServiceScope.Singleton);

        _builtinsRegistered = true;
    }
}
