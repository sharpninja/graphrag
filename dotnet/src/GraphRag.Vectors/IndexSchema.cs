// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Vectors;

/// <summary>
/// Describes the schema of a vector store index.
/// </summary>
public sealed record IndexSchema
{
    /// <summary>
    /// Gets the name of the index.
    /// </summary>
    public required string IndexName { get; init; }

    /// <summary>
    /// Gets the name of the identifier field.
    /// </summary>
    public string IdField { get; init; } = "id";

    /// <summary>
    /// Gets the name of the vector field.
    /// </summary>
    public string VectorField { get; init; } = "vector";

    /// <summary>
    /// Gets the size of the vector.
    /// </summary>
    public int VectorSize { get; init; } = 3072;

    /// <summary>
    /// Gets the mapping of field names to their types.
    /// </summary>
    public Dictionary<string, string>? Fields { get; init; }

    /// <summary>
    /// Returns a copy of the schema with the specified vector size.
    /// </summary>
    /// <param name="vectorSize">The vector dimension to apply.</param>
    /// <returns>A copy of the schema with the updated vector size.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="vectorSize"/> is less than or equal to zero.</exception>
    public IndexSchema WithVectorSize(int vectorSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(vectorSize);
        return this with { VectorSize = vectorSize };
    }
}
