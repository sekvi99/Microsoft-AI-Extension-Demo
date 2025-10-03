namespace AIKnowledgeBaseChat.Application.DTOs;

/// <summary>
///     Represents a chat request from the user.
/// </summary>
/// <param name="Message">The user's message.</param>
public record ChatRequest(string Message);