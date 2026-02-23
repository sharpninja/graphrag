// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;

namespace GraphRag.Chunking;

/// <summary>
/// A factory class for creating <see cref="IChunker"/> instances by strategy name.
/// Supports lazy registration of builtin chunker types.
/// </summary>
public class ChunkerFactory : ServiceFactory<IChunker>
{
    /// <summary>
    /// Create a chunker instance based on the given configuration, lazily registering builtin types.
    /// </summary>
    /// <param name="config">The chunking configuration to use, or <c>null</c> for defaults.</param>
    /// <param name="encode">An optional encoding function that converts text to token IDs.</param>
    /// <param name="decode">An optional decoding function that converts token IDs back to text.</param>
    /// <returns>The created chunker implementation.</returns>
    /// <exception cref="InvalidOperationException">If the chunker type is not registered and not a known builtin.</exception>
    public IChunker CreateChunker(
        ChunkingConfig? config = null,
        Func<string, IReadOnlyList<int>>? encode = null,
        Func<IReadOnlyList<int>, string>? decode = null)
    {
        config ??= new ChunkingConfig();
        var strategy = config.Type;

        if (!Contains(strategy))
        {
            RegisterBuiltin(strategy);
        }

        var args = new Dictionary<string, object?>
        {
            ["size"] = config.Size,
            ["overlap"] = config.Overlap,
            ["encode"] = encode,
            ["decode"] = decode,
        };

        return Create(strategy, args);
    }

    private void RegisterBuiltin(string strategy)
    {
        switch (strategy)
        {
            case ChunkerType.Tokens:
                Register(ChunkerType.Tokens, args =>
                    new TokenChunker(
                        (int)args["size"]!,
                        (int)args["overlap"]!,
                        (Func<string, IReadOnlyList<int>>)args["encode"]!,
                        (Func<IReadOnlyList<int>, string>)args["decode"]!));
                break;

            case ChunkerType.Sentence:
                Register(ChunkerType.Sentence, args =>
                    new SentenceChunker(
                        args.TryGetValue("encode", out var enc)
                            ? enc as Func<string, IReadOnlyList<int>>
                            : null));
                break;

            default:
                var registered = string.Join(", ", Keys);
                throw new InvalidOperationException(
                    $"ChunkingConfig.Type '{strategy}' is not registered in the ChunkerFactory. Registered types: {registered}.");
        }
    }
}
