using MdConverter.Core.Models;

namespace MdConverter.Core.Abstractions.Services;

public interface IDocumentService
{
    Task<List<Document>> GetAllDocuments();
    Task<Document> GetDocumentById(Guid id);
    Task<List<Document>> GetAllDocumentsByUserId(Guid id);
    Task<List<Document>> GetAllDocumentsByUserName(string name);
    Task<Guid> CreateDocument(Document document);
    Task<Guid> UpdateDocument(Guid id, string name);
    Task<Guid> DeleteDocument(Guid id);
}