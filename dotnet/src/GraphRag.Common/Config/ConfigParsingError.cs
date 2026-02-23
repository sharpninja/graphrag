// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Common.Config;

/// <summary>
/// Exception thrown when configuration parsing fails.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "Preserving Python API naming")]
public sealed class ConfigParsingError : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigParsingError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ConfigParsingError(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigParsingError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ConfigParsingError(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
