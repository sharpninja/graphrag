// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Errors;

/// <summary>
/// Exception thrown when the Azure API base URL is missing from the configuration.
/// </summary>
public sealed class AzureApiBaseMissingException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureApiBaseMissingException"/> class.
    /// </summary>
    public AzureApiBaseMissingException()
        : base("Azure API base URL is required but not configured. Set the api_base field or the GRAPHRAG_API_BASE environment variable.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureApiBaseMissingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public AzureApiBaseMissingException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureApiBaseMissingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AzureApiBaseMissingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
