// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using Azure.Identity;
using Azure.Storage.Blobs;

namespace GraphRag.SearchApp.Services;

/// <summary>
/// Reads data from Azure Blob Storage.
/// </summary>
public class BlobDatasource : IDatasource
{
    private readonly BlobContainerClient _containerClient;
    private readonly string _pathPrefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlobDatasource"/> class.
    /// </summary>
    /// <param name="accountName">The Azure Blob Storage account name.</param>
    /// <param name="containerName">The container name.</param>
    /// <param name="pathPrefix">The blob path prefix for the dataset.</param>
    public BlobDatasource(string accountName, string containerName, string pathPrefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountName);
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);

        var serviceClient = new BlobServiceClient(
            new Uri($"https://{accountName}.blob.core.windows.net"),
            new DefaultAzureCredential());
        _containerClient = serviceClient.GetBlobContainerClient(containerName);
        _pathPrefix = pathPrefix;
    }

    /// <inheritdoc />
    public async Task<List<Dictionary<string, object?>>> ReadTableAsync(
        string tableName,
        CancellationToken cancellationToken = default)
    {
        var blobName = $"{_pathPrefix}/{tableName}.parquet";
        var blobClient = _containerClient.GetBlobClient(blobName);

        using var stream = new MemoryStream();
        await blobClient.DownloadToAsync(stream, cancellationToken).ConfigureAwait(false);
        stream.Position = 0;

        // Delegate to Parquet.Net for parsing
        var rows = new List<Dictionary<string, object?>>();
        using var reader = await Parquet.ParquetReader.CreateAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        for (int g = 0; g < reader.RowGroupCount; g++)
        {
            using var groupReader = reader.OpenRowGroupReader(g);
            var fields = reader.Schema.GetDataFields();
            var columns = new Dictionary<string, object?[]>();
            foreach (var field in fields)
            {
                var column = await groupReader.ReadColumnAsync(field, cancellationToken).ConfigureAwait(false);
                columns[field.Name] = column.Data.Cast<object?>().ToArray();
            }

            var rowCount = columns.Values.First().Length;
            for (int r = 0; r < rowCount; r++)
            {
                var row = new Dictionary<string, object?>();
                foreach (var (name, data) in columns)
                {
                    row[name] = data[r];
                }

                rows.Add(row);
            }
        }

        return rows;
    }

    /// <inheritdoc />
    public async Task<string?> ReadSettingsAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var blobName = $"{_pathPrefix}/{fileName}";
        var blobClient = _containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        var response = await blobClient.DownloadContentAsync(cancellationToken).ConfigureAwait(false);
        return response.Value.Content.ToString();
    }

    /// <inheritdoc />
    public async Task<bool> HasTableAsync(
        string tableName,
        CancellationToken cancellationToken = default)
    {
        var blobName = $"{_pathPrefix}/{tableName}.parquet";
        var blobClient = _containerClient.GetBlobClient(blobName);
        return await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false);
    }
}
