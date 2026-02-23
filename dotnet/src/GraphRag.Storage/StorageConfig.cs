// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Storage;

/// <summary>
/// The default configuration section for storage.
/// </summary>
public sealed record StorageConfig
{
    /// <summary>Gets the storage type to use. Builtin types include "file", "blob", and "cosmosdb".</summary>
    public string Type { get; init; } = StorageType.File;

    /// <summary>Gets the encoding to use for file storage.</summary>
    public string? Encoding { get; init; }

    /// <summary>Gets the base directory for the output when using file or Azure Blob storage.</summary>
    public string? BaseDir { get; init; }

    /// <summary>Gets the connection string for remote services.</summary>
    public string? ConnectionString { get; init; }

    /// <summary>Gets the Azure Blob Storage container name or CosmosDB container name to use.</summary>
    public string? ContainerName { get; init; }

    /// <summary>Gets the account URL for Azure services.</summary>
    public string? AccountUrl { get; init; }

    /// <summary>Gets the database name to use.</summary>
    public string? DatabaseName { get; init; }
}
