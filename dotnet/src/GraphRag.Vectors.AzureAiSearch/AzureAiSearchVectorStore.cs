// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using Azure;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using GraphRag.Common.Discovery;
using GraphRag.Vectors.Filtering;

namespace GraphRag.Vectors.AzureAiSearch;

/// <summary>
/// An <see cref="IVectorStore"/> implementation backed by Azure AI Search.
/// </summary>
[StrategyImplementation("azure_ai_search", typeof(IVectorStore))]
public sealed class AzureAiSearchVectorStore : IVectorStore
{
    private readonly SearchIndexClient _indexClient;
    private readonly IndexSchema _schema;
    private readonly VectorStoreConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAiSearchVectorStore"/> class.
    /// </summary>
    /// <param name="config">The vector store configuration.</param>
    /// <param name="schema">The index schema.</param>
    public AzureAiSearchVectorStore(VectorStoreConfig config, IndexSchema schema)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(schema);

        _config = config;
        _schema = schema;

        var endpoint = new Uri(config.Url ?? throw new ArgumentException("Url is required.", nameof(config)));

        _indexClient = string.IsNullOrEmpty(config.ApiKey)
            ? new SearchIndexClient(endpoint, new DefaultAzureCredential())
            : new SearchIndexClient(endpoint, new AzureKeyCredential(config.ApiKey));
    }

    /// <inheritdoc/>
    public Task ConnectAsync(CancellationToken cancellationToken)
    {
        // Azure AI Search is a managed service; no explicit connection step is required.
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task CreateIndexAsync(CancellationToken cancellationToken)
    {
        var fields = new List<SearchField>
        {
            new SimpleField(_schema.IdField, SearchFieldDataType.String) { IsKey = true, IsFilterable = true },
            new SearchField(_schema.VectorField, SearchFieldDataType.Collection(SearchFieldDataType.Single))
            {
                IsSearchable = true,
                VectorSearchDimensions = _schema.VectorSize,
                VectorSearchProfileName = "default-profile",
            },
        };

        if (_schema.Fields is not null)
        {
            foreach (var (name, type) in _schema.Fields)
            {
                fields.Add(new SimpleField(name, MapFieldType(type)) { IsFilterable = true });
            }
        }

        var index = new SearchIndex(_schema.IndexName)
        {
            Fields = fields,
            VectorSearch = new VectorSearch
            {
                Profiles = { new VectorSearchProfile("default-profile", "default-hnsw") },
                Algorithms = { new HnswAlgorithmConfiguration("default-hnsw") },
            },
        };

        await _indexClient.CreateOrUpdateIndexAsync(index, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task LoadDocumentsAsync(IEnumerable<VectorStoreDocument> documents, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(documents);

        var client = _indexClient.GetSearchClient(_schema.IndexName);
        var batch = new List<SearchDocument>();

        foreach (var doc in documents)
        {
            batch.Add(ToSearchDocument(doc));

            if (batch.Count >= 1000)
            {
                await client.MergeOrUploadDocumentsAsync(batch, cancellationToken: cancellationToken).ConfigureAwait(false);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await client.MergeOrUploadDocumentsAsync(batch, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task InsertAsync(VectorStoreDocument document, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        var client = _indexClient.GetSearchClient(_schema.IndexName);
        await client.UploadDocumentsAsync(
            new[] { ToSearchDocument(document) },
            cancellationToken: cancellationToken).ConfigureAwait(false);
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

        var client = _indexClient.GetSearchClient(_schema.IndexName);

        var vectorQuery = new VectorizedQuery(queryEmbedding.ToArray())
        {
            KNearestNeighborsCount = k,
        };
        vectorQuery.Fields.Add(_schema.VectorField);

        var options = new SearchOptions
        {
            VectorSearch = new VectorSearchOptions
            {
                Queries = { vectorQuery },
            },
            Size = k,
        };

        if (selectFields is not null)
        {
            foreach (var field in selectFields)
            {
                options.Select.Add(field);
            }
        }

        if (filters is not null)
        {
            options.Filter = FilterToOData(filters);
        }

        var response = await client.SearchAsync<SearchDocument>(null, options, cancellationToken).ConfigureAwait(false);
        var results = new List<VectorStoreSearchResult>();

        await foreach (var result in response.Value.GetResultsAsync().ConfigureAwait(false))
        {
            results.Add(new VectorStoreSearchResult(FromSearchDocument(result.Document, includeVectors), result.Score ?? 0.0));
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

        var client = _indexClient.GetSearchClient(_schema.IndexName);

        var options = new GetDocumentOptions();
        if (selectFields is not null)
        {
            foreach (var field in selectFields)
            {
                options.SelectedFields.Add(field);
            }
        }

        try
        {
            var doc = await client.GetDocumentAsync<SearchDocument>(id, options, cancellationToken).ConfigureAwait(false);
            return new VectorStoreSearchResult(FromSearchDocument(doc.Value, includeVectors), 1.0);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        var client = _indexClient.GetSearchClient(_schema.IndexName);
        var response = await client.GetDocumentCountAsync(cancellationToken).ConfigureAwait(false);
        return (int)response.Value;
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(IEnumerable<string> ids, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ids);

        var client = _indexClient.GetSearchClient(_schema.IndexName);
        var documents = ids.Select(id =>
        {
            var doc = new SearchDocument();
            doc[_schema.IdField] = id;
            return doc;
        }).ToList();

        if (documents.Count > 0)
        {
            await client.DeleteDocumentsAsync(documents, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(VectorStoreDocument document, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        var client = _indexClient.GetSearchClient(_schema.IndexName);
        await client.MergeOrUploadDocumentsAsync(
            new[] { ToSearchDocument(document) },
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static SearchFieldDataType MapFieldType(string type) => type.ToUpperInvariant() switch
    {
        "STRING" => SearchFieldDataType.String,
        "INT32" => SearchFieldDataType.Int32,
        "INT64" => SearchFieldDataType.Int64,
        "DOUBLE" => SearchFieldDataType.Double,
        "BOOLEAN" => SearchFieldDataType.Boolean,
        "DATETIME" => SearchFieldDataType.DateTimeOffset,
        _ => SearchFieldDataType.String,
    };

    private static string? FilterToOData(FilterExpression filter) => filter switch
    {
        Condition c => $"{c.Field} {ODataOp(c.Op)} '{c.Value}'",
        AndExpression a => $"({FilterToOData(a.Left)} and {FilterToOData(a.Right)})",
        OrExpression o => $"({FilterToOData(o.Left)} or {FilterToOData(o.Right)})",
        NotExpression n => $"not ({FilterToOData(n.Expression)})",
        _ => null,
    };

    private static string ODataOp(string op) => op switch
    {
        "eq" => "eq",
        "ne" => "ne",
        "gt" => "gt",
        "ge" or "gte" => "ge",
        "lt" => "lt",
        "le" or "lte" => "le",
        _ => "eq",
    };

    private SearchDocument ToSearchDocument(VectorStoreDocument doc)
    {
        var searchDoc = new SearchDocument
        {
            [_schema.IdField] = doc.Id,
        };

        if (doc.Vector is not null)
        {
            searchDoc[_schema.VectorField] = doc.Vector.ToArray();
        }

        foreach (var (key, value) in doc.Data)
        {
            searchDoc[key] = value;
        }

        return searchDoc;
    }

    private VectorStoreDocument FromSearchDocument(SearchDocument searchDoc, bool includeVectors)
    {
        var data = new Dictionary<string, object?>();
        IReadOnlyList<float>? vector = null;

        foreach (var field in searchDoc)
        {
            if (field.Key == _schema.IdField || field.Key == _schema.VectorField)
            {
                continue;
            }

            data[field.Key] = field.Value;
        }

        if (includeVectors && searchDoc.TryGetValue(_schema.VectorField, out var vecObj) && vecObj is IEnumerable<float> vec)
        {
            vector = vec.ToList();
        }

        return new VectorStoreDocument
        {
            Id = searchDoc[_schema.IdField]?.ToString() ?? string.Empty,
            Vector = vector,
            Data = data,
        };
    }
}
