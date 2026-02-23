// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm;

/// <summary>
/// Interface for writing metrics to a destination.
/// </summary>
public interface IMetricsWriter
{
    /// <summary>
    /// Writes the specified metrics asynchronously.
    /// </summary>
    /// <param name="metrics">The metrics to write.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAsync(Dictionary<string, double> metrics, CancellationToken cancellationToken = default);
}
