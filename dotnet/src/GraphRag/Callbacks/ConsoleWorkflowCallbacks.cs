// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Logger;

namespace GraphRag.Callbacks;

/// <summary>
/// An <see cref="IWorkflowCallbacks"/> implementation that logs events to the console.
/// </summary>
public sealed class ConsoleWorkflowCallbacks : IWorkflowCallbacks
{
    private readonly bool _verbose;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleWorkflowCallbacks"/> class.
    /// </summary>
    /// <param name="verbose">Whether to enable verbose logging.</param>
    public ConsoleWorkflowCallbacks(bool verbose = false)
    {
        _verbose = verbose;
    }

    /// <inheritdoc />
    public void OnPipelineStart(IReadOnlyList<string> names)
    {
        Console.WriteLine($"Pipeline started with workflows: {string.Join(", ", names)}");
    }

    /// <inheritdoc />
    public void OnPipelineEnd(IReadOnlyList<object> results)
    {
        Console.WriteLine($"Pipeline completed with {results.Count} result(s).");
    }

    /// <inheritdoc />
    public void OnWorkflowStart(string name, object instance)
    {
        Console.WriteLine($"Workflow '{name}' started.");
        if (_verbose)
        {
            Console.WriteLine($"  Instance: {instance}");
        }
    }

    /// <inheritdoc />
    public void OnWorkflowEnd(string name, object instance)
    {
        Console.WriteLine($"Workflow '{name}' completed.");
        if (_verbose)
        {
            Console.WriteLine($"  Instance: {instance}");
        }
    }

    /// <inheritdoc />
    public void OnProgress(ProgressInfo progress)
    {
        if (progress.TotalItems.HasValue)
        {
            Console.WriteLine($"Progress: {progress.CompletedItems}/{progress.TotalItems} - {progress.Description}");
        }
        else
        {
            Console.WriteLine($"Progress: {progress.CompletedItems} completed - {progress.Description}");
        }
    }

    /// <inheritdoc />
    public void OnPipelineError(Exception exception)
    {
        Console.WriteLine($"Pipeline error: {exception.Message}");
        if (_verbose)
        {
            Console.WriteLine(exception.ToString());
        }
    }
}
