// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;

namespace GraphRag.Vectors;

/// <summary>
/// Factory for creating <see cref="IVectorStore"/> instances by store type.
/// </summary>
public class VectorStoreFactory : ServiceFactory<IVectorStore>
{
    private bool _builtinsRegistered;

    /// <summary>
    /// Creates a vector store instance for the given configuration and schema.
    /// </summary>
    /// <param name="config">The vector store configuration.</param>
    /// <param name="schema">The index schema.</param>
    /// <returns>An <see cref="IVectorStore"/> instance.</returns>
    public IVectorStore CreateVectorStore(VectorStoreConfig config, IndexSchema schema)
    {
        EnsureBuiltinsRegistered();

        return Create(config.Type, new Dictionary<string, object?>
        {
            ["config"] = config,
            ["schema"] = schema,
        });
    }

    private void EnsureBuiltinsRegistered()
    {
        if (_builtinsRegistered)
        {
            return;
        }

        _builtinsRegistered = true;

        Register(VectorStoreType.LanceDb, _ =>
            throw new NotImplementedException($"The '{VectorStoreType.LanceDb}' vector store is not yet implemented."));

        Register(VectorStoreType.AzureAiSearch, _ =>
            throw new NotImplementedException($"The '{VectorStoreType.AzureAiSearch}' vector store is not yet implemented."));

        Register(VectorStoreType.CosmosDb, _ =>
            throw new NotImplementedException($"The '{VectorStoreType.CosmosDb}' vector store is not yet implemented."));
    }
}
