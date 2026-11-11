using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Requests;

internal sealed class DeleteAgreementTemplateRequest : ICommandHandler<DeleteAgreementTemplateRequest.Command, int>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IClockService _clockService;

    public DeleteAgreementTemplateRequest(
        IInquiriesDbContext inquiriesDbContext,
        IClockService clockService)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _clockService = clockService;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var agreementTemplate = await _inquiriesDbContext.InquiryTemplateAgreements
            .Where(x => !x.IsDeleted)
            .Include(agreement => agreement.UserAgreements)
                .ThenInclude(x => x.User)
            .Where(agreement => agreement.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.AgreementTemplateNotFound);

        var activeAgreements = agreementTemplate.UserAgreements
            .Where(x => !x.IsDeleted)
            .Where(x => _clockService.TodayUtc() <= x.DateTo || x.DateTo == null)
            .Where(x => x.User!.IsActive);

        if (activeAgreements.Any())
        {
            throw new InvalidDataAppException("You cannot delete agreement that is assigned to the user");
        }

        agreementTemplate.IsDeleted = true;

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return request.Id;
    }

    internal sealed record Command(int Id) : ICommand<int>, ITransactionalRequest;
}
