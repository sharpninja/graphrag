// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Runtime.CompilerServices;

using GraphRag.Callbacks;
using GraphRag.Config.Models;
using GraphRag.Index.Typing;

namespace GraphRag.Index.Run;

/// <summary>
/// Executes an indexing pipeline and yields results for each workflow.
/// </summary>
public static class PipelineRunner
{
    /// <summary>
    /// Runs all workflows in the given pipeline, yielding a result for each one.
    /// </summary>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="config">The GraphRAG configuration.</param>
    /// <param name="context">The pipeline run context.</param>
    /// <param name="callbacks">Optional additional workflow callbacks.</param>
    /// <param name="isUpdateRun">Whether this is an incremental update run.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>An async stream of <see cref="PipelineRunResult"/> for each executed workflow.</returns>
    public static async IAsyncEnumerable<PipelineRunResult> RunPipelineAsync(
        Pipeline pipeline,
        GraphRagConfig config,
        PipelineRunContext context,
        IEnumerable<IWorkflowCallbacks>? callbacks = null,
        bool isUpdateRun = false,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var callbackList = callbacks?.ToList() ?? [];

        // Notify all callbacks that the pipeline is starting.
        foreach (var cb in callbackList)
        {
            cb.OnPipelineStart(pipeline.Names);
        }

        var results = new List<object>();

        foreach (var (name, function) in pipeline.Run())
        {
            ct.ThrowIfCancellationRequested();

            PipelineRunResult result;
            Exception? error = null;
            bool stop = false;

            try
            {
                foreach (var cb in callbackList)
                {
                    cb.OnWorkflowStart(name, function);
                }

                using var profiler = new WorkflowProfiler();
                var output = await function(config, context).ConfigureAwait(false);

                // Capture metrics after disposal.
                profiler.Dispose();
                if (profiler.Metrics is not null)
                {
                    context.Stats.Workflows[name] = profiler.Metrics;
                }

                if (output.Result is not null)
                {
                    results.Add(output.Result);
                }

                result = new PipelineRunResult(
                    Workflow: name,
                    Result: output.Result,
                    State: context.State,
                    Error: null);

                foreach (var cb in callbackList)
                {
                    cb.OnWorkflowEnd(name, function);
                }

                stop = output.Stop;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                error = ex;
                foreach (var cb in callbackList)
                {
                    cb.OnPipelineError(ex);
                }

                result = new PipelineRunResult(
                    Workflow: name,
                    Result: null,
                    State: context.State,
                    Error: ex);
            }

            yield return result;

            if (error is not null || stop)
            {
                break;
            }
        }

        // Notify all callbacks that the pipeline has ended.
        foreach (var cb in callbackList)
        {
            cb.OnPipelineEnd(results);
        }
    }
}
