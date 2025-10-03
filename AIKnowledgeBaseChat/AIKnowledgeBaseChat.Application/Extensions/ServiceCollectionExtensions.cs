namespace AIKnowledgeBaseChat.Application.Extensions;

using AIKnowledgeBaseChat.Application.Services;
using AIKnowledgeBaseChat.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IChatService, ChatService>();
        return services;
    }
}