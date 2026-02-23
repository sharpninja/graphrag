// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;
using GraphRag.Llm.Config;
using GraphRag.Llm.Metrics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GraphRag.Llm.Factories;

/// <summary>
/// Factory for creating <see cref="IMetricsWriter"/> instances by strategy name.
/// </summary>
public sealed class MetricsWriterFactory : ServiceFactory<IMetricsWriter>
{
    private bool _builtinsRegistered;

    /// <summary>
    /// Registers the built-in metrics writers if not already registered.
    /// </summary>
    public void EnsureBuiltins()
    {
        if (_builtinsRegistered)
        {
            return;
        }

        Register(
            MetricsWriterType.Log,
            args =>
            {
                var logger = args.TryGetValue("logger", out var l) && l is ILogger log
                    ? log
                    : NullLogger.Instance;
                return new LogMetricsWriter(logger);
            },
            ServiceScope.Singleton);

        Register(
            MetricsWriterType.File,
            args =>
            {
                var baseDir = args.TryGetValue("baseDir", out var d) && d is string dir
                    ? dir
                    : "metrics";
                return new FileMetricsWriter(baseDir);
            },
            ServiceScope.Singleton);

        _builtinsRegistered = true;
    }
}
