// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;

namespace GraphRag.Storage;

/// <summary>
/// A factory class for creating <see cref="IStorage"/> instances by strategy name.
/// Supports lazy registration of builtin storage types.
/// </summary>
public class StorageFactory : ServiceFactory<IStorage>
{
    /// <summary>
    /// Create a storage instance based on the given configuration, lazily registering builtin types.
    /// </summary>
    /// <param name="config">The storage configuration to use.</param>
    /// <returns>The created storage implementation.</returns>
    /// <exception cref="InvalidOperationException">If the storage type is not registered and not a known builtin.</exception>
    public IStorage CreateStorage(StorageConfig config)
    {
        var strategy = config.Type;
        if (!Contains(strategy))
        {
            RegisterBuiltin(strategy);
        }

        var args = new Dictionary<string, object?>
        {
            ["type"] = config.Type,
            ["encoding"] = config.Encoding,
            ["base_dir"] = config.BaseDir,
            ["connection_string"] = config.ConnectionString,
            ["container_name"] = config.ContainerName,
            ["account_url"] = config.AccountUrl,
            ["database_name"] = config.DatabaseName,
        };

        return Create(strategy, args);
    }

    private void RegisterBuiltin(string strategy)
    {
        switch (strategy)
        {
            case StorageType.File:
                Register(StorageType.File, args =>
                    new FileStorage(
                        baseDir: args.TryGetValue("base_dir", out var bd) ? bd?.ToString() ?? "." : ".",
                        encoding: args.TryGetValue("encoding", out var enc) ? enc?.ToString() ?? "utf-8" : "utf-8"));
                break;

            case StorageType.Memory:
                Register(StorageType.Memory, _ => new MemoryStorage());
                break;

            case StorageType.AzureBlob:
                Register(StorageType.AzureBlob, _ =>
                    throw new NotImplementedException("Azure Blob storage is not yet implemented."));
                break;

            case StorageType.AzureCosmos:
                Register(StorageType.AzureCosmos, _ =>
                    throw new NotImplementedException("Azure Cosmos DB storage is not yet implemented."));
                break;

            default:
                var registered = string.Join(", ", Keys);
                throw new InvalidOperationException(
                    $"StorageConfig.Type '{strategy}' is not registered in the StorageFactory. Registered types: {registered}.");
        }
    }
}
