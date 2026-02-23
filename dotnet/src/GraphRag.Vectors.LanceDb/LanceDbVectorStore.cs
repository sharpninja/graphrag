// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Discovery;
using GraphRag.Vectors.Filtering;

namespace GraphRag.Vectors.LanceDb;

/// <summary>
/// An <see cref="IVectorStore"/> placeholder implementation for LanceDB.
/// </summary>
/// <remarks>
/// TODO: Implement when an official LanceDB .NET SDK becomes available.
/// </remarks>
[StrategyImplementation("lancedb", typeof(IVectorStore))]
public sealed class LanceDbVectorStore : IVectorStore
{
    private const string NotAvailableMessage = "LanceDB .NET client not yet available. Use Azure AI Search or Cosmos DB.";

    private readonly string _databaseUri;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanceDbVectorStore"/> class.
    /// </summary>
    /// <param name="databaseUri">The URI of the LanceDB database location.</param>
    public LanceDbVectorStore(string databaseUri)
    {
        ArgumentException.ThrowIfNullOrEmpty(databaseUri);
        _databaseUri = databaseUri;
    }

    /// <inheritdoc/>
    public Task ConnectAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException(NotAvailableMessage);

    /// <inheritdoc/>
    public Task CreateIndexAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException(NotAvailableMessage);

    /// <inheritdoc/>
    public Task LoadDocumentsAsync(IEnumerable<VectorStoreDocument> documents, CancellationToken cancellationToken) =>
        throw new NotImplementedException(NotAvailableMessage);

    /// <inheritdoc/>
    public Task InsertAsync(VectorStoreDocument document, CancellationToken cancellationToken) =>
        throw new NotImplementedException(NotAvailableMessage);

    /// <inheritdoc/>
    public Task<IReadOnlyList<VectorStoreSearchResult>> SimilaritySearchByVectorAsync(
        IReadOnlyList<float> queryEmbedding,
        int k = 10,
        IReadOnlyList<string>? selectFields = null,
        FilterExpression? filters = null,
        bool includeVectors = true,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException(NotAvailableMessage);

    /// <inheritdoc/>
    public Task<VectorStoreSearchResult?> SearchByIdAsync(
        string id,
        IReadOnlyList<string>? selectFields = null,
        bool includeVectors = true,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException(NotAvailableMessage);

    /// <inheritdoc/>
    public Task<int> CountAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException(NotAvailableMessage);

    /// <inheritdoc/>
    public Task RemoveAsync(IEnumerable<string> ids, CancellationToken cancellationToken) =>
        throw new NotImplementedException(NotAvailableMessage);

    /// <inheritdoc/>
    public Task UpdateAsync(VectorStoreDocument document, CancellationToken cancellationToken) =>
        throw new NotImplementedException(NotAvailableMessage);
}
