using Modsen.Portal.Identity.Requests.Dto;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class UserSignatureOutput
{
    public string? FullName { get; set; }
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    internal static UserSignatureOutput Of(UserSignatureDto source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new UserSignatureOutput
        {
            FullName = source.FullName,
            Position = source.Position,
            Phone = source.Phone,
            Email = source.Email,
        };
    }
}

