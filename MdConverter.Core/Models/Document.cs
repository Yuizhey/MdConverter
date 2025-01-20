namespace MdConverter.Core.Models;

public class Document
{
    public const int MAX_NAME_LENGTH = 50;
    public Guid Id { get; }
    public string Name { get;}
    public string UserName { get; }

    private Document(Guid id, string name, string userName)
    {
        Id = id;
        Name = name;
        UserName = userName;
    }

    // public static Result<Document> Create(Guid id, string name, string passwordHash)
    // {
    //     
    // }
}