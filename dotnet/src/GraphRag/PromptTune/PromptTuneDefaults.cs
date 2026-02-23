// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.PromptTune;

/// <summary>
/// Default values used during prompt tuning.
/// </summary>
public static class PromptTuneDefaults
{
    /// <summary>Default number of top results to consider.</summary>
    public const int K = 15;

    /// <summary>Default document limit for selection.</summary>
    public const int Limit = 15;

    /// <summary>Default maximum token count per chunk.</summary>
    public const int MaxTokenCount = 2000;

    /// <summary>Default maximum subset size for sampling.</summary>
    public const int NSubsetMax = 300;
}
