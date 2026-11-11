using Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class ClientStatusOutput
{
    public required bool IsSuccessful { get; set; }

    internal static ClientStatusOutput Of(ClientStatusDto source)
    {
        return new ClientStatusOutput
        {
            IsSuccessful = source.IsSuccessful
        };
    }
}
