using MdConverter.Api.Filters;
using MdConverter.Api.RequestModels;
using MdConverter.Api.ResponseModels;
using MdConverter.Application.Services;
using MdConverter.Core.Abstractions.Services;
using MdConverter.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MdConverter.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class DocumentController : ControllerBase
{
    private readonly JwtService jwtService;
    private readonly IDocumentService documentService;

    public DocumentController(JwtService jwtService, IDocumentService documentService)
    {
        this.jwtService = jwtService;
        this.documentService = documentService;
    }

    [HttpGet]// Получение документов пользователя по имени пользователя (из токена)
    [Authorize]  // Это требование для того, чтобы запрос был защищен
    public async Task<ActionResult<List<DocumentResponse>>> GetAllDocumentsByUserName()
    {
        // Извлекаем токен из заголовка Authorization
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("Token is missing.");
        }

        try
        {
            // Извлекаем имя пользователя из токена
            var userName = jwtService.GetUserNameFromToken(token);

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("Invalid token.");
            }

            // Получаем документы пользователя по имени
            var documents = await documentService.GetAllDocumentsByUserName(userName);
            var response = documents.Select(u => new DocumentResponse(u.Id, u.Name, u.UserName)).ToList();
            return Ok(response);
        }
        catch (Exception)
        {
            return Unauthorized("Invalid token.");
        }
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