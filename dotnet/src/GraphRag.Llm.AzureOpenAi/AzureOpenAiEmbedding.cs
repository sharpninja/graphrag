// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.ClientModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using GraphRag.Common.Discovery;
using GraphRag.Llm.Types;
using OpenAI.Embeddings;

namespace GraphRag.Llm.AzureOpenAi;

/// <summary>
/// Azure OpenAI implementation of <see cref="ILlmEmbedding"/> using the Azure.AI.OpenAI SDK.
/// </summary>
[StrategyImplementation("azure_openai", typeof(ILlmEmbedding))]
public sealed class AzureOpenAiEmbedding : ILlmEmbedding
{
    private readonly EmbeddingClient _embeddingClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAiEmbedding"/> class.
    /// </summary>
    /// <param name="endpoint">The Azure OpenAI endpoint URI.</param>
    /// <param name="deploymentName">The deployment or model name.</param>
    /// <param name="tokenizer">The tokenizer to associate with this provider.</param>
    /// <param name="apiKey">An optional API key. When <c>null</c>, <see cref="DefaultAzureCredential"/> is used.</param>
    public AzureOpenAiEmbedding(Uri endpoint, string deploymentName, ITokenizer tokenizer, string? apiKey = null)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        ArgumentException.ThrowIfNullOrEmpty(deploymentName);
        ArgumentNullException.ThrowIfNull(tokenizer);

        Tokenizer = tokenizer;

        AzureOpenAIClient azureClient = apiKey is not null
            ? new AzureOpenAIClient(endpoint, new ApiKeyCredential(apiKey))
            : new AzureOpenAIClient(endpoint, new DefaultAzureCredential());

        _embeddingClient = azureClient.GetEmbeddingClient(deploymentName);
    }

    /// <inheritdoc />
    public ITokenizer Tokenizer { get; }

    /// <inheritdoc />
    public async Task<LlmEmbeddingResponse> EmbedAsync(LlmEmbeddingArgs args, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(args);

        var options = new EmbeddingGenerationOptions();
        if (args.Dimensions.HasValue)
        {
            options.Dimensions = args.Dimensions.Value;
        }

        if (args.User is not null)
        {
            options.EndUserId = args.User;
        }

        var response = await _embeddingClient.GenerateEmbeddingsAsync(args.Input, options, cancellationToken).ConfigureAwait(false);

        var embeddings = new List<IReadOnlyList<float>>(response.Value.Count);
        var totalTokens = 0;

        foreach (var embedding in response.Value)
        {
            embeddings.Add(embedding.ToFloats().ToArray());
        }

        totalTokens = response.Value.Usage.InputTokenCount;

        return new LlmEmbeddingResponse(
            Embeddings: embeddings,
            Usage: new LlmUsage(
                PromptTokens: totalTokens,
                CompletionTokens: 0,
                TotalTokens: totalTokens));
    }

    /// <inheritdoc />
    public LlmEmbeddingResponse Embed(LlmEmbeddingArgs args)
    {
        return EmbedAsync(args).GetAwaiter().GetResult();
    }
}
