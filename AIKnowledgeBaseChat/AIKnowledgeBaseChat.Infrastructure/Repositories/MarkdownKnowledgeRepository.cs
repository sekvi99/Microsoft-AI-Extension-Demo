using System.Text;
using AIKnowledgeBaseChat.Domain.Entities;
using AIKnowledgeBaseChat.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIKnowledgeBaseChat.Infrastructure.Repositories;

/// <summary>
///     Repository for loading knowledge documents from markdown files.
/// </summary>
public class MarkdownKnowledgeRepository : IKnowledgeRepository
{
    private readonly string _knowledgeBasePath;
    private readonly ILogger<MarkdownKnowledgeRepository> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MarkdownKnowledgeRepository" /> class.
    /// </summary>
    /// <param name="knowledgeBasePath">The path to the knowledge base directory.</param>
    /// <param name="logger">The logger.</param>
    public MarkdownKnowledgeRepository(
        string knowledgeBasePath,
        ILogger<MarkdownKnowledgeRepository> logger)
    {
        _knowledgeBasePath = knowledgeBasePath ?? throw new ArgumentNullException(nameof(knowledgeBasePath));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (!Directory.Exists(_knowledgeBasePath))
            throw new DirectoryNotFoundException(
                $"Knowledge base path not found: {_knowledgeBasePath}");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<KnowledgeDocument>> GetAllDocumentsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var markdownFiles = Directory.GetFiles(_knowledgeBasePath, "*.md");
            var documents = new List<KnowledgeDocument>();

            foreach (var filePath in markdownFiles)
            {
                var document = await LoadDocumentAsync(filePath, cancellationToken);
                if (document != null) documents.Add(document);
            }

            _logger.LogInformation("Loaded {Count} knowledge documents from {Path}",
                documents.Count, _knowledgeBasePath);

            return documents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading knowledge documents from {Path}", _knowledgeBasePath);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<KnowledgeDocument?> GetDocumentByNameAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty.", nameof(fileName));

        try
        {
            var filePath = Path.Combine(_knowledgeBasePath, fileName);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Document not found: {FileName}", fileName);
                return null;
            }

            return await LoadDocumentAsync(filePath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading document: {FileName}", fileName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> GetCombinedKnowledgeBaseAsync(
        CancellationToken cancellationToken = default)
    {
        var documents = await GetAllDocumentsAsync(cancellationToken);
        var sb = new StringBuilder();

        foreach (var doc in documents.OrderBy(d => d.FileName))
        {
            sb.AppendLine($"## Document: {doc.Title}");
            sb.AppendLine($"## Source File: {doc.FileName}");
            sb.AppendLine();
            sb.AppendLine(doc.Content);
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
        }

        _logger.LogDebug("Combined knowledge base created with {Length} characters", sb.Length);
        return sb.ToString();
    }

    /// <summary>
    ///     Loads a document from the specified file path.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The loaded document or null if loading failed.</returns>
    private async Task<KnowledgeDocument?> LoadDocumentAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        try
        {
            var content = await File.ReadAllTextAsync(filePath, cancellationToken);
            var fileName = Path.GetFileName(filePath);
            var title = ExtractTitle(content) ?? Path.GetFileNameWithoutExtension(fileName);
            var lastModified = File.GetLastWriteTimeUtc(filePath);

            _logger.LogDebug("Loaded document: {FileName} ({Title})", fileName, title);

            return new KnowledgeDocument
            {
                FileName = fileName,
                Content = content,
                Title = title,
                LastModified = lastModified
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file: {FilePath}", filePath);
            return null;
        }
    }

    /// <summary>
    ///     Extracts the title from markdown content (first # heading).
    /// </summary>
    /// <param name="content">The markdown content.</param>
    /// <returns>The extracted title or null if not found.</returns>
    private static string? ExtractTitle(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return null;

        var lines = content.Split('\n');
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("# ")) return trimmed.Substring(2).Trim();
        }

        return null;
    }
}