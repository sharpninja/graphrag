// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Storage;

/// <summary>
/// Builtin storage implementation types.
/// </summary>
public static class StorageType
{
    /// <summary>Gets the identifier for file-based storage.</summary>
    public const string File = "file";

    /// <summary>Gets the identifier for in-memory storage.</summary>
    public const string Memory = "memory";

    /// <summary>Gets the identifier for Azure Blob storage.</summary>
    public const string AzureBlob = "blob";

    /// <summary>Gets the identifier for Azure Cosmos DB storage.</summary>
    public const string AzureCosmos = "cosmosdb";
}
