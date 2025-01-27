using System.ComponentModel.DataAnnotations;

namespace MdConverter.Api.RequestModels;

public class DocumentRequest
{
    public string Name { get; set; } // Название документа
    public string MarkdownText { get; set; } // Текст Markdown
}
