// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Vectors.Filtering;

namespace GraphRag.Vectors;

/// <summary>
/// Provides an interface for interacting with a vector store.
/// </summary>
public interface IVectorStore
{
    /// <summary>
    /// Connects to the vector store.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ConnectAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Creates the index in the vector store.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateIndexAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Loads a batch of documents into the vector store.
    /// </summary>
    /// <param name="documents">The documents to load.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task LoadDocumentsAsync(IEnumerable<VectorStoreDocument> documents, CancellationToken cancellationToken);

    /// <summary>
    /// Inserts a single document into the vector store.
    /// </summary>
    /// <param name="document">The document to insert.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertAsync(VectorStoreDocument document, CancellationToken cancellationToken);

    /// <summary>
    /// Performs a similarity search using a query embedding vector.
    /// </summary>
    /// <param name="queryEmbedding">The query embedding vector.</param>
    /// <param name="k">The number of results to return.</param>
    /// <param name="selectFields">Optional list of fields to include in the results.</param>
    /// <param name="filters">Optional filter expression to apply.</param>
    /// <param name="includeVectors">Whether to include vectors in the results.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of search results ordered by similarity.</returns>
    Task<IReadOnlyList<VectorStoreSearchResult>> SimilaritySearchByVectorAsync(
        IReadOnlyList<float> queryEmbedding,
        int k = 10,
        IReadOnlyList<string>? selectFields = null,
        FilterExpression? filters = null,
        bool includeVectors = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for a document by its identifier.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <param name="selectFields">Optional list of fields to include in the result.</param>
    /// <param name="includeVectors">Whether to include the vector in the result.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The matching search result, or <c>null</c> if not found.</returns>
    Task<VectorStoreSearchResult?> SearchByIdAsync(
        string id,
        IReadOnlyList<string>? selectFields = null,
        bool includeVectors = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the number of documents in the vector store.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The document count.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Removes documents by their identifiers.
    /// </summary>
    /// <param name="ids">The identifiers of documents to remove.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(IEnumerable<string> ids, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing document in the vector store.
    /// </summary>
    /// <param name="document">The document with updated data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(VectorStoreDocument document, CancellationToken cancellationToken);
}
