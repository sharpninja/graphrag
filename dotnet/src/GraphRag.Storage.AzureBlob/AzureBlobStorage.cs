// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text;
using System.Text.RegularExpressions;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using GraphRag.Common.Discovery;

namespace GraphRag.Storage.AzureBlob;

/// <summary>
/// Azure Blob Storage implementation of <see cref="IStorage"/>.
/// </summary>
[StrategyImplementation("blob", typeof(IStorage))]
public sealed class AzureBlobStorage : IStorage
{
    private readonly BlobContainerClient _containerClient;
    private readonly string _pathPrefix;
    private readonly string _encoding;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobStorage"/> class.
    /// </summary>
    /// <param name="connectionString">The Azure Storage connection string.</param>
    /// <param name="containerName">The name of the blob container.</param>
    /// <param name="pathPrefix">An optional path prefix for all blob keys.</param>
    /// <param name="encoding">The default text encoding name. Defaults to <c>"utf-8"</c>.</param>
    public AzureBlobStorage(string connectionString, string containerName, string? pathPrefix = null, string encoding = "utf-8")
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        ArgumentException.ThrowIfNullOrEmpty(containerName);
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _pathPrefix = pathPrefix ?? string.Empty;
        _encoding = encoding;
    }

    /// <inheritdoc/>
    public IEnumerable<string> Find(Regex filePattern)
    {
        var blobs = _containerClient.GetBlobs(prefix: NullIfEmpty(_pathPrefix));
        foreach (var blob in blobs)
        {
            var name = StripPrefix(blob.Name);
            if (filePattern.IsMatch(name))
            {
                yield return name;
            }
        }
    }

    /// <inheritdoc/>
    public async Task<object?> GetAsync(string key, bool? asBytes = null, string? encoding = null, CancellationToken cancellationToken = default)
    {
        var blobClient = GetBlobClient(key);
        if (!await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        var response = await blobClient.DownloadContentAsync(cancellationToken).ConfigureAwait(false);
        var content = response.Value.Content;

        if (asBytes == true)
        {
            return content.ToArray();
        }

        var enc = Encoding.GetEncoding(encoding ?? _encoding);
        return enc.GetString(content);
    }

    /// <inheritdoc/>
    public async Task SetAsync(string key, object value, string? encoding = null, CancellationToken cancellationToken = default)
    {
        var blobClient = GetBlobClient(key);
        await _containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        if (value is byte[] bytes)
        {
            await blobClient.UploadAsync(new BinaryData(bytes), overwrite: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var enc = Encoding.GetEncoding(encoding ?? _encoding);
            var data = enc.GetBytes(value?.ToString() ?? string.Empty);
            await blobClient.UploadAsync(new BinaryData(data), overwrite: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> HasAsync(string key, CancellationToken cancellationToken = default)
    {
        var blobClient = GetBlobClient(key);
        var response = await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false);
        return response.Value;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var blobClient = GetBlobClient(key);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await foreach (var blob in _containerClient.GetBlobsAsync(prefix: NullIfEmpty(_pathPrefix), cancellationToken: cancellationToken))
        {
            await _containerClient.DeleteBlobIfExistsAsync(blob.Name, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public IStorage Child(string? name)
    {
        if (name is null)
        {
            return this;
        }

        var newPrefix = string.IsNullOrEmpty(_pathPrefix) ? name : $"{_pathPrefix}/{name}";
        return new AzureBlobStorage(_containerClient.AccountName, _containerClient.Name, newPrefix, _encoding);
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> Keys()
    {
        var keys = new List<string>();
        foreach (var blob in _containerClient.GetBlobs(prefix: NullIfEmpty(_pathPrefix)))
        {
            keys.Add(StripPrefix(blob.Name));
        }

        return keys;
    }

    /// <inheritdoc/>
    public async Task<string> GetCreationDateAsync(string key, CancellationToken cancellationToken = default)
    {
        var blobClient = GetBlobClient(key);
        var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return IStorage.GetTimestampFormattedWithLocalTz(properties.Value.CreatedOn.UtcDateTime);
    }

    private static string? NullIfEmpty(string value)
    {
        return string.IsNullOrEmpty(value) ? null : value;
    }

    private BlobClient GetBlobClient(string key)
    {
        var blobName = string.IsNullOrEmpty(_pathPrefix) ? key : $"{_pathPrefix}/{key}";
        return _containerClient.GetBlobClient(blobName);
    }

    private string StripPrefix(string blobName)
    {
        if (!string.IsNullOrEmpty(_pathPrefix) && blobName.StartsWith(_pathPrefix + "/", StringComparison.Ordinal))
        {
            return blobName[(_pathPrefix.Length + 1)..];
        }

        return blobName;
    }
}
