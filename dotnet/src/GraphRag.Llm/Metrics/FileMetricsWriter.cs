// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphRag.Llm.Metrics;

/// <summary>
/// A metrics writer that writes metrics as JSON to files in a base directory.
/// </summary>
public sealed class FileMetricsWriter : IMetricsWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _baseDir;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileMetricsWriter"/> class.
    /// </summary>
    /// <param name="baseDir">The base directory to write metrics files to.</param>
    public FileMetricsWriter(string baseDir)
    {
        ArgumentNullException.ThrowIfNull(baseDir);
        _baseDir = baseDir;
    }

    /// <inheritdoc />
    public async Task WriteAsync(Dictionary<string, double> metrics, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Directory.CreateDirectory(_baseDir);

        var fileName = $"metrics_{DateTimeOffset.UtcNow:yyyyMMdd_HHmmss_fff}.json";
        var path = Path.Combine(_baseDir, fileName);

        var json = JsonSerializer.Serialize(metrics, JsonOptions);
        await File.WriteAllTextAsync(path, json, cancellationToken).ConfigureAwait(false);
    }
}
