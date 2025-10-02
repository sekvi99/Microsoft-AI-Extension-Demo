namespace AIKnowledgeBaseChat.Domain.Interfaces;

/// <summary>
///     Service interface for chat interactions with the AI.
/// </summary>
public interface IChatService
{
    /// <summary>
    ///     Sends a message to the AI and gets a complete response.
    /// </summary>
    /// <param name="userMessage">The user's message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AI's response.</returns>
    Task<string> SendMessageAsync(
        string userMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sends a message to the AI and streams the response.
    /// </summary>
    /// <param name="userMessage">The user's message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An async stream of response chunks.</returns>
    IAsyncEnumerable<string> StreamMessageAsync(
        string userMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Clears the conversation history.
    /// </summary>
    Task ClearHistoryAsync();
}