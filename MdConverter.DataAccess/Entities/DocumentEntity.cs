namespace MdConverter.DataAccess.Entities;

public class DocumentEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public UserEntity User { get; set; }
    public Guid UserId { get; set; }
}