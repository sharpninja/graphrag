// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Config;

/// <summary>
/// Known retry strategies.
/// </summary>
public static class RetryType
{
    /// <summary>Exponential backoff retry strategy.</summary>
    public const string ExponentialBackoff = "exponential_backoff";

    /// <summary>Immediate retry strategy.</summary>
    public const string Immediate = "immediate";
}
