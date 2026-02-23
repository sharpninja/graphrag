// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Errors;

/// <summary>
/// Exception thrown when the Azure API version is missing from the configuration.
/// </summary>
public sealed class AzureApiVersionMissingException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureApiVersionMissingException"/> class.
    /// </summary>
    public AzureApiVersionMissingException()
        : base("Azure API version is required but not configured. Set the api_version field or the GRAPHRAG_API_VERSION environment variable.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureApiVersionMissingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public AzureApiVersionMissingException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureApiVersionMissingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AzureApiVersionMissingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
