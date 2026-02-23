// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Config;

/// <summary>
/// Configuration for metrics collection and reporting.
/// </summary>
public sealed record MetricsConfig
{
    /// <summary>Gets the metrics processor type.</summary>
    public string Type { get; init; } = MetricsProcessorType.Default;

    /// <summary>Gets the metrics writer type.</summary>
    public string WriterType { get; init; } = MetricsWriterType.Log;

    /// <summary>Gets the metrics store type.</summary>
    public string StoreType { get; init; } = MetricsStoreType.Memory;

    /// <summary>Gets the output file path for file-based metrics writers.</summary>
    public string? OutputPath { get; init; }
}
