using Markdig;
using MdConverter.Api.RequestModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MdConverter.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class MarkDownController : ControllerBase
{
    [HttpPost]
    public IActionResult Render([FromBody] MarkdownRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MarkdownText))
            return BadRequest("Markdown text cannot be empty.");

        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        var html = Markdown.ToHtml(request.MarkdownText, pipeline);
        
        return Ok(new { html });
    }
}