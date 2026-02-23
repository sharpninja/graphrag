// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Index.Workflows;

/// <summary>
/// Contains constant names for all known indexing workflows.
/// </summary>
public static class WorkflowNames
{
    /// <summary>Load raw input documents from the configured source.</summary>
    public const string LoadInputDocuments = "load_input_documents";

    /// <summary>Create base text units from input documents.</summary>
    public const string CreateBaseTextUnits = "create_base_text_units";

    /// <summary>Create finalized document records.</summary>
    public const string CreateFinalDocuments = "create_final_documents";

    /// <summary>Extract the knowledge graph from text units.</summary>
    public const string ExtractGraph = "extract_graph";

    /// <summary>Extract the knowledge graph using NLP techniques.</summary>
    public const string ExtractGraphNlp = "extract_graph_nlp";

    /// <summary>Finalize the knowledge graph after extraction.</summary>
    public const string FinalizeGraph = "finalize_graph";

    /// <summary>Prune low-value nodes and edges from the graph.</summary>
    public const string PruneGraph = "prune_graph";

    /// <summary>Extract covariates (claims) from the text.</summary>
    public const string ExtractCovariates = "extract_covariates";

    /// <summary>Detect and create community structures in the graph.</summary>
    public const string CreateCommunities = "create_communities";

    /// <summary>Create finalized text unit records.</summary>
    public const string CreateFinalTextUnits = "create_final_text_units";

    /// <summary>Generate community reports using LLM summarization.</summary>
    public const string CreateCommunityReports = "create_community_reports";

    /// <summary>Generate community reports using text-based summarization.</summary>
    public const string CreateCommunityReportsText = "create_community_reports_text";

    /// <summary>Generate text embeddings for indexed content.</summary>
    public const string GenerateTextEmbeddings = "generate_text_embeddings";

    /// <summary>Load documents for an incremental update run.</summary>
    public const string LoadUpdateDocuments = "load_update_documents";

    /// <summary>Update entities and relationships during an incremental run.</summary>
    public const string UpdateEntitiesRelationships = "update_entities_relationships";

    /// <summary>Update community structures during an incremental run.</summary>
    public const string UpdateCommunities = "update_communities";

    /// <summary>Update community reports during an incremental run.</summary>
    public const string UpdateCommunityReports = "update_community_reports";

    /// <summary>Update covariates during an incremental run.</summary>
    public const string UpdateCovariates = "update_covariates";

    /// <summary>Update finalized documents during an incremental run.</summary>
    public const string UpdateFinalDocuments = "update_final_documents";

    /// <summary>Update text embeddings during an incremental run.</summary>
    public const string UpdateTextEmbeddings = "update_text_embeddings";

    /// <summary>Update text units during an incremental run.</summary>
    public const string UpdateTextUnits = "update_text_units";

    /// <summary>Clean up state at the end of an update run.</summary>
    public const string UpdateCleanState = "update_clean_state";
}
