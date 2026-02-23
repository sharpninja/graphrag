// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Discovery;
using SharpToken;

namespace GraphRag.Llm.SharpToken;

/// <summary>
/// A tokenizer implementation backed by the SharpToken library using BPE encodings.
/// </summary>
[StrategyImplementation("sharptoken", typeof(ITokenizer))]
public sealed class SharpTokenTokenizer : ITokenizer
{
    private readonly GptEncoding _encoding;

    /// <summary>
    /// Initializes a new instance of the <see cref="SharpTokenTokenizer"/> class.
    /// </summary>
    /// <param name="encodingName">The name of the BPE encoding to use (default is <c>"cl100k_base"</c>).</param>
    public SharpTokenTokenizer(string encodingName = "cl100k_base")
    {
        ArgumentException.ThrowIfNullOrEmpty(encodingName);
        _encoding = GptEncoding.GetEncoding(encodingName);
    }

    /// <inheritdoc />
    public IReadOnlyList<int> Encode(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return _encoding.Encode(text);
    }

    /// <inheritdoc />
    public string Decode(IReadOnlyList<int> tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);
        return _encoding.Decode(tokens is List<int> list ? list : [.. tokens]);
    }

    /// <inheritdoc />
    public int CountTokens(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return _encoding.Encode(text).Count;
    }
}
