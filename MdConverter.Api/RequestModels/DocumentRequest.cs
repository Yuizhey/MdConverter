using System.ComponentModel.DataAnnotations;

namespace MdConverter.Api.RequestModels;

public record DocumentRequest(
    [Required] string name,
    [Required] string userName);