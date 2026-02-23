// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Enums;

/// <summary>
/// Async execution mode types.
/// </summary>
public static class AsyncType
{
    /// <summary>Gets the identifier for asyncio-based execution.</summary>
    public const string AsyncIO = "asyncio";

    /// <summary>Gets the identifier for threaded execution.</summary>
    public const string Threaded = "threaded";
}
