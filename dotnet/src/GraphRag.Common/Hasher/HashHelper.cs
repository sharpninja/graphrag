// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Common.Hasher;

/// <summary>
/// Delegate type for a hasher function.
/// </summary>
/// <param name="data">The data to hash.</param>
/// <returns>The resulting hash string.</returns>
public delegate string HasherFunc(string data);

/// <summary>
/// Hashing utilities for GraphRAG.
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// Generate a SHA-256 hash for the input data.
    /// </summary>
    /// <param name="data">The string data to hash.</param>
    /// <returns>The hex-encoded SHA-256 hash.</returns>
    public static string Sha256Hash(string data)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(data);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        return Convert.ToHexStringLower(hash);
    }

    /// <summary>
    /// Hash the input data using YAML serialization and the specified hasher.
    /// </summary>
    /// <param name="data">The data to hash. Will be serialized to YAML.</param>
    /// <param name="hasher">Optional hasher function. Defaults to SHA-256.</param>
    /// <returns>The resulting hash string.</returns>
    public static string HashData(object? data, HasherFunc? hasher = null)
    {
        hasher ??= Sha256Hash;

        var serializer = new YamlDotNet.Serialization.SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(MakeYamlSerializable(data) ?? string.Empty);
        return hasher(yaml);
    }

    /// <summary>
    /// Convert data to a YAML-serializable format.
    /// </summary>
    /// <param name="data">The data to convert.</param>
    /// <returns>A YAML-serializable representation of the data.</returns>
    public static object? MakeYamlSerializable(object? data)
    {
        return data switch
        {
            null => null,
            string s => s,
            IDictionary<string, object?> dict => dict
                .OrderBy(kv => kv.Key)
                .Select(kv => new KeyValuePair<string, object?>(kv.Key, MakeYamlSerializable(kv.Value)))
                .ToDictionary(kv => kv.Key, kv => kv.Value),
            System.Collections.IEnumerable enumerable => enumerable
                .Cast<object?>()
                .Select(MakeYamlSerializable)
                .ToList(),
            _ => data.ToString(),
        };
    }
}
