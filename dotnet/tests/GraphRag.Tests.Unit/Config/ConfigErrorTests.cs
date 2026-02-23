// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Config.Errors;

namespace GraphRag.Tests.Unit.Config;

public class ConfigErrorTests
{
    [Fact]
    public void ApiKeyMissingException_HasDefaultMessage()
    {
        var ex = new ApiKeyMissingException();

        ex.Message.Should().Contain("API key");
        ex.Should().BeAssignableTo<InvalidOperationException>();
    }

    [Fact]
    public void ApiKeyMissingException_AcceptsCustomMessage()
    {
        var ex = new ApiKeyMissingException("Custom message");

        ex.Message.Should().Be("Custom message");
    }

    [Fact]
    public void AzureApiBaseMissingException_HasDefaultMessage()
    {
        var ex = new AzureApiBaseMissingException();

        ex.Message.Should().Contain("Azure API base");
        ex.Should().BeAssignableTo<InvalidOperationException>();
    }

    [Fact]
    public void AzureApiVersionMissingException_HasDefaultMessage()
    {
        var ex = new AzureApiVersionMissingException();

        ex.Message.Should().Contain("Azure API version");
        ex.Should().BeAssignableTo<InvalidOperationException>();
    }

    [Fact]
    public void ConflictingSettingsException_HasDefaultMessage()
    {
        var ex = new ConflictingSettingsException();

        ex.Message.Should().Contain("Conflicting");
        ex.Should().BeAssignableTo<InvalidOperationException>();
    }

    [Fact]
    public void ConflictingSettingsException_AcceptsCustomMessage()
    {
        var ex = new ConflictingSettingsException("Two settings conflict");

        ex.Message.Should().Be("Two settings conflict");
    }

    [Fact]
    public void ConfigExceptions_SupportInnerException()
    {
        var inner = new InvalidOperationException("inner");

        var ex1 = new ApiKeyMissingException("msg", inner);
        var ex2 = new AzureApiBaseMissingException("msg", inner);
        var ex3 = new AzureApiVersionMissingException("msg", inner);
        var ex4 = new ConflictingSettingsException("msg", inner);

        ex1.InnerException.Should().BeSameAs(inner);
        ex2.InnerException.Should().BeSameAs(inner);
        ex3.InnerException.Should().BeSameAs(inner);
        ex4.InnerException.Should().BeSameAs(inner);
    }
}
