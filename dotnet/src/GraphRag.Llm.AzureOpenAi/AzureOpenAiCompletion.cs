// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.ClientModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using GraphRag.Common.Discovery;
using GraphRag.Llm.Types;
using OpenAI.Chat;

namespace GraphRag.Llm.AzureOpenAi;

/// <summary>
/// Azure OpenAI implementation of <see cref="ILlmCompletion"/> using the Azure.AI.OpenAI SDK.
/// </summary>
[StrategyImplementation("azure_openai", typeof(ILlmCompletion))]
public sealed class AzureOpenAiCompletion : ILlmCompletion
{
    private readonly ChatClient _chatClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAiCompletion"/> class.
    /// </summary>
    /// <param name="endpoint">The Azure OpenAI endpoint URI.</param>
    /// <param name="deploymentName">The deployment or model name.</param>
    /// <param name="tokenizer">The tokenizer to associate with this provider.</param>
    /// <param name="apiKey">An optional API key. When <c>null</c>, <see cref="DefaultAzureCredential"/> is used.</param>
    public AzureOpenAiCompletion(Uri endpoint, string deploymentName, ITokenizer tokenizer, string? apiKey = null)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        ArgumentException.ThrowIfNullOrEmpty(deploymentName);
        ArgumentNullException.ThrowIfNull(tokenizer);

        Tokenizer = tokenizer;

        AzureOpenAIClient azureClient = apiKey is not null
            ? new AzureOpenAIClient(endpoint, new ApiKeyCredential(apiKey))
            : new AzureOpenAIClient(endpoint, new DefaultAzureCredential());

        _chatClient = azureClient.GetChatClient(deploymentName);
    }

    /// <inheritdoc />
    public ITokenizer Tokenizer { get; }

    /// <inheritdoc />
    public async Task<LlmCompletionResponse> CompleteAsync(LlmCompletionArgs args, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(args);

        var chatMessages = ConvertMessages(args.Messages);
        var options = BuildOptions(args);

        var response = await _chatClient.CompleteChatAsync(chatMessages, options, cancellationToken).ConfigureAwait(false);
        var completion = response.Value;

        var content = completion.Content.Count > 0 ? completion.Content[0].Text : null;

        return new LlmCompletionResponse(
            Content: content,
            Usage: new LlmUsage(
                PromptTokens: completion.Usage.InputTokenCount,
                CompletionTokens: completion.Usage.OutputTokenCount,
                TotalTokens: completion.Usage.TotalTokenCount));
    }

    /// <inheritdoc />
    public LlmCompletionResponse Complete(LlmCompletionArgs args)
    {
        return CompleteAsync(args).GetAwaiter().GetResult();
    }

    private static List<ChatMessage> ConvertMessages(IReadOnlyList<LlmMessage> messages)
    {
        var chatMessages = new List<ChatMessage>(messages.Count);
        foreach (var msg in messages)
        {
            ChatMessage chatMessage = msg.Role.ToLowerInvariant() switch
            {
                "system" => new SystemChatMessage(msg.Content),
                "assistant" => new AssistantChatMessage(msg.Content),
                "tool" => new ToolChatMessage(msg.ToolCallId ?? string.Empty, msg.Content),
                _ => new UserChatMessage(msg.Content),
            };

            chatMessages.Add(chatMessage);
        }

        return chatMessages;
    }

    private static ChatCompletionOptions BuildOptions(LlmCompletionArgs args)
    {
        var options = new ChatCompletionOptions();

        if (args.Temperature.HasValue)
        {
            options.Temperature = (float)args.Temperature.Value;
        }

        if (args.TopP.HasValue)
        {
            options.TopP = (float)args.TopP.Value;
        }

        if (args.MaxTokens.HasValue)
        {
            options.MaxOutputTokenCount = args.MaxTokens.Value;
        }

        if (args.Seed.HasValue)
        {
            options.Seed = args.Seed.Value;
        }

        if (args.User is not null)
        {
            options.EndUserId = args.User;
        }

        return options;
    }
}
