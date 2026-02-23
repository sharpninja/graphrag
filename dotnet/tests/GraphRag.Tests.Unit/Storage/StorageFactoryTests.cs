// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Storage;

namespace GraphRag.Tests.Unit.Storage;

/// <summary>
/// Unit tests for <see cref="StorageFactory"/>.
/// </summary>
public class StorageFactoryTests
{
    private readonly StorageFactory _factory = new();

    [Fact]
    public void CreateStorage_FileType_CreatesFileStorage()
    {
        var config = new StorageConfig { Type = StorageType.File, BaseDir = Path.GetTempPath() };
        var storage = _factory.CreateStorage(config);
        storage.Should().BeOfType<FileStorage>();
    }

    [Fact]
    public void CreateStorage_MemoryType_CreatesMemoryStorage()
    {
        var config = new StorageConfig { Type = StorageType.Memory };
        var storage = _factory.CreateStorage(config);
        storage.Should().BeOfType<MemoryStorage>();
    }

    [Fact]
    public void CreateStorage_UnknownType_ThrowsInvalidOperationException()
    {
        var config = new StorageConfig { Type = "unknown" };
        var act = () => _factory.CreateStorage(config);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateStorage_CustomRegistration_OverridesBuiltin()
    {
        var custom = new MemoryStorage();
        _factory.Register(StorageType.Memory, _ => custom);

        var config = new StorageConfig { Type = StorageType.Memory };
        var result = _factory.CreateStorage(config);
        result.Should().BeSameAs(custom);
    }
}
