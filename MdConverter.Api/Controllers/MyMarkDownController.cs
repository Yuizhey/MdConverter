using MdConverter.Api.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Markdown;

namespace MdConverter.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MyMarkDownController : ControllerBase
    {
        [HttpPost]
        public IActionResult Render([FromBody] MarkdownRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MarkdownText))
                return BadRequest("Markdown text cannot be empty.");

            MarkdownParser mdParser = new MarkdownParser();
            MarkdownRenderer mdRenderer = new MarkdownRenderer();
            MarkdownProcessor mdProcessor = new MarkdownProcessor(mdParser, mdRenderer);
            var html = mdProcessor.GetHtml(request.MarkdownText);

            return Ok(new { html });
        }
    }
}