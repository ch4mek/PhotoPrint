using Modsen.Portal.Equipment.Requests.Equipment.Core.CoreDto;

namespace Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;

internal class AvailableFilterOptionsDto
{
    public required List<UserFilterCoreDto> Approvers { get; set; } = [];
    public required List<UserFilterCoreDto> Users { get; set; } = [];
}
