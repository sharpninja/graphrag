// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Metrics;

/// <summary>
/// Default metrics processor that records metrics to a store and optionally writes them.
/// </summary>
public sealed class DefaultMetricsProcessor : IMetricsProcessor
{
    private readonly IMetricsStore _store;
    private readonly IMetricsWriter? _writer;
    private int _counter;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultMetricsProcessor"/> class.
    /// </summary>
    /// <param name="store">The metrics store to record metrics to.</param>
    /// <param name="writer">An optional metrics writer to write metrics to.</param>
    public DefaultMetricsProcessor(IMetricsStore store, IMetricsWriter? writer = null)
    {
        ArgumentNullException.ThrowIfNull(store);

        _store = store;
        _writer = writer;
    }

    /// <inheritdoc />
    public async Task ProcessAsync(Dictionary<string, double> metrics, CancellationToken cancellationToken = default)
    {
        var key = $"metrics_{Interlocked.Increment(ref _counter)}";
        _store.Record(key, metrics);

        if (_writer is not null)
        {
            await _writer.WriteAsync(metrics, cancellationToken).ConfigureAwait(false);
        }
    }
}
