using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MdConverter.Api.RequestModels;

public class MarkdownRequest
{
    [Required]
    [JsonPropertyName("markdownText")]
    public string MarkdownText { get; set; }
}

