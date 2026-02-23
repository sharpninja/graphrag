// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Index;
using GraphRag.Index.Workflows;

namespace GraphRag.Tests.Unit.Index;

public class PipelineFactoryTests
{
    [Fact]
    public void CreatePipeline_Standard_ReturnsWorkflows()
    {
        var pipeline = PipelineFactory.CreateStandard();

        pipeline.Names.Should().NotBeEmpty();
        pipeline.Names.Should().Contain(WorkflowNames.ExtractGraph);
        pipeline.Names.Should().Contain(WorkflowNames.CreateCommunityReports);
    }

    [Fact]
    public void CreatePipeline_Fast_ReturnsWorkflows()
    {
        var pipeline = PipelineFactory.CreateFast();

        pipeline.Names.Should().NotBeEmpty();
        pipeline.Names.Should().Contain(WorkflowNames.ExtractGraphNlp);
        pipeline.Names.Should().NotContain(WorkflowNames.ExtractGraph);
    }
}
