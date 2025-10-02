using AIKnowledgeBaseChat.Domain.Entities;

namespace AIKnowledgeBaseChat.Domain.Interfaces;

/// <summary>
///     Repository interface for accessing knowledge documents.
/// </summary>
public interface IKnowledgeRepository
{
    /// <summary>
    ///     Gets all knowledge documents from the repository.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of knowledge documents.</returns>
    Task<IEnumerable<KnowledgeDocument>> GetAllDocumentsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a specific document by its file name.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The knowledge document or null if not found.</returns>
    Task<KnowledgeDocument?> GetDocumentByNameAsync(
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all knowledge documents combined into a single string.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The combined knowledge base content.</returns>
    Task<string> GetCombinedKnowledgeBaseAsync(
        CancellationToken cancellationToken = default);
}