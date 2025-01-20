using System.ComponentModel.DataAnnotations;

namespace MdConverter.Api.RequestModels;

public class MarkdownRequest
{
    [Required]
    public string MarkdownText { get; set; }
}

