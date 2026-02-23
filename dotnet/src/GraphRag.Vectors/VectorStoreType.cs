// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Vectors;

/// <summary>
/// Known vector store type identifiers.
/// </summary>
public static class VectorStoreType
{
    /// <summary>
    /// LanceDB vector store.
    /// </summary>
    public const string LanceDb = "lancedb";

    /// <summary>
    /// Azure AI Search vector store.
    /// </summary>
    public const string AzureAiSearch = "azure_ai_search";

    /// <summary>
    /// Azure Cosmos DB vector store.
    /// </summary>
    public const string CosmosDb = "cosmosdb";
}
