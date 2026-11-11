using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.Calendar.Requests;

internal sealed class DeleteExclusionRequest : ICommandHandler<DeleteExclusionRequest.Command, int>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public DeleteExclusionRequest(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var exclusion = await _inquiriesDbContext.CalendarExclusions
                .Where(exclusion => exclusion.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken) ??
            throw new EntityNotFoundException($"Calendar exclusion {request.Id} couldn't be found.");

        _inquiriesDbContext.CalendarExclusions.Remove(exclusion);
        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return exclusion.Id;
    }

    internal sealed record Command(int Id) : ICommand<int>, ITransactionalRequest;
}
