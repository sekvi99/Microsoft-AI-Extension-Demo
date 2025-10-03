using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace AIKnowledgeBaseChat.Infrastructure.AI;

/// <summary>
///     Factory for creating configured OpenAI chat clients with middleware pipeline.
/// </summary>
public static class OpenAIChatClientFactory
{
    /// <summary>
    ///     Creates a configured IChatClient with OpenAI and middleware pipeline.
    /// </summary>
    /// <param name="apiKey">The OpenAI API key.</param>
    /// <param name="modelId">The model ID to use (e.g., "gpt-4").</param>
    /// <param name="cache">The distributed cache for caching responses.</param>
    /// <param name="loggerFactory">The logger factory for logging.</param>
    /// <returns>A configured IChatClient instance.</returns>
    public static IChatClient CreateChatClient(
        string apiKey,
        string modelId,
        IDistributedCache cache,
        ILoggerFactory loggerFactory)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key cannot be empty.", nameof(apiKey));

        if (string.IsNullOrWhiteSpace(modelId))
            throw new ArgumentException("Model ID cannot be empty.", nameof(modelId));

        // Create the base OpenAI client
        var openaiClient =
            new ChatClient(modelId, apiKey)
                .AsIChatClient();

        // Build pipeline with middleware
        return new ChatClientBuilder(openaiClient)
            .UseDistributedCache(cache)
            .UseLogging(loggerFactory)
            .ConfigureOptions(options =>
            {
                options.ModelId = modelId;
                options.Temperature = 0.7f;
                options.MaxOutputTokens = 2000;
            })
            .Build();
    }
}