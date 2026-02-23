// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Storage;

namespace GraphRag.Tests.Unit.Storage;

/// <summary>
/// Unit tests for <see cref="StorageConfig"/>.
/// </summary>
public class StorageConfigTests
{
    [Fact]
    public void DefaultType_IsFile()
    {
        var config = new StorageConfig();
        config.Type.Should().Be("file");
    }

    [Fact]
    public void Properties_SetViaInit_Work()
    {
        var config = new StorageConfig
        {
            Type = StorageType.Memory,
            Encoding = "utf-16",
            BaseDir = "/tmp",
            ConnectionString = "conn",
            ContainerName = "container",
            AccountUrl = "https://account.blob.core.windows.net",
            DatabaseName = "mydb",
        };

        config.Type.Should().Be(StorageType.Memory);
        config.Encoding.Should().Be("utf-16");
        config.BaseDir.Should().Be("/tmp");
        config.ConnectionString.Should().Be("conn");
        config.ContainerName.Should().Be("container");
        config.AccountUrl.Should().Be("https://account.blob.core.windows.net");
        config.DatabaseName.Should().Be("mydb");
    }
}
