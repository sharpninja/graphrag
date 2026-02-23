// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Hasher;
using GraphRag.Llm.Types;

namespace GraphRag.Llm.Cache;

/// <summary>
/// Static helper that creates cache keys for LLM requests by hashing model and message content.
/// </summary>
public static class LlmCacheKeyCreator
{
    /// <summary>
    /// Creates a cache key for the specified completion arguments and model name.
    /// </summary>
    /// <param name="args">The completion request arguments.</param>
    /// <param name="model">The model name to include in the cache key.</param>
    /// <returns>A deterministic hash string suitable for use as a cache key.</returns>
    public static string CreateKey(LlmCompletionArgs args, string? model = null)
    {
        var messages = new List<Dictionary<string, object?>>(args.Messages.Count);
        foreach (var msg in args.Messages)
        {
            messages.Add(new Dictionary<string, object?>
            {
                ["role"] = msg.Role,
                ["content"] = msg.Content,
            });
        }

        var data = new Dictionary<string, object?>
        {
            ["model"] = model,
            ["messages"] = messages,
        };

        return HashHelper.HashData(data);
    }
}
