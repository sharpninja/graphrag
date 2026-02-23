// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Net;
using System.Text.RegularExpressions;

using GraphRag.Common.Discovery;

using Microsoft.Azure.Cosmos;

using Newtonsoft.Json.Linq;

namespace GraphRag.Storage.AzureCosmos;

/// <summary>
/// Azure Cosmos DB implementation of <see cref="IStorage"/>.
/// </summary>
[StrategyImplementation("cosmosdb", typeof(IStorage))]
public sealed class AzureCosmosStorage : IStorage, IDisposable
{
    private readonly CosmosClient _cosmosClient;
    private readonly Database _database;
    private readonly Container _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCosmosStorage"/> class.
    /// </summary>
    /// <param name="connectionString">The Cosmos DB connection string.</param>
    /// <param name="databaseName">The name of the database.</param>
    /// <param name="containerName">The name of the container.</param>
    public AzureCosmosStorage(string connectionString, string databaseName, string containerName)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        ArgumentException.ThrowIfNullOrEmpty(databaseName);
        ArgumentException.ThrowIfNullOrEmpty(containerName);
        _cosmosClient = new CosmosClient(connectionString);
        _database = _cosmosClient.GetDatabase(databaseName);
        _container = _database.GetContainer(containerName);
    }

    /// <inheritdoc/>
    public IEnumerable<string> Find(Regex filePattern)
    {
        var query = new QueryDefinition("SELECT c.id FROM c");
        using var iterator = _container.GetItemQueryIterator<JObject>(query);
        while (iterator.HasMoreResults)
        {
            var response = iterator.ReadNextAsync().GetAwaiter().GetResult();
            foreach (var item in response)
            {
                var id = item["id"]?.ToString() ?? string.Empty;
                if (filePattern.IsMatch(id))
                {
                    yield return id;
                }
            }
        }
    }

    /// <inheritdoc/>
    public async Task<object?> GetAsync(string key, bool? asBytes = null, string? encoding = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<JObject>(key, new PartitionKey(key), cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Resource["body"]?.ToString();
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task SetAsync(string key, object value, string? encoding = null, CancellationToken cancellationToken = default)
    {
        var document = new JObject
        {
            ["id"] = key,
            ["body"] = value?.ToString() ?? string.Empty,
        };

        await _container.UpsertItemAsync(document, new PartitionKey(key), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<bool> HasAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _container.ReadItemAsync<JObject>(key, new PartitionKey(key), cancellationToken: cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _container.DeleteItemAsync<JObject>(key, new PartitionKey(key), cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Item does not exist; nothing to delete.
        }
    }

    /// <inheritdoc/>
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT c.id FROM c");
        using var iterator = _container.GetItemQueryIterator<JObject>(query);
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
            foreach (var item in response)
            {
                var id = item["id"]?.ToString();
                if (id is not null)
                {
                    await _container.DeleteItemAsync<JObject>(id, new PartitionKey(id), cancellationToken: cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }

    /// <inheritdoc/>
    public IStorage Child(string? name)
    {
        if (name is null)
        {
            return this;
        }

        return new AzureCosmosStorage(_cosmosClient.Endpoint.ToString(), _database.Id, name);
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> Keys()
    {
        var keys = new List<string>();
        var query = new QueryDefinition("SELECT c.id FROM c");
        using var iterator = _container.GetItemQueryIterator<JObject>(query);
        while (iterator.HasMoreResults)
        {
            var response = iterator.ReadNextAsync().GetAwaiter().GetResult();
            foreach (var item in response)
            {
                var id = item["id"]?.ToString();
                if (id is not null)
                {
                    keys.Add(id);
                }
            }
        }

        return keys;
    }

    /// <inheritdoc/>
    public Task<string> GetCreationDateAsync(string key, CancellationToken cancellationToken = default)
    {
        // Cosmos DB does not expose a built-in creation timestamp; return current time.
        return Task.FromResult(IStorage.GetTimestampFormattedWithLocalTz(DateTime.UtcNow));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _cosmosClient.Dispose();
    }
}
