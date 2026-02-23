// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Storage.Tables;

/// <summary>
/// Builtin table storage implementation types.
/// </summary>
public static class TableType
{
    /// <summary>Gets the identifier for Parquet table format.</summary>
    public const string Parquet = "parquet";

    /// <summary>Gets the identifier for CSV table format.</summary>
    public const string Csv = "csv";
}
