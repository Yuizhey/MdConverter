using System.Text.Json.Serialization;

namespace MdConverter.Api.ResponseModels;

public class DocumentContentResponse
{
    [JsonPropertyName("content")]
    public string Content { get; set; }
}