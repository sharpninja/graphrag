// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Cache;
using GraphRag.Config.Defaults;
using GraphRag.Config.Enums;
using GraphRag.Input;
using GraphRag.Llm.Config;
using GraphRag.Storage;
using GraphRag.Storage.Tables;
using GraphRag.Vectors;

namespace GraphRag.Config.Models;

/// <summary>
/// The top-level configuration for the GraphRag pipeline.
/// </summary>
public sealed record GraphRagConfig
{
    /// <summary>Gets the completion model configurations keyed by model identifier.</summary>
    public IReadOnlyDictionary<string, ModelConfig> CompletionModels { get; init; } = new Dictionary<string, ModelConfig>();

    /// <summary>Gets the embedding model configurations keyed by model identifier.</summary>
    public IReadOnlyDictionary<string, ModelConfig> EmbeddingModels { get; init; } = new Dictionary<string, ModelConfig>();

    /// <summary>Gets the number of concurrent requests allowed.</summary>
    public int ConcurrentRequests { get; init; } = 25;

    /// <summary>Gets the async execution mode.</summary>
    public string AsyncMode { get; init; } = AsyncType.Threaded;

    /// <summary>Gets the table provider type for data storage.</summary>
    public string? TableProvider { get; init; }

    /// <summary>Gets the list of workflow names to execute.</summary>
    public IReadOnlyList<string>? Workflows { get; init; }

    /// <summary>Gets the reporting configuration.</summary>
    public ReportingConfig Reporting { get; init; } = new();

    /// <summary>Gets the input storage configuration.</summary>
    public StorageConfig InputStorage { get; init; } = new() { BaseDir = DefaultValues.DefaultInputBaseDir };

    /// <summary>Gets the output storage configuration.</summary>
    public StorageConfig OutputStorage { get; init; } = new() { BaseDir = DefaultValues.DefaultOutputBaseDir };

    /// <summary>Gets the update output storage configuration.</summary>
    public StorageConfig UpdateOutputStorage { get; init; } = new() { BaseDir = DefaultValues.DefaultUpdateOutputBaseDir };

    /// <summary>Gets the cache configuration.</summary>
    public CacheConfig Cache { get; init; } = new() { Storage = new StorageConfig { BaseDir = DefaultValues.DefaultCacheBaseDir } };

    /// <summary>Gets the input configuration.</summary>
    public InputConfig Input { get; init; } = new();

    /// <summary>Gets the text embedding configuration.</summary>
    public EmbedTextConfig EmbedText { get; init; } = new();

    /// <summary>Gets the chunking configuration.</summary>
    public GraphRagChunkingConfig Chunking { get; init; } = new();

    /// <summary>Gets the snapshots configuration.</summary>
    public SnapshotsConfig Snapshots { get; init; } = new();

    /// <summary>Gets the graph extraction configuration.</summary>
    public ExtractGraphConfig ExtractGraph { get; init; } = new();

    /// <summary>Gets the NLP-based graph extraction configuration.</summary>
    public ExtractGraphNlpConfig ExtractGraphNlp { get; init; } = new();

    /// <summary>Gets the description summarization configuration.</summary>
    public SummarizeDescriptionsConfig SummarizeDescriptions { get; init; } = new();

    /// <summary>Gets the community reports configuration.</summary>
    public CommunityReportsConfig CommunityReports { get; init; } = new();

    /// <summary>Gets the claim extraction configuration.</summary>
    public ExtractClaimsConfig ExtractClaims { get; init; } = new();

    /// <summary>Gets the graph pruning configuration.</summary>
    public PruneGraphConfig PruneGraph { get; init; } = new();

    /// <summary>Gets the graph clustering configuration.</summary>
    public ClusterGraphConfig ClusterGraph { get; init; } = new();

    /// <summary>Gets the local search configuration.</summary>
    public LocalSearchConfig LocalSearch { get; init; } = new();

    /// <summary>Gets the global search configuration.</summary>
    public GlobalSearchConfig GlobalSearch { get; init; } = new();

    /// <summary>Gets the DRIFT search configuration.</summary>
    public DriftSearchConfig DriftSearch { get; init; } = new();

    /// <summary>Gets the basic search configuration.</summary>
    public BasicSearchConfig BasicSearch { get; init; } = new();

    /// <summary>Gets the vector store configuration.</summary>
    public VectorStoreConfig VectorStore { get; init; } = new() { Type = "lancedb", DbUri = "output/lancedb" };

    /// <summary>
    /// Gets the completion model configuration for the specified model identifier.
    /// </summary>
    /// <param name="modelId">The model identifier, or <c>null</c> to use the default.</param>
    /// <returns>The model configuration.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the model identifier is not found.</exception>
    public ModelConfig GetCompletionModelConfig(string? modelId = null)
    {
        var key = modelId ?? DefaultValues.DefaultCompletionModelId;
        if (CompletionModels.TryGetValue(key, out var config))
        {
            return config;
        }

        throw new KeyNotFoundException($"Completion model '{key}' not found in configuration.");
    }

    /// <summary>
    /// Gets the embedding model configuration for the specified model identifier.
    /// </summary>
    /// <param name="modelId">The model identifier, or <c>null</c> to use the default.</param>
    /// <returns>The model configuration.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the model identifier is not found.</exception>
    public ModelConfig GetEmbeddingModelConfig(string? modelId = null)
    {
        var key = modelId ?? DefaultValues.DefaultEmbeddingModelId;
        if (EmbeddingModels.TryGetValue(key, out var config))
        {
            return config;
        }

        throw new KeyNotFoundException($"Embedding model '{key}' not found in configuration.");
    }
}
