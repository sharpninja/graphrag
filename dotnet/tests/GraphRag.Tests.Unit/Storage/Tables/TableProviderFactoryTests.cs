// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Storage.Tables;

namespace GraphRag.Tests.Unit.Storage.Tables;

/// <summary>
/// Unit tests for <see cref="TableProviderFactory"/>.
/// </summary>
public class TableProviderFactoryTests
{
    private readonly TableProviderFactory _factory = new();

    [Fact]
    public void CreateTableProvider_UnknownType_ThrowsInvalidOperationException()
    {
        var config = new TableProviderConfig { Type = "unknown" };
        var act = () => _factory.CreateTableProvider(config);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateTableProvider_Parquet_ThrowsNotImplementedException()
    {
        var config = new TableProviderConfig { Type = TableType.Parquet };
        var act = () => _factory.CreateTableProvider(config);
        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void CreateTableProvider_Csv_ThrowsNotImplementedException()
    {
        var config = new TableProviderConfig { Type = TableType.Csv };
        var act = () => _factory.CreateTableProvider(config);
        act.Should().Throw<NotImplementedException>();
    }
}
