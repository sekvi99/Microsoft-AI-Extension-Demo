# AI Knowledge Base Chat - Microsoft.Extensions.AI Demo

A comprehensive demonstration of the **Microsoft.Extensions.AI** NuGet package, showcasing how to build a knowledge base chat application using Clean Architecture principles.

## 📋 Overview

This project demonstrates:
- ✅ Clean Architecture with clear separation of concerns
- ✅ Microsoft.Extensions.AI integration with OpenAI
- ✅ Knowledge base from markdown files
- ✅ Conversation history management
- ✅ Streaming and non-streaming responses
- ✅ Middleware pipeline (caching, logging, telemetry)
- ✅ Dependency injection
- ✅ Console chat client interface

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────┐
│              Presentation Layer                     │
│           (Console Chat Client)                     │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│              Application Layer                      │
│  • ChatService: Orchestrates chat interactions      │
│  • Manages conversation history                     │
│  • Loads knowledge base into context                │
└──────────────────────┬──────────────────────────────┘
                       │
          ┌────────────┴────────────┐
          │                         │
┌─────────▼────────┐    ┌──────────▼──────────┐
│  IChatClient     │    │  IKnowledgeRepo     │
│  (OpenAI via     │    │  (Markdown Files)   │
│   Extensions.AI) │    │                     │
└──────────────────┘    └─────────────────────┘
│              Infrastructure Layer              │
└────────────────────────────────────────────────┘
```

### Layer Descriptions

- **Domain**: Core business entities and interfaces (no dependencies)
- **Application**: Business logic and use cases
- **Infrastructure**: External concerns (AI, file system, databases)
- **Presentation**: User interface (Console application)

## 📦 Prerequisites

- .NET 9.0 SDK or later
- OpenAI API key
- Visual Studio 2022, Visual Studio Code, or JetBrains Rider (optional)

## 🚀 Getting Started

### 1. Clone or Create the Project Structure

Create the following project structure:

```
AIKnowledgeBaseChat/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   └── Presentation/
|       └── appsettings.json
│       └── KnowledgeBase/
│           ├── dotnet-basics.md
│           ├── csharp-features.md
│           ├── aspnet-core.md
│           ├── entity-framework.md
│           └── best-practices.md
└── AIKnowledgeBaseChat.sln
```

### 2. Install Dependencies

Navigate to each project directory and restore packages:

```bash
# Navigate to solution directory
cd AIKnowledgeBaseChat

# Restore all packages
dotnet restore
```

### 3. Configure OpenAI API Key and Knowledge Base path

**Using appsettings.json**

Edit `src/Presentation/appsettings.json`:

```json
{
  "OpenAI": {
    "ApiKey": "your-api-key",
    "ModelId": "gpt-4"
  },
  "KnowledgeBase": {
    "Path": "your-knowledge-base-path"
  }
}
```

### 4. Add Knowledge Base Files

Copy the 5 sample markdown files to `src/Presentation/KnowledgeBase/`:
- `dotnet-basics.md`
- `csharp-features.md`
- `aspnet-core.md`
- `entity-framework.md`
- `best-practices.md`

These files contain the knowledge that the AI will use to answer questions.

### 5. Build the Solution

```bash
# From solution root
dotnet build
```

### 6. Run the Application

```bash
# From solution root
dotnet run --project src/AIKnowledgeBaseChat.Presentation

# Or navigate to the project
cd src/AIKnowledgeBaseChat.Presentation
dotnet run
```

## 💬 Using the Chat Application

Once running, you'll see:

```
╔════════════════════════════════════════════════════════════╗
║        .NET Knowledge Base AI Chat Assistant               ║
║              Powered by Microsoft.Extensions.AI            ║
╚════════════════════════════════════════════════════════════╝

Ask me anything about .NET development!
Commands:
  - Type your question and press Enter
  - Type '/stream' before your question for streaming responses
  - Type '/clear' to clear conversation history
  - Type '/exit' or '/quit' to exit
```

### Example Interactions

**Normal Response:**
```
You: What is dependency injection in .NET?
AI: Dependency injection (DI) is a design pattern built into .NET...
```

**Streaming Response:**
```
You: /stream Explain SOLID principles
AI: SOLID principles are five design principles for object-oriented programming...
```

**Clear History:**
```
You: /clear
✓ Conversation history cleared.
```

## 🎯 Features Demonstrated

### 1. Clean Architecture
- **Domain Layer**: Pure business entities, no dependencies
- **Application Layer**: Business logic, interfaces
- **Infrastructure Layer**: External services (AI, file system)
- **Presentation Layer**: User interface

### 2. Microsoft.Extensions.AI Features

#### IChatClient Interface
```csharp
IChatClient chatClient = /* configured client */;
var response = await chatClient.GetResponseAsync("Your question");
```

#### Streaming Responses
```csharp
await foreach (var update in chatClient.GetStreamingResponseAsync(prompt))
{
    Console.Write(update.Text);
}
```

#### Middleware Pipeline
```csharp
var client = new ChatClientBuilder(baseClient)
    .UseDistributedCache(cache)
    .UseLogging(loggerFactory)
    .ConfigureOptions(options => options.Temperature = 0.7f)
    .Build();
```

#### Conversation History
```csharp
List<ChatMessage> history = [
    new(ChatRole.System, "You are an expert..."),
    new(ChatRole.User, "Question?"),
];
var response = await client.GetResponseAsync(history);
history.Add(response.Message);
```

### 3. Knowledge Base Integration

The application loads markdown files and incorporates them into the system prompt:

```csharp
private async Task EnsureKnowledgeBaseLoadedAsync(CancellationToken cancellationToken)
{
    var knowledgeBase = await _knowledgeRepository.GetCombinedKnowledgeBaseAsync(cancellationToken);
    
    var systemMessage = new ChatMessage(ChatRole.System, 
        $"You are a helpful AI assistant with expertise in .NET development. " +
        $"Answer ONLY based on the following knowledge base:\n\n{knowledgeBase}");
    
    _conversationHistory.Insert(0, systemMessage);
}
```

### 4. Dependency Injection

All services are registered in the DI container:

```csharp
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Later usage
public class ChatService
{
    public ChatService(IChatClient chatClient, IKnowledgeRepository repository)
    {
        _chatClient = chatClient;
        _repository = repository;
    }
}
```

## 🧪 Testing

### Unit Testing Example

```csharp
public class MockChatClient : IChatClient
{
    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ChatResponse(
            new ChatMessage(ChatRole.Assistant, "Mock response")));
    }
}

[Fact]
public async Task ChatService_SendsMessage_ReturnsResponse()
{
    var mockClient = new MockChatClient();
    var service = new ChatService(mockClient, mockRepository, mockLogger);
    
    var result = await service.SendMessageAsync("Test");
    
    Assert.Equal("Mock response", result);
}
```

## 📚 Project Structure Details

### Domain Layer (`AIKnowledgeBaseChat.Domain`)
- `Entities/KnowledgeDocument.cs`: Domain entity
- `Interfaces/IKnowledgeRepository.cs`: Repository abstraction
- `Interfaces/IChatService.cs`: Service abstraction
- **No external dependencies**

### Application Layer (`AIKnowledgeBaseChat.Application`)
- `Services/ChatService.cs`: Core business logic
- `DTOs/`: Data transfer objects
- `Extensions/ServiceCollectionExtensions.cs`: DI registration
- **Dependencies**: Domain, Microsoft.Extensions.AI.Abstractions

### Infrastructure Layer (`AIKnowledgeBaseChat.Infrastructure`)
- `Repositories/MarkdownKnowledgeRepository.cs`: File system access
- `AI/OpenAIChatClientFactory.cs`: AI client configuration
- `Extensions/ServiceCollectionExtensions.cs`: DI registration
- **Dependencies**: Domain, Application, Microsoft.Extensions.AI

### Presentation Layer (`AIKnowledgeBaseChat.Console`)
- `Program.cs`: Application entry point and UI
- `appsettings.json`: Configuration
- `KnowledgeBase/`: Markdown knowledge files
- **Dependencies**: Application, Infrastructure

## 🔧 Configuration Options

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Extensions.AI": "Debug"
    }
  },
  "OpenAI": {
    "ApiKey": "your-api-key",
    "ModelId": "gpt-4"
  },
  "KnowledgeBase": {
    "Path": "KnowledgeBase"
  }
}
```

### Supported Models

- `gpt-4` (recommended for best results)
- `gpt-4-turbo`
- `gpt-3.5-turbo` (faster, lower cost)
- Any OpenAI-compatible model

## 🐛 Troubleshooting

### Issue: "OpenAI API key not configured"
**Solution**: Ensure your API key is properly configured in appsettings.json, user secrets, or environment variables.

### Issue: "Knowledge base path not found"
**Solution**: Verify the markdown files exist in the `KnowledgeBase` directory and the path in configuration is correct.

### Issue: "Rate limit exceeded"
**Solution**: Implement the custom RateLimitingChatClient middleware or reduce request frequency.

### Issue: Token limit exceeded
**Solution**: Trim conversation history or summarize older messages before sending.

## 🤝 Contributing

To extend this demo:

1. **Add more knowledge documents**: Place new `.md` files in the `KnowledgeBase` directory
2. **Add middleware**: Create custom `DelegatingChatClient` implementations
3. **Change AI provider**: Swap OpenAI for Azure AI, Ollama, or other providers
4. **Add features**: Implement RAG, vector search, or multi-modal support

## 📄 License

This demo project is provided as educational material for learning Microsoft.Extensions.AI.

## 🙏 Acknowledgments

- Microsoft .NET Team for Microsoft.Extensions.AI
- OpenAI for the GPT models
- The .NET community

---

**Happy Coding! 🚀**

For questions or issues, refer to the official [Microsoft.Extensions.AI documentation](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai).