namespace MdConverter.Api.ResponseModels;

public record UserResponse(Guid id, string name, string passwordHash);