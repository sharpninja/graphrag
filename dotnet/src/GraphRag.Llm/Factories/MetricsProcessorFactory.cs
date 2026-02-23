// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;
using GraphRag.Llm.Config;
using GraphRag.Llm.Metrics;

namespace GraphRag.Llm.Factories;

/// <summary>
/// Factory for creating <see cref="IMetricsProcessor"/> instances by strategy name.
/// </summary>
public sealed class MetricsProcessorFactory : ServiceFactory<IMetricsProcessor>
{
    private bool _builtinsRegistered;

    /// <summary>
    /// Registers the built-in metrics processors if not already registered.
    /// </summary>
    public void EnsureBuiltins()
    {
        if (_builtinsRegistered)
        {
            return;
        }

        Register(
            MetricsProcessorType.Default,
            args =>
            {
                var store = args.TryGetValue("store", out var s) && s is IMetricsStore ms
                    ? ms
                    : new MemoryMetricsStore();
                var writer = args.TryGetValue("writer", out var w) && w is IMetricsWriter mw
                    ? mw
                    : null;
                return new DefaultMetricsProcessor(store, writer);
            },
            ServiceScope.Singleton);

        _builtinsRegistered = true;
    }
}
