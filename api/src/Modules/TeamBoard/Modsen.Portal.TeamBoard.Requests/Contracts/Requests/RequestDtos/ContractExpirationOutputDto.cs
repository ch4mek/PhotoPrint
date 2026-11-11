using Modsen.Portal.Core.Communication.Notifications;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;

public class ContractExpirationOutputDto : INotificationOutput
{
    public int UserId { get; set; }
    public required string UserFirstName { get; set; }
    public required string UserLastName { get; set; }
    public int DaysLeft { get; set; }
}
