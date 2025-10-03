using AIKnowledgeBaseChat.Application.Extensions;
using AIKnowledgeBaseChat.Domain.Interfaces;
using AIKnowledgeBaseChat.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Build the host
var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>(true);

// Register services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Build and run
var host = builder.Build();

// Run the chat application
await RunChatApplicationAsync(host.Services);

/// <summary>
/// Runs the interactive chat application.
/// </summary>
/// <param name="services">The service provider.</param>
static async Task RunChatApplicationAsync(IServiceProvider services)
{
    var chatService = services.GetRequiredService<IChatService>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    // Display welcome screen
    DisplayWelcomeScreen();

    while (true)
    {
        // Get user input
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("You: ");
        Console.ResetColor();

        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            continue;

        input = input.Trim();

        // Handle exit commands
        if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase) ||
            input.Equals("/quit", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Goodbye! Happy coding! 👋");
            Console.ResetColor();
            break;
        }

        // Handle clear command
        if (input.Equals("/clear", StringComparison.OrdinalIgnoreCase))
        {
            await chatService.ClearHistoryAsync();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("✓ Conversation history cleared.\n");
            Console.ResetColor();
            continue;
        }

        // Handle help command
        if (input.Equals("/help", StringComparison.OrdinalIgnoreCase))
        {
            DisplayHelp();
            continue;
        }

        // Check for stream mode
        var streamMode = false;
        if (input.StartsWith("/stream ", StringComparison.OrdinalIgnoreCase))
        {
            streamMode = true;
            input = input.Substring(8).Trim();
        }

        if (string.IsNullOrWhiteSpace(input))
            continue;

        // Display AI prefix
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("AI: ");
        Console.ResetColor();

        try
        {
            if (streamMode)
            {
                // Stream the response
                await foreach (var chunk in chatService.StreamMessageAsync(input)) Console.Write(chunk);
                Console.WriteLine();
            }
            else
            {
                // Get complete response
                var response = await chatService.SendMessageAsync(input);
                Console.WriteLine(response);
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
            logger.LogError(ex, "Error processing chat message");
        }

        Console.WriteLine();
    }
}

/// <summary>
/// Displays the welcome screen.
/// </summary>
static void DisplayWelcomeScreen()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║        .NET Knowledge Base AI Chat Assistant               ║");
    Console.WriteLine("║              Powered by Microsoft.Extensions.AI            ║");
    Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine("Ask me anything about .NET development!");
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Available Commands:");
    Console.ResetColor();
    Console.WriteLine("  • Type your question and press Enter");
    Console.WriteLine("  • /stream [question] - Get streaming responses");
    Console.WriteLine("  • /clear - Clear conversation history");
    Console.WriteLine("  • /help - Show this help message");
    Console.WriteLine("  • /exit or /quit - Exit the application");
    Console.WriteLine();
    Console.WriteLine(new string('─', 60));
    Console.WriteLine();
}

/// <summary>
/// Displays the help information.
/// </summary>
static void DisplayHelp()
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("═══════════════════ HELP ═══════════════════");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine("This AI assistant can answer questions about:");
    Console.WriteLine("  • .NET Basics (runtime, BCL, SDK)");
    Console.WriteLine("  • C# Features (pattern matching, records, LINQ)");
    Console.WriteLine("  • ASP.NET Core (web apps, APIs, middleware)");
    Console.WriteLine("  • Entity Framework Core (ORM, queries, migrations)");
    Console.WriteLine("  • .NET Best Practices (SOLID, Clean Architecture)");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  /stream [question] - Stream the AI response in real-time");
    Console.WriteLine("  /clear             - Reset conversation history");
    Console.WriteLine("  /help              - Show this help message");
    Console.WriteLine("  /exit or /quit     - Exit the application");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  • What is dependency injection?");
    Console.WriteLine("  • /stream Explain SOLID principles");
    Console.WriteLine("  • How do I use async/await in C#?");
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("═══════════════════════════════════════════");
    Console.ResetColor();
    Console.WriteLine();
}