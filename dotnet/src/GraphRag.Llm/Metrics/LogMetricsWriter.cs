// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.Extensions.Logging;

namespace GraphRag.Llm.Metrics;

/// <summary>
/// A metrics writer that writes metrics to an <see cref="ILogger"/> instance.
/// </summary>
public sealed partial class LogMetricsWriter : IMetricsWriter
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogMetricsWriter"/> class.
    /// </summary>
    /// <param name="logger">The logger to write metrics to.</param>
    public LogMetricsWriter(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    /// <inheritdoc />
    public Task WriteAsync(Dictionary<string, double> metrics, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var (key, value) in metrics)
        {
            LogMetric(_logger, key, value);
        }

        return Task.CompletedTask;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Metric: {Key} = {Value}")]
    private static partial void LogMetric(ILogger logger, string key, double value);
}
