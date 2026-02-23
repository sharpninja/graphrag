// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Input;
using GraphRag.Storage;

namespace GraphRag.Tests.Unit.Input;

/// <summary>
/// Unit tests for <see cref="InputReaderFactory"/>.
/// </summary>
public class InputReaderFactoryTests
{
    [Fact]
    public void CreateInputReader_TextType_CreatesTextFileReader()
    {
        var factory = new InputReaderFactory();
        var config = new InputConfig { Type = InputType.Text };
        var storage = new MemoryStorage();

        var reader = factory.CreateInputReader(config, storage);

        reader.Should().BeOfType<TextFileReader>();
    }

    [Fact]
    public void CreateInputReader_UnknownType_Throws()
    {
        var factory = new InputReaderFactory();
        var config = new InputConfig { Type = "unknown" };
        var storage = new MemoryStorage();

        var act = () => factory.CreateInputReader(config, storage);

        act.Should().Throw<InvalidOperationException>();
    }
}
