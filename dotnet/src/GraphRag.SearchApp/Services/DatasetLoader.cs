// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text.Json;
using GraphRag.SearchApp.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GraphRag.SearchApp.Services;

/// <summary>
/// Loads dataset listings and creates datasource instances.
/// </summary>
public class DatasetLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly SearchAppConfig _config;
    private readonly ILogger<DatasetLoader> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetLoader"/> class.
    /// </summary>
    /// <param name="options">The search app configuration.</param>
    /// <param name="logger">The logger instance.</param>
    public DatasetLoader(IOptions<SearchAppConfig> options, ILogger<DatasetLoader> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        _config = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Loads the dataset listing from the configured listing file.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The list of available datasets.</returns>
    public async Task<IReadOnlyList<DatasetConfig>> LoadDatasetListingAsync(
        CancellationToken cancellationToken = default)
    {
        var listingPath = Path.Combine(_config.DataRoot, _config.ListingFile);
        if (!File.Exists(listingPath))
        {
            _logger.LogWarning("Listing file not found at {Path}, returning empty list.", listingPath);
            return [];
        }

        var json = await File.ReadAllTextAsync(listingPath, cancellationToken).ConfigureAwait(false);
        var datasets = JsonSerializer.Deserialize<List<DatasetConfig>>(json, JsonOptions);

        return datasets ?? [];
    }

    /// <summary>
    /// Creates a datasource for the specified dataset configuration.
    /// </summary>
    /// <param name="dataset">The dataset configuration.</param>
    /// <returns>An <see cref="IDatasource"/> instance for reading the dataset.</returns>
    public IDatasource CreateDatasource(DatasetConfig dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        if (!string.IsNullOrEmpty(_config.BlobAccountName) && !string.IsNullOrEmpty(_config.BlobContainerName))
        {
            _logger.LogInformation("Creating blob datasource for dataset {Key}.", dataset.Key);
            return new BlobDatasource(_config.BlobAccountName, _config.BlobContainerName, dataset.Path);
        }

        _logger.LogInformation("Creating local datasource for dataset {Key}.", dataset.Key);
        var fullPath = Path.IsPathRooted(dataset.Path) ? dataset.Path : Path.Combine(_config.DataRoot, dataset.Path);

        // Create a simple local file-based table provider
        return new LocalFileDatasource(fullPath);
    }
}
