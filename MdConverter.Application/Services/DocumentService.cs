using MdConverter.Core.Abstractions.Repositories;
using MdConverter.Core.Abstractions.Services;
using Document = MdConverter.Core.Models.Document;

namespace MdConverter.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository documentRepository;

    public DocumentService(IDocumentRepository documentRepository)
    {
        this.documentRepository = documentRepository;
    }

    public async Task<List<Document>> GetAllDocuments()
    {
        return await documentRepository.GetAllDocuments();
    }

    public async Task<Document> GetDocumentById(Guid id)
    {
        return await documentRepository.GetDocumentById(id);
    }

    public async Task<List<Document>> GetAllDocumentsByUserId(Guid id)
    {
        return await documentRepository.GetAllDocumentsByUserId(id);
    }
    
    public async Task<List<Document>> GetAllDocumentsByUserName(string name)
    {
        return await documentRepository.GetAllDocumentsByUserName(name);
    }

    public async Task<Guid> CreateDocument(Document document)
    {
        return await documentRepository.CreateDocument(document);
    }
    
    public async Task<Guid> UpdateDocument(Guid id, string name)
    {
        return await documentRepository.UpdateDocument(id, name);
    }
    
    public async Task<Guid> DeleteDocument(Guid id)
    {
        return await documentRepository.DeleteDocument(id);
    }
}