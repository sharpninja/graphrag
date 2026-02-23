// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Config;

/// <summary>
/// Known authentication methods.
/// </summary>
public static class AuthMethod
{
    /// <summary>API key authentication.</summary>
    public const string ApiKey = "api_key";

    /// <summary>Azure managed identity authentication.</summary>
    public const string AzureManagedIdentity = "managed_identity";
}
