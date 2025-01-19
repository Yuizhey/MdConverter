namespace MdConverter.Core.Models;

public class Document
{
    public const int MAX_PARAMETER_LENGTH = 50;
    public Guid Id { get; }
    public string Name { get;}
    public string UserName { get; }
}