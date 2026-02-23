// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Config;

/// <summary>
/// Known tokenizer types.
/// </summary>
public static class TokenizerType
{
    /// <summary>LiteLLM-based tokenizer.</summary>
    public const string LiteLlm = "litellm";

    /// <summary>Tiktoken-based tokenizer.</summary>
    public const string Tiktoken = "tiktoken";
}
