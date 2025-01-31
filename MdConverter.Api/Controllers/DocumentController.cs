using System.Text;
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
    private readonly MinioService minioService;

    public DocumentController(JwtService jwtService, IDocumentService documentService, MinioService minioService)
    {
        this.jwtService = jwtService;
        this.documentService = documentService;
        this.minioService = minioService;
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
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> CreateDocument([FromBody] DocumentRequest userRequest)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("Token is missing.");
        }

        var userName = jwtService.GetUserNameFromToken(token);
        if (string.IsNullOrEmpty(userName))
        {
            return Unauthorized("Invalid token.");
        }

        var fileName = $"{userName}/{userRequest.Name}.md";
    
        // Проверяем, существует ли файл на MinIO
        var fileExists = await minioService.FileExistsAsync(fileName);
    
        if (fileExists)
        {
            // Если файл существует, можно решить: перезаписывать его или возвращать ошибку
            // В этом случае перезаписываем
            await minioService.DeleteFileAsync(fileName);  // Удаляем старый файл
        }

        // Создание документа
        var (document, error) = Document.Create(Guid.NewGuid(), userRequest.Name, userName);
        if (!string.IsNullOrEmpty(error))
        {
            return BadRequest(error);
        }

        // Сохранение нового файла
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(userRequest.MarkdownText));
        await minioService.UploadFileAsync(fileName, fileStream);

        // Сохраняем информацию о документе в базе данных
        var userId = await documentService.CreateDocument(document);

        return Ok(userId);
    }


        // Удаление документа
    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteDocument(Guid documentId)
    {
        try
        {
            var document = await documentService.GetDocumentById(documentId);
            if (document == null)
            {
                return NotFound("Document not found.");
            }

            var fileName = $"{document.UserName}/{document.Name}";

                // Удаляем файл из Minio
            await minioService.DeleteFileAsync(fileName);

                // Удаляем документ из базы данных
            await documentService.DeleteDocument(documentId);

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting document: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
        
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<DocumentContentResponse>> GetDocumentContent(string documentName)
    {
        try
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userName = jwtService.GetUserNameFromToken(token);

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("Invalid token.");
            }

            var fileName = $"{userName}/{documentName}.md";
            var fileStream = await minioService.DownloadFileAsync(fileName);

            using var reader = new StreamReader(fileStream);
            var content = await reader.ReadToEndAsync();

            return Ok(new DocumentContentResponse { Content = content });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching document content: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}