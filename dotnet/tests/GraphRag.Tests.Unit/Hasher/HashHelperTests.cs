// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Hasher;

namespace GraphRag.Tests.Unit.Hasher;

public class HashHelperTests
{
    [Fact]
    public void Sha256Hash_ProducesDeterministicOutput()
    {
        var hash1 = HashHelper.Sha256Hash("test");
        var hash2 = HashHelper.Sha256Hash("test");
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Sha256Hash_ProducesDifferentOutputForDifferentInput()
    {
        var hash1 = HashHelper.Sha256Hash("test1");
        var hash2 = HashHelper.Sha256Hash("test2");
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Sha256Hash_ReturnsLowercaseHex()
    {
        var hash = HashHelper.Sha256Hash("test");
        Assert.Matches("^[0-9a-f]{64}$", hash);
    }

    [Fact]
    public void HashData_HashesDictionary()
    {
        var data = new Dictionary<string, object?> { ["key"] = "value" };
        var hash = HashHelper.HashData(data);
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void HashData_SameInputProducesSameHash()
    {
        var data1 = new Dictionary<string, object?> { ["strategy"] = "test", ["x"] = "1" };
        var data2 = new Dictionary<string, object?> { ["strategy"] = "test", ["x"] = "1" };
        Assert.Equal(HashHelper.HashData(data1), HashHelper.HashData(data2));
    }

    [Fact]
    public void HashData_DifferentInputProducesDifferentHash()
    {
        var data1 = new Dictionary<string, object?> { ["strategy"] = "a" };
        var data2 = new Dictionary<string, object?> { ["strategy"] = "b" };
        Assert.NotEqual(HashHelper.HashData(data1), HashHelper.HashData(data2));
    }

    [Fact]
    public void HashData_AcceptsCustomHasher()
    {
        var data = new Dictionary<string, object?> { ["key"] = "value" };
        var hash = HashHelper.HashData(data, _ => "custom_hash");
        Assert.Equal("custom_hash", hash);
    }

    [Fact]
    public void MakeYamlSerializable_HandlesNull()
    {
        Assert.Null(HashHelper.MakeYamlSerializable(null));
    }

    [Fact]
    public void MakeYamlSerializable_HandlesString()
    {
        Assert.Equal("hello", HashHelper.MakeYamlSerializable("hello"));
    }

    [Fact]
    public void MakeYamlSerializable_HandlesList()
    {
        var result = HashHelper.MakeYamlSerializable(new List<object> { 1, 2, 3 });
        Assert.IsType<List<object?>>(result);
    }
}
