// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;

namespace GraphRag.Storage.Tables;

/// <summary>
/// A factory class for creating <see cref="ITableProvider"/> instances by strategy name.
/// Supports lazy registration of builtin table types.
/// </summary>
public class TableProviderFactory : ServiceFactory<ITableProvider>
{
    /// <summary>
    /// Create a table provider instance based on the given configuration, lazily registering builtin types.
    /// </summary>
    /// <param name="config">The table provider configuration to use.</param>
    /// <param name="storage">Optional storage implementation for file-based table providers.</param>
    /// <returns>The created table provider implementation.</returns>
    /// <exception cref="InvalidOperationException">If the table type is not registered and not a known builtin.</exception>
    public ITableProvider CreateTableProvider(TableProviderConfig config, IStorage? storage = null)
    {
        var strategy = config.Type;
        if (!Contains(strategy))
        {
            RegisterBuiltin(strategy);
        }

        var args = new Dictionary<string, object?>
        {
            ["type"] = config.Type,
        };

        if (storage is not null)
        {
            args["storage"] = storage;
        }

        return Create(strategy, args);
    }

    private void RegisterBuiltin(string strategy)
    {
        switch (strategy)
        {
            case TableType.Parquet:
                Register(TableType.Parquet, _ =>
                    throw new NotImplementedException("Parquet table provider is not yet implemented."));
                break;

            case TableType.Csv:
                Register(TableType.Csv, _ =>
                    throw new NotImplementedException("CSV table provider is not yet implemented."));
                break;

            default:
                var registered = string.Join(", ", Keys);
                throw new InvalidOperationException(
                    $"TableProviderConfig.Type '{strategy}' is not registered in the TableProviderFactory. Registered types: {registered}.");
        }
    }
}
