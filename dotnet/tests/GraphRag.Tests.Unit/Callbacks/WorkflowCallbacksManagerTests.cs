// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Callbacks;
using GraphRag.Logger;
using Moq;

namespace GraphRag.Tests.Unit.Callbacks;

/// <summary>
/// Unit tests for <see cref="WorkflowCallbacksManager"/>.
/// </summary>
public class WorkflowCallbacksManagerTests
{
    [Fact]
    public void Register_AddsCallback()
    {
        var manager = new WorkflowCallbacksManager();
        var mock = new Mock<IWorkflowCallbacks>();

        manager.Register(mock.Object);

        var names = new List<string> { "workflow1" };
        manager.OnPipelineStart(names);

        mock.Verify(cb => cb.OnPipelineStart(names), Times.Once);
    }

    [Fact]
    public void OnPipelineStart_DelegatesToAll()
    {
        var manager = new WorkflowCallbacksManager();
        var mock1 = new Mock<IWorkflowCallbacks>();
        var mock2 = new Mock<IWorkflowCallbacks>();
        manager.Register(mock1.Object);
        manager.Register(mock2.Object);

        var names = new List<string> { "a", "b" };
        manager.OnPipelineStart(names);

        mock1.Verify(cb => cb.OnPipelineStart(names), Times.Once);
        mock2.Verify(cb => cb.OnPipelineStart(names), Times.Once);
    }

    [Fact]
    public void OnProgress_DelegatesToAll()
    {
        var manager = new WorkflowCallbacksManager();
        var mock1 = new Mock<IWorkflowCallbacks>();
        var mock2 = new Mock<IWorkflowCallbacks>();
        manager.Register(mock1.Object);
        manager.Register(mock2.Object);

        var progress = new ProgressInfo { CompletedItems = 5, TotalItems = 10 };
        manager.OnProgress(progress);

        mock1.Verify(cb => cb.OnProgress(progress), Times.Once);
        mock2.Verify(cb => cb.OnProgress(progress), Times.Once);
    }
}
