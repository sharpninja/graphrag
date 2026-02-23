// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Config;

/// <summary>
/// Configuration for the tokenizer.
/// </summary>
public sealed record TokenizerConfig
{
    /// <summary>Gets the tokenizer type.</summary>
    public string Type { get; init; } = TokenizerType.Tiktoken;

    /// <summary>Gets the model name used to select the tokenizer encoding.</summary>
    public string? Model { get; init; }
}
