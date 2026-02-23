// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Config;

/// <summary>
/// Known LLM provider types.
/// </summary>
public static class LlmProviderType
{
    /// <summary>LiteLLM provider.</summary>
    public const string LiteLlm = "litellm";

    /// <summary>Mock provider for testing.</summary>
    public const string MockLlm = "mock";
}
