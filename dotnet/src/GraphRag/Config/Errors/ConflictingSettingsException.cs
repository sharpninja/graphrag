// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Errors;

/// <summary>
/// Exception thrown when conflicting configuration settings are detected.
/// </summary>
public sealed class ConflictingSettingsException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictingSettingsException"/> class.
    /// </summary>
    public ConflictingSettingsException()
        : base("Conflicting configuration settings detected.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictingSettingsException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ConflictingSettingsException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictingSettingsException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ConflictingSettingsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
