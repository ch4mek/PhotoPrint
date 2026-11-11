using Modsen.Portal.Equipment.Requests.Equipment.Core.CoreDto;

namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

internal class UserFilterOutput
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }

    public static UserFilterOutput Of(UserFilterCoreDto source)
    {
        return new UserFilterOutput
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FirstNameRu = source.FirstNameRu,
            LastNameRu = source.LastNameRu,
        };
    }
}
