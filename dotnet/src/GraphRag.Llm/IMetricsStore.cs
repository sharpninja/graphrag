// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm;

/// <summary>
/// Interface for storing and retrieving metrics.
/// </summary>
public interface IMetricsStore
{
    /// <summary>
    /// Records a set of metrics under the specified key.
    /// </summary>
    /// <param name="key">The key to associate with the metrics.</param>
    /// <param name="metrics">The metrics to record.</param>
    void Record(string key, Dictionary<string, double> metrics);

    /// <summary>
    /// Gets all recorded metrics.
    /// </summary>
    /// <returns>A read-only dictionary of all recorded metrics keyed by their identifier.</returns>
    IReadOnlyDictionary<string, Dictionary<string, double>> GetAll();
}
