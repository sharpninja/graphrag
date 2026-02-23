// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;
using GraphRag.Storage;

namespace GraphRag.Input;

/// <summary>
/// A factory class for creating <see cref="IInputReader"/> instances by strategy name.
/// Supports lazy registration of builtin input reader types.
/// </summary>
public class InputReaderFactory : ServiceFactory<IInputReader>
{
    /// <summary>
    /// Creates an input reader instance based on the given configuration and storage.
    /// </summary>
    /// <param name="config">The input configuration to use.</param>
    /// <param name="storage">The storage instance to read files from.</param>
    /// <returns>The created input reader implementation.</returns>
    /// <exception cref="InvalidOperationException">If the input type is not registered and not a known builtin.</exception>
    public IInputReader CreateInputReader(InputConfig config, IStorage storage)
    {
        var strategy = config.Type;
        if (!Contains(strategy))
        {
            RegisterBuiltin(strategy, config, storage);
        }

        var args = new Dictionary<string, object?>
        {
            ["type"] = config.Type,
            ["encoding"] = config.Encoding,
            ["file_pattern"] = config.FilePattern,
            ["id_column"] = config.IdColumn,
            ["title_column"] = config.TitleColumn,
            ["text_column"] = config.TextColumn,
            ["storage"] = storage,
        };

        return Create(strategy, args);
    }

    private void RegisterBuiltin(string strategy, InputConfig config, IStorage storage)
    {
        switch (strategy)
        {
            case InputType.Text:
                Register(InputType.Text, args => new TextFileReader(
                    storage: args.TryGetValue("storage", out var s) ? (IStorage)s! : storage,
                    encoding: args.TryGetValue("encoding", out var enc) ? enc?.ToString() : null,
                    filePattern: args.TryGetValue("file_pattern", out var fp) ? fp?.ToString() : null));
                break;

            case InputType.Csv:
                Register(InputType.Csv, args => new CsvFileReader(
                    storage: args.TryGetValue("storage", out var s) ? (IStorage)s! : storage,
                    encoding: args.TryGetValue("encoding", out var enc) ? enc?.ToString() : null,
                    filePattern: args.TryGetValue("file_pattern", out var fp) ? fp?.ToString() : null,
                    idColumn: args.TryGetValue("id_column", out var id) ? id?.ToString() : null,
                    titleColumn: args.TryGetValue("title_column", out var tc) ? tc?.ToString() : null,
                    textColumn: args.TryGetValue("text_column", out var tx) ? tx?.ToString() : null));
                break;

            case InputType.Json:
                Register(InputType.Json, args => new JsonFileReader(
                    storage: args.TryGetValue("storage", out var s) ? (IStorage)s! : storage,
                    encoding: args.TryGetValue("encoding", out var enc) ? enc?.ToString() : null,
                    filePattern: args.TryGetValue("file_pattern", out var fp) ? fp?.ToString() : null,
                    idColumn: args.TryGetValue("id_column", out var id) ? id?.ToString() : null,
                    titleColumn: args.TryGetValue("title_column", out var tc) ? tc?.ToString() : null,
                    textColumn: args.TryGetValue("text_column", out var tx) ? tx?.ToString() : null));
                break;

            case InputType.JsonLines:
                Register(InputType.JsonLines, args => new JsonLinesFileReader(
                    storage: args.TryGetValue("storage", out var s) ? (IStorage)s! : storage,
                    encoding: args.TryGetValue("encoding", out var enc) ? enc?.ToString() : null,
                    filePattern: args.TryGetValue("file_pattern", out var fp) ? fp?.ToString() : null,
                    idColumn: args.TryGetValue("id_column", out var id) ? id?.ToString() : null,
                    titleColumn: args.TryGetValue("title_column", out var tc) ? tc?.ToString() : null,
                    textColumn: args.TryGetValue("text_column", out var tx) ? tx?.ToString() : null));
                break;

            case InputType.MarkItDown:
                Register(InputType.MarkItDown, _ =>
                    throw new NotImplementedException("MarkItDown input reader is not yet implemented."));
                break;

            default:
                var registered = string.Join(", ", Keys);
                throw new InvalidOperationException(
                    $"InputConfig.Type '{strategy}' is not registered in the InputReaderFactory. Registered types: {registered}.");
        }
    }
}
