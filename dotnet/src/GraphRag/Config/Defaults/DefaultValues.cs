// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Defaults;

/// <summary>
/// Default constant values used across GraphRag configuration.
/// </summary>
public static class DefaultValues
{
    /// <summary>Gets the default base directory for input files.</summary>
    public const string DefaultInputBaseDir = "input";

    /// <summary>Gets the default base directory for output files.</summary>
    public const string DefaultOutputBaseDir = "output";

    /// <summary>Gets the default base directory for cache files.</summary>
    public const string DefaultCacheBaseDir = "cache";

    /// <summary>Gets the default base directory for update output files.</summary>
    public const string DefaultUpdateOutputBaseDir = "update_output";

    /// <summary>Gets the default completion model identifier.</summary>
    public const string DefaultCompletionModelId = "default_completion_model";

    /// <summary>Gets the default completion model name.</summary>
    public const string DefaultCompletionModel = "gpt-4.1";

    /// <summary>Gets the default embedding model identifier.</summary>
    public const string DefaultEmbeddingModelId = "default_embedding_model";

    /// <summary>Gets the default embedding model name.</summary>
    public const string DefaultEmbeddingModel = "text-embedding-3-large";

    /// <summary>Gets the default model provider.</summary>
    public const string DefaultModelProvider = "openai";

    /// <summary>Gets the default encoding model for tokenization.</summary>
    public const string EncodingModel = "o200k_base";

    /// <summary>Gets the default entity types for extraction.</summary>
    public static readonly IReadOnlyList<string> DefaultEntityTypes = ["organization", "person", "geo", "event"];
}
