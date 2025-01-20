using System.ComponentModel.DataAnnotations;

namespace MdConverter.Api.RequestModels;

public record UserRequest(
    [Required]string name, 
    [Required]string password);