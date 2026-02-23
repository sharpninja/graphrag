// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Input;

/// <summary>
/// Defines an interface for reading input documents from a source.
/// </summary>
public interface IInputReader
{
    /// <summary>
    /// Reads all input files and returns them as a list of <see cref="TextDocument"/> instances.
    /// </summary>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A list of text documents.</returns>
    Task<List<TextDocument>> ReadFilesAsync(CancellationToken ct = default);

    /// <summary>
    /// Reads input files as an asynchronous stream of <see cref="TextDocument"/> instances.
    /// </summary>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>An asynchronous enumerable of text documents.</returns>
    IAsyncEnumerable<TextDocument> ReadAsync(CancellationToken ct = default);
}
