// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Storage.Tables;

/// <summary>
/// The default configuration section for table providers.
/// </summary>
public sealed record TableProviderConfig
{
    /// <summary>Gets the table type to use.</summary>
    public string Type { get; init; } = TableType.Parquet;
}
