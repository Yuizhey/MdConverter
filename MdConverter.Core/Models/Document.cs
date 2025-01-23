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

    public static (Document document, string error) Create(Guid id, string name, string userName)
    {
        var error = string.Empty;
        if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_LENGTH)
        {
            error = "Name must be between 1 and 50 characters";
        }
        var document = new Document(id, name, userName);
        return (document, error);
    }
}