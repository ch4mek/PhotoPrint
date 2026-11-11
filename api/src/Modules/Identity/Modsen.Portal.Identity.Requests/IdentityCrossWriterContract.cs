using MediatR;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Identity.Requests.Users.Requests;

namespace Modsen.Portal.Identity.Requests
{
    internal class IdentityCrossWriterContract : IIdentityCrossWriterContract
    {
        private readonly ISender _sender;

        public IdentityCrossWriterContract(ISender sender)
        {
            _sender = sender;
        }

        public async Task<int> UpdateUserInternal(CrossWriterUserDto user,
            CancellationToken cancellationToken)
        {
            var command = new UpdateUserGeneralInfoRequest.InternalCommand(
                user.Id, user.FirstName, user.LastName, user.FirstNameRu, user.LastNameRu, user.PatronymicRu,
                user.Email, user.DepartmentId, user.ResourceGroupId, user.Position);

            var response = await _sender.Send(command, cancellationToken);

            return response.Id;
        }
    }
}
