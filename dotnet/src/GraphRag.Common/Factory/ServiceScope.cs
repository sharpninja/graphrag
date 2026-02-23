// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Common.Factories;

/// <summary>
/// The scope of a service registration.
/// </summary>
public enum ServiceScope
{
    /// <summary>
    /// A new instance is created for each request.
    /// </summary>
    Transient,

    /// <summary>
    /// A single instance is cached and reused based on init args hash.
    /// </summary>
    Singleton,
}
