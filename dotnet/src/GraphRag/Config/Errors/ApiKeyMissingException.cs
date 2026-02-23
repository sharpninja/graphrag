// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Errors;

/// <summary>
/// Exception thrown when a required API key is missing from the configuration.
/// </summary>
public sealed class ApiKeyMissingException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyMissingException"/> class.
    /// </summary>
    public ApiKeyMissingException()
        : base("API key is required but not configured. Set the api_key field or the GRAPHRAG_API_KEY environment variable.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyMissingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ApiKeyMissingException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyMissingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ApiKeyMissingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
