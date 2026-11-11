using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;

namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

internal class AvailableFilterOptionsOutput
{
    public required List<UserFilterOutput> Approvers { get; set; } = [];
    public required List<UserFilterOutput> Users { get; set; } = [];

    public static AvailableFilterOptionsOutput Of(AvailableFilterOptionsDto source)
    {
        return new AvailableFilterOptionsOutput
        {
            Approvers = source.Approvers.Select(UserFilterOutput.Of).ToList(),
            Users = source.Users.Select(UserFilterOutput.Of).ToList(),
        };
    }
}
