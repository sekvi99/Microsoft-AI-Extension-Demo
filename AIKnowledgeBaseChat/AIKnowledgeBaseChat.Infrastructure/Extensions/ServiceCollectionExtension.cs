using AIKnowledgeBaseChat.Domain.Interfaces;
using AIKnowledgeBaseChat.Infrastructure.AI;
using AIKnowledgeBaseChat.Infrastructure.Repositories;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AIKnowledgeBaseChat.Infrastructure.Extensions;

/// <summary>
///     Extension methods for registering infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds infrastructure layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        // Add distributed cache for caching AI responses
        services.AddDistributedMemoryCache();

        // Register knowledge repository
        var knowledgeBasePath = configuration["KnowledgeBase:Path"]
                                ?? Path.Combine(AppContext.BaseDirectory, "KnowledgeBase");

        services.AddSingleton<IKnowledgeRepository>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<MarkdownKnowledgeRepository>>();
            return new MarkdownKnowledgeRepository(knowledgeBasePath, logger);
        });

        // Register AI Chat Client with OpenAI
        services.AddSingleton<IChatClient>(sp =>
        {
            var apiKey = configuration["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException(
                    "OpenAI API key not configured. Please set it in appsettings.json, " +
                    "user secrets, or environment variables.");

            var modelId = configuration["OpenAI:ModelId"] ?? "gpt-4";
            var cache = sp.GetRequiredService<IDistributedCache>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            return OpenAIChatClientFactory.CreateChatClient(apiKey, modelId, cache, loggerFactory);
        });

        return services;
    }
}