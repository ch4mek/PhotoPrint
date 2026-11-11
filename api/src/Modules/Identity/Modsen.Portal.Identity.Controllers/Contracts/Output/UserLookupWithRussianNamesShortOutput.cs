using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class UserLookupWithRussianNamesShortOutput
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }

    public static UserLookupWithRussianNamesShortOutput Of(UserLookupDto source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new UserLookupWithRussianNamesShortOutput
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FirstNameRu = source.FirstNameRu,
            LastNameRu = source.LastNameRu,
        };
    }
}
