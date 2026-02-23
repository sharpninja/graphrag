// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm;

/// <summary>
/// Interface for processing metrics.
/// </summary>
public interface IMetricsProcessor
{
    /// <summary>
    /// Processes the specified metrics asynchronously.
    /// </summary>
    /// <param name="metrics">The metrics to process.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ProcessAsync(Dictionary<string, double> metrics, CancellationToken cancellationToken = default);
}
