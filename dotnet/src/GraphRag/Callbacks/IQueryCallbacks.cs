// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Callbacks;

/// <summary>
/// Defines callbacks for query execution lifecycle events.
/// </summary>
public interface IQueryCallbacks
{
    /// <summary>
    /// Called when context data is available.
    /// </summary>
    /// <param name="context">The context object.</param>
    void OnContext(object context);

    /// <summary>
    /// Called when a map response phase starts.
    /// </summary>
    /// <param name="contexts">The list of context strings to map over.</param>
    void OnMapResponseStart(IReadOnlyList<string> contexts);

    /// <summary>
    /// Called when a map response phase ends.
    /// </summary>
    /// <param name="outputs">The list of mapped outputs.</param>
    void OnMapResponseEnd(IReadOnlyList<object> outputs);

    /// <summary>
    /// Called when a reduce response phase starts.
    /// </summary>
    /// <param name="context">The context for the reduce phase.</param>
    void OnReduceResponseStart(object context);

    /// <summary>
    /// Called when a reduce response phase ends.
    /// </summary>
    /// <param name="output">The final reduced output.</param>
    void OnReduceResponseEnd(string output);

    /// <summary>
    /// Called when a new token is received from the LLM.
    /// </summary>
    /// <param name="token">The new token.</param>
    void OnLlmNewToken(string token);
}
