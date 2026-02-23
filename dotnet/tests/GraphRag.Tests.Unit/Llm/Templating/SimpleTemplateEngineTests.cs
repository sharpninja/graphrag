// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Llm.Templating;

namespace GraphRag.Tests.Unit.Llm.Templating;

/// <summary>
/// Unit tests for <see cref="SimpleTemplateEngine"/>.
/// </summary>
public class SimpleTemplateEngineTests
{
    private readonly SimpleTemplateEngine _sut = new();

    [Fact]
    public void Render_ReplacesVariables()
    {
        var result = _sut.Render("Hello {{name}}!", new Dictionary<string, object?> { ["name"] = "World" });

        result.Should().Be("Hello World!");
    }

    [Fact]
    public void Render_MissingVariable_LeavesPlaceholder()
    {
        var result = _sut.Render("Hello {{name}}!", new Dictionary<string, object?>());

        result.Should().Be("Hello {{name}}!");
    }

    [Fact]
    public void Render_MultipleVariables_ReplacesAll()
    {
        var result = _sut.Render(
            "{{greeting}} {{name}}, welcome to {{place}}!",
            new Dictionary<string, object?>
            {
                ["greeting"] = "Hi",
                ["name"] = "Alice",
                ["place"] = "Wonderland",
            });

        result.Should().Be("Hi Alice, welcome to Wonderland!");
    }
}
