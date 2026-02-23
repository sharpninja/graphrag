// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.ObjectModel;
using GraphRag.Common.Discovery;
using GraphRag.Vectors.Filtering;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace GraphRag.Vectors.AzureCosmos;

/// <summary>
/// An <see cref="IVectorStore"/> implementation backed by Azure Cosmos DB.
/// </summary>
[StrategyImplementation("cosmosdb", typeof(IVectorStore))]
public sealed class CosmosDbVectorStore : IVectorStore, IDisposable
{
    private readonly CosmosClient _client;
    private readonly string _databaseName;
    private readonly IndexSchema _schema;
    private Database? _database;
    private Container? _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbVectorStore"/> class.
    /// </summary>
    /// <param name="config">The vector store configuration.</param>
    /// <param name="schema">The index schema.</param>
    public CosmosDbVectorStore(VectorStoreConfig config, IndexSchema schema)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(schema);

        _databaseName = config.DatabaseName ?? throw new ArgumentException("DatabaseName is required.", nameof(config));
        _schema = schema;

        var connectionString = config.ConnectionString ?? throw new ArgumentException("ConnectionString is required.", nameof(config));
        _client = new CosmosClient(connectionString);
    }

    /// <inheritdoc/>
    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        _database = _client.GetDatabase(_databaseName);
        _container = _database.GetContainer(_schema.IndexName);

        // Verify connectivity by reading the container properties.
        await _container.ReadContainerAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task CreateIndexAsync(CancellationToken cancellationToken)
    {
        _database = (await _client.CreateDatabaseIfNotExistsAsync(_databaseName, cancellationToken: cancellationToken).ConfigureAwait(false)).Database;

        var containerProperties = new ContainerProperties(_schema.IndexName, $"/{_schema.IdField}")
        {
            IndexingPolicy = new IndexingPolicy
            {
                VectorIndexes =
                {
                    new VectorIndexPath
                    {
                        Path = $"/{_schema.VectorField}",
                        Type = VectorIndexType.Flat,
                    },
                },
            },
            VectorEmbeddingPolicy = new VectorEmbeddingPolicy(
                new Collection<Embedding>(
                [
                    new Embedding
                    {
                        Path = $"/{_schema.VectorField}",
                        DataType = VectorDataType.Float32,
                        DistanceFunction = DistanceFunction.Cosine,
                        Dimensions = _schema.VectorSize,
                    },
                ])),
        };

        _container = (await _database.CreateContainerIfNotExistsAsync(containerProperties, cancellationToken: cancellationToken).ConfigureAwait(false)).Container;
    }

    /// <inheritdoc/>
    public async Task LoadDocumentsAsync(IEnumerable<VectorStoreDocument> documents, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(documents);
        EnsureContainer();

        foreach (var doc in documents)
        {
            var json = ToJObject(doc);
            await _container!.UpsertItemAsync(json, new PartitionKey(doc.Id), cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task InsertAsync(VectorStoreDocument document, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);
        EnsureContainer();

        var json = ToJObject(document);
        await _container!.CreateItemAsync(json, new PartitionKey(document.Id), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<VectorStoreSearchResult>> SimilaritySearchByVectorAsync(
        IReadOnlyList<float> queryEmbedding,
        int k = 10,
        IReadOnlyList<string>? selectFields = null,
        FilterExpression? filters = null,
        bool includeVectors = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryEmbedding);
        EnsureContainer();

        var vectorLiteral = $"[{string.Join(",", queryEmbedding)}]";
        var selectClause = selectFields is not null && selectFields.Count > 0
            ? string.Join(", ", selectFields.Select(f => $"c.{f}"))
            : "c";

        var whereClause = filters is not null ? $" WHERE {FilterToSql(filters)}" : string.Empty;

        var sql = $"SELECT TOP {k} {selectClause}, VectorDistance(c.{_schema.VectorField}, {vectorLiteral}) AS score FROM c{whereClause} ORDER BY VectorDistance(c.{_schema.VectorField}, {vectorLiteral})";

        var query = new QueryDefinition(sql);
        var iterator = _container!.GetItemQueryIterator<JObject>(query);

        var results = new List<VectorStoreSearchResult>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
            foreach (var item in response)
            {
                results.Add(new VectorStoreSearchResult(FromJObject(item, includeVectors), item.Value<double>("score")));
            }
        }

        return results;
    }

    /// <inheritdoc/>
    public async Task<VectorStoreSearchResult?> SearchByIdAsync(
        string id,
        IReadOnlyList<string>? selectFields = null,
        bool includeVectors = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        EnsureContainer();

        try
        {
            var response = await _container!.ReadItemAsync<JObject>(id, new PartitionKey(id), cancellationToken: cancellationToken).ConfigureAwait(false);
            return new VectorStoreSearchResult(FromJObject(response.Resource, includeVectors), 1.0);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        EnsureContainer();

        var query = new QueryDefinition("SELECT VALUE COUNT(1) FROM c");
        var iterator = _container!.GetItemQueryIterator<int>(query);

        var response = await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
        return response.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(IEnumerable<string> ids, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ids);
        EnsureContainer();

        foreach (var id in ids)
        {
            try
            {
                await _container!.DeleteItemAsync<JObject>(id, new PartitionKey(id), cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Item already removed; nothing to do.
            }
        }
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(VectorStoreDocument document, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);
        EnsureContainer();

        var json = ToJObject(document);
        await _container!.UpsertItemAsync(json, new PartitionKey(document.Id), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _client.Dispose();
    }

    private static string? FilterToSql(FilterExpression filter) => filter switch
    {
        Condition c => $"c.{c.Field} {SqlOp(c.Op)} '{c.Value}'",
        AndExpression a => $"({FilterToSql(a.Left)} AND {FilterToSql(a.Right)})",
        OrExpression o => $"({FilterToSql(o.Left)} OR {FilterToSql(o.Right)})",
        NotExpression n => $"NOT ({FilterToSql(n.Expression)})",
        _ => null,
    };

    private static string SqlOp(string op) => op switch
    {
        "eq" => "=",
        "ne" => "!=",
        "gt" => ">",
        "ge" or "gte" => ">=",
        "lt" => "<",
        "le" or "lte" => "<=",
        _ => "=",
    };

    private JObject ToJObject(VectorStoreDocument doc)
    {
        var obj = new JObject
        {
            [_schema.IdField] = doc.Id,
        };

        if (doc.Vector is not null)
        {
            obj[_schema.VectorField] = new JArray(doc.Vector.ToArray());
        }

        foreach (var (key, value) in doc.Data)
        {
            obj[key] = value is not null ? JToken.FromObject(value) : JValue.CreateNull();
        }

        return obj;
    }

    private VectorStoreDocument FromJObject(JObject obj, bool includeVectors)
    {
        var data = new Dictionary<string, object?>();
        IReadOnlyList<float>? vector = null;

        foreach (var prop in obj.Properties())
        {
            if (prop.Name == _schema.IdField || prop.Name == _schema.VectorField || prop.Name == "score")
            {
                continue;
            }

            // Skip Cosmos DB system properties.
            if (prop.Name.StartsWith('_'))
            {
                continue;
            }

            data[prop.Name] = prop.Value.Type == JTokenType.Null ? null : prop.Value.ToObject<object>();
        }

        if (includeVectors && obj.TryGetValue(_schema.VectorField, out var vecToken))
        {
            vector = vecToken.ToObject<List<float>>();
        }

        return new VectorStoreDocument
        {
            Id = obj.Value<string>(_schema.IdField) ?? string.Empty,
            Vector = vector,
            Data = data,
        };
    }

    private void EnsureContainer()
    {
        if (_container is null)
        {
            throw new InvalidOperationException("Call ConnectAsync or CreateIndexAsync before performing operations.");
        }
    }
}
