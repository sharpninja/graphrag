// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Index.Typing;

/// <summary>
/// Represents the mutable state carried through a pipeline run.
/// </summary>
/// <remarks>
/// This is a type alias for <see cref="Dictionary{TKey, TValue}"/> with string keys and nullable object values.
/// </remarks>
public static class PipelineState
{
    /// <summary>
    /// Creates a new, empty pipeline state dictionary.
    /// </summary>
    /// <returns>A new <see cref="Dictionary{TKey, TValue}"/> instance.</returns>
    public static Dictionary<string, object?> Create() => new();
}
