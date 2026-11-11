using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Requests;

internal sealed class UpdateAgreementTemplateRequest : ICommandHandler<UpdateAgreementTemplateRequest.Command, InquiryAgreementTemplateDto>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public UpdateAgreementTemplateRequest(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<InquiryAgreementTemplateDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var agreementTemplate = await _inquiriesDbContext.InquiryTemplateAgreements
            .Where(x => !x.IsDeleted)
            .FirstOrDefaultAsync(agreement => agreement.Id == request.Id, cancellationToken);

        if (agreementTemplate is null)
        {
            throw new EntityNotFoundException("Agreement is not exist");
        }

        var isDaysPerContractYearChanged = agreementTemplate.DaysPerContractYear != request.DaysPerContractYear;
        if (isDaysPerContractYearChanged)
        {
            var hasRequests = await _inquiriesDbContext.UserInquiries
                .AnyAsync(x => x.AgreementTemplateId == agreementTemplate.Id, cancellationToken);

            var hasAgreementTransactions = await _inquiriesDbContext.UserAgreementTransactions
                .AnyAsync(x => x.InquiryTemplateId == agreementTemplate.InquiryTemplateId, cancellationToken);

            if (hasRequests || hasAgreementTransactions)
            {
                throw new ActionNotAllowedException("Can not edit Number of days of accrual per year if agreement has any transactions");
            }
        }

        var isAgreementWithSameNameExist = await _inquiriesDbContext.InquiryTemplateAgreements
        .Where(x => !x.IsDeleted)
        .Where(agreement => agreement.InquiryTemplateId == agreementTemplate.InquiryTemplateId)
        .Where(agreement => agreement.Id != request.Id)
        .Where(agreement => agreement.Name == request.Name)
        .AnyAsync(cancellationToken);

        if (isAgreementWithSameNameExist)
        {
            throw new EntityConflictException("Agreement with the same name already exists");
        }

        if (request.IsDefault)
        {
            await _inquiriesDbContext.InquiryTemplateAgreements
                .Where(x => !x.IsDeleted)
                .Where(agreement => agreement.InquiryTemplateId == agreementTemplate.InquiryTemplateId)
                .Where(agreement => agreement.Id != request.Id)
                .ExecuteUpdateAsync(agreement => agreement
                    .SetProperty(property => property.IsDefault, false), cancellationToken);
        }

        agreementTemplate.Name = request.Name;
        agreementTemplate.DaysPerContractYear = request.DaysPerContractYear;
        agreementTemplate.IsDefault = request.IsDefault;

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return new InquiryAgreementTemplateDto
        {
            Id = request.Id,
            Name = request.Name,
            DaysPerContractYear = request.DaysPerContractYear,
            IsDefault = request.IsDefault
        };
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.Name)
                .ShortTextType();

            RuleFor(command => command.DaysPerContractYear)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(9999);
        }
    }

    internal sealed record Command : ICommand<InquiryAgreementTemplateDto>, ITransactionalRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int DaysPerContractYear { get; set; }
        public bool IsDefault { get; set; }
    }
}
