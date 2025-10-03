using System.Runtime.CompilerServices;
using AIKnowledgeBaseChat.Domain.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace AIKnowledgeBaseChat.Application.Services;

/// <summary>
///     Service responsible for managing chat interactions with the AI.
/// </summary>
public class ChatService : IChatService
{
    private readonly IChatClient _chatClient;
    private readonly List<ChatMessage> _conversationHistory;
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly ILogger<ChatService> _logger;
    private string? _knowledgeBaseContext;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ChatService" /> class.
    /// </summary>
    /// <param name="chatClient">The AI chat client.</param>
    /// <param name="knowledgeRepository">The knowledge repository.</param>
    /// <param name="logger">The logger.</param>
    public ChatService(
        IChatClient chatClient,
        IKnowledgeRepository knowledgeRepository,
        ILogger<ChatService> logger)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        _knowledgeRepository = knowledgeRepository ?? throw new ArgumentNullException(nameof(knowledgeRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _conversationHistory = new List<ChatMessage>();
    }

    /// <inheritdoc />
    public async Task<string> SendMessageAsync(
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userMessage))
            throw new ArgumentException("User message cannot be empty.", nameof(userMessage));

        try
        {
            // Ensure knowledge base is loaded
            await EnsureKnowledgeBaseLoadedAsync(cancellationToken);

            // Add user message to history
            _conversationHistory.Add(new ChatMessage(ChatRole.User, userMessage));
            _logger.LogDebug("Added user message to conversation history");

            // Get response from AI
            var response = await _chatClient.GetResponseAsync(
                _conversationHistory,
                cancellationToken: cancellationToken);

            // Add assistant response to history
            if (response.Messages != null)
            {
                foreach (var message in response.Messages)
                {
                    _logger.LogDebug($"Received message: {message}");
                    _conversationHistory.Add(message);
                }

                _logger.LogInformation("Chat response generated successfully");
                return response.Messages.FirstOrDefault().Text ?? "I couldn't generate a response.";
            }

            _logger.LogWarning("Received null message in response");
            return "I couldn't generate a response.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to AI");
            throw;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> StreamMessageAsync(
        string userMessage,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userMessage))
            throw new ArgumentException("User message cannot be empty.", nameof(userMessage));

        // Ensure knowledge base is loaded
        await EnsureKnowledgeBaseLoadedAsync(cancellationToken);

        // Add user message to history
        _conversationHistory.Add(new ChatMessage(ChatRole.User, userMessage));
        _logger.LogDebug("Added user message to conversation history for streaming");

        var updates = new List<ChatResponseUpdate>();

        await foreach (var update in _chatClient.GetStreamingResponseAsync(
                           _conversationHistory,
                           cancellationToken: cancellationToken))
            if (!string.IsNullOrEmpty(update.Text))
            {
                updates.Add(update);
                yield return update.Text;
            }

        // Add the complete response to history
        _conversationHistory.AddMessages(updates);
        _logger.LogInformation("Streaming response completed and added to history");
    }

    /// <inheritdoc />
    public Task ClearHistoryAsync()
    {
        _conversationHistory.Clear();
        _knowledgeBaseContext = null;
        _logger.LogInformation("Conversation history cleared");
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Ensures the knowledge base is loaded into the conversation context.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    private async Task EnsureKnowledgeBaseLoadedAsync(CancellationToken cancellationToken)
    {
        if (_knowledgeBaseContext == null)
        {
            _logger.LogInformation("Loading knowledge base into conversation context");
            _knowledgeBaseContext = await _knowledgeRepository.GetCombinedKnowledgeBaseAsync(cancellationToken);

            // Initialize conversation with system message containing knowledge base
            var systemMessage = new ChatMessage(ChatRole.System,
                $@"You are a helpful AI assistant with expertise in .NET development. 
You should ONLY answer questions based on the following knowledge base. 
If a question is outside this knowledge base, politely inform the user that you can only answer questions about the topics covered in your knowledge base.

Knowledge Base:
{_knowledgeBaseContext}

Guidelines:
- Be concise and accurate
- Reference specific sections from the knowledge base when applicable
- If you're unsure or the information isn't in the knowledge base, say so
- Provide code examples when relevant
- Maintain a professional and helpful tone");

            _conversationHistory.Insert(0, systemMessage);
            _logger.LogInformation("Knowledge base loaded into conversation context");
        }
    }
}