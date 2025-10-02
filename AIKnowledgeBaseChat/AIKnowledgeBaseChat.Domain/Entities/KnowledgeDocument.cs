namespace AIKnowledgeBaseChat.Domain.Entities;

/// <summary>
///     Represents a knowledge document loaded from a markdown file.
/// </summary>
public class KnowledgeDocument
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="KnowledgeDocument" /> class.
    /// </summary>
    public KnowledgeDocument()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="KnowledgeDocument" /> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="content">The document content.</param>
    /// <param name="title">The document title.</param>
    public KnowledgeDocument(string fileName, string content, string title)
    {
        FileName = fileName;
        Content = content;
        Title = title;
        LastModified = DateTime.UtcNow;
    }

    /// <summary>
    ///     Gets or sets the file name of the document.
    /// </summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the content of the document.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the title extracted from the document.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the last modified timestamp.
    /// </summary>
    public DateTime LastModified { get; init; }
}