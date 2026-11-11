using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Requests;

internal sealed class CreateCorrectionAgreementTransactionRequest
    : ICommandHandler<CreateCorrectionAgreementTransactionRequest.Command, int>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IClockService _clockService;

    public CreateCorrectionAgreementTransactionRequest(
        IInquiriesDbContext inquiriesDbContext,
        IClockService clockService)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _clockService = clockService;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var userAgreement = await _inquiriesDbContext.UserAgreements
            .Include(x => x.InquiryTemplateAgreement)
            .Where(x => x.UserId == request.UserId)
            .Where(x => x.InquiryAgreementTemplateId == request.AgreementTemplateId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserAgreementNotFound);

        var transaction = new UserAgreementTransactionEntity
        {
            UserId = userAgreement.UserId,
            TransactionDate = _clockService.UtcNow,
            TransactionType = UserAgreementTransactionType.Accrual,
            Desctiption = request.Description,
            AmountOfDays = request.DaysCount,
            UserAgreementId = null,
            UserInquiryId = null,
            InquiryTemplateId = userAgreement.InquiryTemplateAgreement!.InquiryTemplateId,
            IsCreatedManually = true,
        };

        await _inquiriesDbContext.UserAgreementTransactions.AddAsync(transaction, cancellationToken);
        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.UserId)
                .NotEmpty();

            RuleFor(command => command.AgreementTemplateId)
                .NotEmpty();

            RuleFor(command => command.Description)
                .LongTextType1(false);

            RuleFor(command => command.DaysCount)
                .NotEmpty();
        }
    }

    internal sealed record Command : ICommand<int>, ITransactionalRequest
    {
        public int UserId { get; set; }
        public int AgreementTemplateId { get; set; }
        public decimal DaysCount { get; set; }
        public string? Description { get; set; }
    }
}
