// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.SearchApp.Config;

/// <summary>
/// Configuration options for the GraphRAG Search application.
/// </summary>
public class SearchAppConfig
{
    /// <summary>
    /// Gets or sets the root directory for local dataset output.
    /// </summary>
    public string DataRoot { get; set; } = "./output";

    /// <summary>
    /// Gets or sets the Azure Blob Storage account name.
    /// </summary>
    public string BlobAccountName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Azure Blob Storage container name.
    /// </summary>
    public string BlobContainerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the dataset listing JSON file.
    /// </summary>
    public string ListingFile { get; set; } = "listing.json";

    /// <summary>
    /// Gets or sets the default number of suggested questions to generate.
    /// </summary>
    public int DefaultSuggestedQuestions { get; set; } = 5;

    /// <summary>
    /// Gets or sets the cache time-to-live in seconds.
    /// </summary>
    public int CacheTtlSeconds { get; set; } = 604800;
}
