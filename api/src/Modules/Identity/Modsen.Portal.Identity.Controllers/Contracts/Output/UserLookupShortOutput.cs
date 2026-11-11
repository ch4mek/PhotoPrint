using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class UserLookupShortOutput
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public static UserLookupShortOutput Of(UserLookupDto source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new UserLookupShortOutput
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
        };
    }
}
