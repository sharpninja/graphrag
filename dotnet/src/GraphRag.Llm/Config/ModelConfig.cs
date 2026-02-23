// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Config;

/// <summary>
/// Configuration for an LLM model.
/// </summary>
public sealed record ModelConfig
{
    /// <summary>Gets the LLM provider type.</summary>
    public string Type { get; init; } = LlmProviderType.LiteLlm;

    /// <summary>Gets the model name or identifier.</summary>
    public string? Model { get; init; }

    /// <summary>Gets the API base URL.</summary>
    public string? ApiBase { get; init; }

    /// <summary>Gets the API key.</summary>
    public string? ApiKey { get; init; }

    /// <summary>Gets the authentication method.</summary>
    public string AuthMethod { get; init; } = Config.AuthMethod.ApiKey;

    /// <summary>Gets the Azure API version.</summary>
    public string? AzureApiVersion { get; init; }

    /// <summary>Gets the organization identifier.</summary>
    public string? Organization { get; init; }

    /// <summary>Gets the proxy URL.</summary>
    public string? Proxy { get; init; }

    /// <summary>Gets the audience for managed identity authentication.</summary>
    public string? Audience { get; init; }

    /// <summary>Gets the sampling temperature.</summary>
    public double? Temperature { get; init; }

    /// <summary>Gets the top-p value for nucleus sampling.</summary>
    public double? TopP { get; init; }

    /// <summary>Gets the maximum number of tokens to generate.</summary>
    public int? MaxTokens { get; init; }

    /// <summary>Gets the seed for deterministic generation.</summary>
    public int? Seed { get; init; }

    /// <summary>Gets the maximum number of retries.</summary>
    public int MaxRetries { get; init; } = 3;

    /// <summary>Gets the request timeout in seconds.</summary>
    public int? RequestTimeout { get; init; }

    /// <summary>Gets the rate limiting configuration.</summary>
    public RateLimitConfig? RateLimit { get; init; }

    /// <summary>Gets the retry configuration.</summary>
    public RetryConfig? Retry { get; init; }

    /// <summary>Gets the metrics configuration.</summary>
    public MetricsConfig? Metrics { get; init; }

    /// <summary>Gets the tokenizer configuration.</summary>
    public TokenizerConfig? Tokenizer { get; init; }

    /// <summary>Gets the template engine configuration.</summary>
    public TemplateEngineConfig? TemplateEngine { get; init; }
}
