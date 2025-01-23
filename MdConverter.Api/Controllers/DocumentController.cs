using MdConverter.Api.Filters;
using MdConverter.Api.ResponseModels;
using MdConverter.Core.Abstractions.Services;
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

    [HttpGet]
    public async Task<ActionResult<List<DocumentResponse>>> GetAllDocuments()
    {
        var documents = await documentService.GetAllDocuments();
        var response = documents.Select(u => new DocumentResponse(u.Id, u.Name, u.UserName));
        return Ok(response);
    }

    [MyAuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult<List<DocumentResponse>>> GetAllDocumentsByUserId(Guid id)
    {
        var documents = await documentService.GetAllDocumentsByUserId(id);
        var response = documents.Select(u => new DocumentResponse(u.Id, u.Name, u.UserName));
        return Ok(response);
    }
    
    [MyAuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult<List<DocumentResponse>>> GetAllDocumentsByUserName(string userName)
    {
        var documents = await documentService.GetAllDocumentsByUserName(userName);
        var response = documents.Select(u => new DocumentResponse(u.Id, u.Name, u.UserName));
        return Ok(response);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteDocument(Guid documentId)
    {
        var document = await documentService.GetDocumentById(documentId);
        return Ok(document.Id);
    }
}