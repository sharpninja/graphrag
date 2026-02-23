// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Metrics;

/// <summary>
/// A no-op metrics store that discards all recorded metrics.
/// </summary>
public sealed class NoopMetricsStore : IMetricsStore
{
    private static readonly IReadOnlyDictionary<string, Dictionary<string, double>> Empty =
        new Dictionary<string, Dictionary<string, double>>();

    /// <inheritdoc />
    public void Record(string key, Dictionary<string, double> metrics)
    {
        // No-op
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, Dictionary<string, double>> GetAll()
    {
        return Empty;
    }
}
