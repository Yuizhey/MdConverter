using MdConverter.Core.Models;

namespace MdConverter.Core.Abstractions.Repositories;

public interface IDocumentRepository
{
    Task<List<Document>> GetAllDocuments();
    Task<List<Document>> GetAllDocumentsByUserId(Guid userId);
    Task<List<Document>> GetAllDocumentsByUserName(string userName);
    Task<Document> GetDocumentById(Guid Id);
    Task<Guid> DeleteDocument(Guid id);
    Task<Guid> CreateDocument(Document document);
    Task<Guid> UpdateDocument(Guid id, string name);
}