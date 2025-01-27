using MdConverter.Core.Abstractions.Repositories;
using MdConverter.Core.Models;
using MdConverter.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace MdConverter.DataAccess.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly MdConverterDbContext context;

    public DocumentRepository(MdConverterDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Document>> GetAllDocuments()
    {
        var documentEntities = await context.Documents.AsNoTracking().Include(u => u.User).ToListAsync();
        var documents = documentEntities.Select(d=> Document.Create(d.Id, d.Name, d.User.Name).document).ToList();
        return documents;
    }
    
    public async Task<List<Document>> GetAllDocumentsByUserId(Guid userId)
    {
        var documentEntitys = await context.Documents.AsNoTracking().Include(u => u.User)
            .Where(u => u.UserId == userId).ToListAsync();
        var documents = documentEntitys.Select(d=> Document.Create(d.Id, d.Name, d.User.Name).document).ToList();
        return documents;
    }
    
    public async Task<List<Document>> GetAllDocumentsByUserName(string userName)
    {
        var documentEntitys =await context.Documents.AsNoTracking().Include(u => u.User)
            .Where(u => u.User.Name == userName).ToListAsync();
        var documents = documentEntitys.Select(d=> Document.Create(d.Id, d.Name, d.User.Name).document).ToList();
        return documents;
    }
    
    public async Task<Document> GetDocumentById(Guid Id)
    {
        var documentEntity =await context.Documents.AsNoTracking().Include(u => u.User)
            .FirstOrDefaultAsync(u => u.Id == Id);
        var document = Document.Create(documentEntity.Id, documentEntity.Name, documentEntity.User.Name).document;
        return document;
    }

    public async Task<Guid> DeleteDocument(Guid id)
    {
        var document = await context.Documents
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync();

        if (document != null)
        {
            context.Documents.Remove(document);  // Удаление из контекста
            await context.SaveChangesAsync();    // Применение изменений в базе данных
        }

        return id;
    }

    public async Task<Guid> CreateDocument(Document document)
    {
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Name==document.UserName);
        var documentEntity = new DocumentEntity
        {
            Id = document.Id,
            Name = document.Name,
            UserId = user.Id
        };
        await context.Documents.AddAsync(documentEntity);
        await context.SaveChangesAsync();
        return documentEntity.Id;
    }
    
    public async Task<Guid> UpdateDocument(Guid id, string name)
    {
        var document = await context.Documents.Where(u => u.Id == id).ExecuteUpdateAsync(
            d=>d.SetProperty(i => i.Name, name));
        // await context.SaveChangesAsync();
        return id;
    }
}