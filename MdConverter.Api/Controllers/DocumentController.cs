using MdConverter.Api.Filters;
using MdConverter.Api.RequestModels;
using MdConverter.Api.ResponseModels;
using MdConverter.Core.Abstractions.Services;
using MdConverter.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace MdConverter.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService documentService;

    public DocumentController(IDocumentService documentService)
    {
        this.documentService = documentService;
    }
    
    [MyAuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult<List<DocumentResponse>>> GetAllDocumentsByUserName(string userName)
    {
        var documents = await documentService.GetAllDocumentsByUserName(userName);
        var response = documents.Select(u => new DocumentResponse(u.Id, u.Name, u.UserName));
        return Ok(response);
    }
    
    [MyAuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateUser([FromBody]DocumentRequest userRequest)
    {
        var (document, error) = Document.Create(Guid.NewGuid(), userRequest.name, userRequest.userName);
        if (!string.IsNullOrEmpty(error))
        {
            return BadRequest(error);
        }
        
        var userId = await documentService.CreateDocument(document);
        
        return Ok(userId);
    }
    
    [MyAuthorizeFilter]
    [HttpDelete]
    public async Task<ActionResult<Guid>> DeleteDocument(Guid documentId)
    {
        var document_id = await documentService.DeleteDocument(documentId);
        return Ok(document_id);
    }
}