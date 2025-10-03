namespace AIKnowledgeBaseChat.Application.DTOs;

/// <summary>
///     Represents a chat response from the AI.
/// </summary>
/// <param name="Message">The AI's response message.</param>
/// <param name="Timestamp">The timestamp when the response was generated.</param>
public record ChatResponse(string Message, DateTime Timestamp);