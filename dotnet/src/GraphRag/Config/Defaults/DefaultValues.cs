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

    /// <summary>Gets the default completion model authentication type.</summary>
    public const string DefaultCompletionModelAuthType = "api_key";

    /// <summary>Gets the default embedding model identifier.</summary>
    public const string DefaultEmbeddingModelId = "default_embedding_model";

    /// <summary>Gets the default embedding model name.</summary>
    public const string DefaultEmbeddingModel = "text-embedding-3-large";

    /// <summary>Gets the default embedding model authentication type.</summary>
    public const string DefaultEmbeddingModelAuthType = "api_key";

    /// <summary>Gets the default model provider.</summary>
    public const string DefaultModelProvider = "openai";

    /// <summary>Gets the default encoding model for tokenization.</summary>
    public const string EncodingModel = "o200k_base";

    /// <summary>Gets the Azure Cognitive Services audience URL for managed identity auth.</summary>
    public const string CognitiveServicesAudience = "https://cognitiveservices.azure.com/.default";

    /// <summary>Gets the default entity types for extraction.</summary>
    public static readonly IReadOnlyList<string> DefaultEntityTypes = ["organization", "person", "geo", "event"];
}

/// <summary>Default values for basic search configuration.</summary>
public static class BasicSearchDefaults
{
    /// <summary>Gets the default number of results to return.</summary>
    public const int K = 20;

    /// <summary>Gets the default maximum context tokens.</summary>
    public const int MaxContextTokens = 8000;
}

/// <summary>Default values for chunking configuration.</summary>
public static class ChunkingDefaults
{
    /// <summary>Gets the default chunk size in tokens.</summary>
    public const int Size = 1200;

    /// <summary>Gets the default chunk overlap in tokens.</summary>
    public const int Overlap = 100;

    /// <summary>Gets the default encoding model for chunking.</summary>
    public const string EncodingModel = DefaultValues.EncodingModel;

    /// <summary>Gets the default group-by columns.</summary>
    public static readonly IReadOnlyList<string> GroupByColumns = ["id"];
}

/// <summary>Default values for cluster graph configuration.</summary>
public static class ClusterGraphDefaults
{
    /// <summary>Gets the default maximum cluster size.</summary>
    public const int MaxClusterSize = 10;

    /// <summary>Gets a value indicating whether to use the largest connected component by default.</summary>
    public const bool UseLcc = true;

    /// <summary>Gets the default seed for clustering.</summary>
    public const uint Seed = 0xDEAD;
}

/// <summary>Default values for community report configuration.</summary>
public static class CommunityReportDefaults
{
    /// <summary>Gets the default maximum output length in tokens.</summary>
    public const int MaxLength = 2000;

    /// <summary>Gets the default maximum input length in tokens.</summary>
    public const int MaxInputLength = 8000;
}

/// <summary>Default values for drift search configuration.</summary>
public static class DriftSearchDefaults
{
    /// <summary>Gets the default maximum data tokens.</summary>
    public const int DataMaxTokens = 12000;

    /// <summary>Gets the default maximum reduce tokens.</summary>
    public const int ReduceMaxTokens = 2000;

    /// <summary>Gets the default reduce temperature.</summary>
    public const double ReduceTemperature = 0.0;

    /// <summary>Gets the default reduce max completion tokens.</summary>
    public const int ReduceMaxCompletionTokens = 1024;

    /// <summary>Gets the default concurrency.</summary>
    public const int Concurrency = 4;

    /// <summary>Gets the default number of drift follow-up queries.</summary>
    public const int DriftKFollowups = 3;

    /// <summary>Gets the default number of primer folds.</summary>
    public const int PrimerFolds = 1;

    /// <summary>Gets the default primer LLM max tokens.</summary>
    public const int PrimerLlmMaxTokens = 4000;

    /// <summary>Gets the default search depth.</summary>
    public const int NDepth = 3;

    /// <summary>Gets the default local search text unit proportion.</summary>
    public const double LocalSearchTextUnitProp = 0.5;

    /// <summary>Gets the default local search community proportion.</summary>
    public const double LocalSearchCommunityProp = 0.1;

    /// <summary>Gets the default top-k mapped entities for local search.</summary>
    public const int LocalSearchTopKMappedEntities = 10;

    /// <summary>Gets the default top-k relationships for local search.</summary>
    public const int LocalSearchTopKRelationships = 10;

    /// <summary>Gets the default local search max data tokens.</summary>
    public const int LocalSearchMaxDataTokens = 12000;

    /// <summary>Gets the default local search temperature.</summary>
    public const double LocalSearchTemperature = 0.0;

    /// <summary>Gets the default local search top-p.</summary>
    public const double LocalSearchTopP = 1.0;

    /// <summary>Gets the default local search N.</summary>
    public const int LocalSearchN = 10;

    /// <summary>Gets the default local search LLM max generation tokens.</summary>
    public const int LocalSearchLlmMaxGenTokens = 1024;

    /// <summary>Gets the default local search LLM max completion tokens.</summary>
    public const int LocalSearchLlmMaxGenCompletionTokens = 512;
}

/// <summary>Default values for text embedding configuration.</summary>
public static class EmbedTextDefaults
{
    /// <summary>Gets the default batch size.</summary>
    public const int BatchSize = 16;

    /// <summary>Gets the default maximum tokens per batch.</summary>
    public const int BatchMaxTokens = 8191;
}

/// <summary>Default values for claim extraction configuration.</summary>
public static class ExtractClaimsDefaults
{
    /// <summary>Gets the default description for claims.</summary>
    public const string Description = "Any claims or facts that could be relevant to information discovery.";

    /// <summary>Gets the default maximum number of gleanings.</summary>
    public const int MaxGleanings = 1;
}

/// <summary>Default values for graph extraction configuration.</summary>
public static class ExtractGraphDefaults
{
    /// <summary>Gets the default maximum number of gleanings.</summary>
    public const int MaxGleanings = 1;
}

/// <summary>Default values for text analyzer configuration.</summary>
public static class TextAnalyzerDefaults
{
    /// <summary>Gets the default maximum word length.</summary>
    public const int MaxWordLength = 15;

    /// <summary>Gets the default word delimiter.</summary>
    public const string WordDelimiter = " ";

    /// <summary>Gets a value indicating whether to include named entities by default.</summary>
    public const bool IncludeNamedEntities = true;
}

/// <summary>Default values for NLP graph extraction configuration.</summary>
public static class ExtractGraphNlpDefaults
{
    /// <summary>Gets a value indicating whether to normalize edge weights by default.</summary>
    public const bool NormalizeEdgeWeights = true;
}

/// <summary>Default values for global search configuration.</summary>
public static class GlobalSearchDefaults
{
    /// <summary>Gets the default maximum context tokens.</summary>
    public const int MaxContextTokens = 8000;

    /// <summary>Gets the default maximum data tokens.</summary>
    public const int DataMaxTokens = 12000;

    /// <summary>Gets the default map max length.</summary>
    public const int MapMaxLength = 1000;

    /// <summary>Gets the default reduce max length.</summary>
    public const int ReduceMaxLength = 2000;

    /// <summary>Gets the default dynamic search threshold.</summary>
    public const int DynamicSearchThreshold = 0;

    /// <summary>Gets a value indicating whether to keep parent in dynamic search by default.</summary>
    public const bool DynamicSearchKeepParent = false;

    /// <summary>Gets the default number of dynamic search repeats.</summary>
    public const int DynamicSearchNumRepeats = 1;

    /// <summary>Gets a value indicating whether to use summary in dynamic search by default.</summary>
    public const bool DynamicSearchUseSummary = false;

    /// <summary>Gets the default maximum level for dynamic search.</summary>
    public const int DynamicSearchMaxLevel = -1;
}

/// <summary>Default values for storage configuration.</summary>
public static class StorageDefaults
{
    /// <summary>Gets the default storage type.</summary>
    public const string Type = "file";

    /// <summary>Gets the default encoding.</summary>
    public const string Encoding = "utf-8";
}

/// <summary>Default values for input configuration.</summary>
public static class InputDefaults
{
    /// <summary>Gets the default input type.</summary>
    public const string Type = "file";

    /// <summary>Gets the default file type.</summary>
    public const string FileType = "text";

    /// <summary>Gets the default file pattern.</summary>
    public const string FilePattern = ".*\\.txt$";

    /// <summary>Gets the default encoding.</summary>
    public const string Encoding = "utf-8";
}

/// <summary>Default values for cache configuration.</summary>
public static class CacheDefaults
{
    /// <summary>Gets the default cache type.</summary>
    public const string Type = "file";
}

/// <summary>Default values for local search configuration.</summary>
public static class LocalSearchDefaults
{
    /// <summary>Gets the default text unit proportion.</summary>
    public const double TextUnitProp = 0.5;

    /// <summary>Gets the default community proportion.</summary>
    public const double CommunityProp = 0.1;

    /// <summary>Gets the default conversation history max turns.</summary>
    public const int ConversationHistoryMaxTurns = 5;

    /// <summary>Gets the default top-k entities.</summary>
    public const int TopKEntities = 10;

    /// <summary>Gets the default top-k relationships.</summary>
    public const int TopKRelationships = 10;

    /// <summary>Gets the default maximum context tokens.</summary>
    public const int MaxContextTokens = 12000;
}

/// <summary>Default values for graph pruning configuration.</summary>
public static class PruneGraphDefaults
{
    /// <summary>Gets the default minimum node frequency.</summary>
    public const int MinNodeFreq = 0;

    /// <summary>Gets the default maximum node frequency standard deviation.</summary>
    public const double MaxNodeFreqStd = -1;

    /// <summary>Gets the default minimum node degree.</summary>
    public const int MinNodeDegree = 0;

    /// <summary>Gets the default maximum node degree standard deviation.</summary>
    public const double MaxNodeDegreeStd = -1;

    /// <summary>Gets the default minimum edge weight percentile.</summary>
    public const double MinEdgeWeightPct = 0;

    /// <summary>Gets a value indicating whether to remove ego nodes by default.</summary>
    public const bool RemoveEgoNodes = false;

    /// <summary>Gets a value indicating whether to use largest connected component only by default.</summary>
    public const bool LccOnly = false;
}

/// <summary>Default values for reporting configuration.</summary>
public static class ReportingDefaults
{
    /// <summary>Gets the default reporting type.</summary>
    public const string Type = "file";
}

/// <summary>Default values for snapshots configuration.</summary>
public static class SnapshotsDefaults
{
    /// <summary>Gets a value indicating whether embedding snapshots are enabled by default.</summary>
    public const bool Embeddings = false;

    /// <summary>Gets a value indicating whether GraphML snapshots are enabled by default.</summary>
    public const bool GraphMl = false;

    /// <summary>Gets a value indicating whether raw graph snapshots are enabled by default.</summary>
    public const bool RawGraph = false;
}

/// <summary>Default values for description summarization configuration.</summary>
public static class SummarizeDescriptionsDefaults
{
    /// <summary>Gets the default maximum output length in tokens.</summary>
    public const int MaxLength = 500;

    /// <summary>Gets the default maximum input tokens.</summary>
    public const int MaxInputTokens = 4000;
}

/// <summary>Default values for vector store configuration.</summary>
public static class VectorStoreDefaults
{
    /// <summary>Gets the default vector store type.</summary>
    public const string Type = "lancedb";

    /// <summary>Gets the default database URI.</summary>
    public const string DbUri = "output/lancedb";
}

/// <summary>Top-level default values for GraphRag configuration.</summary>
public static class GraphRagConfigDefaults
{
    /// <summary>Gets the default number of concurrent requests.</summary>
    public const int ConcurrentRequests = 25;

    /// <summary>Gets the default async mode.</summary>
    public const string AsyncMode = "threaded";
}
