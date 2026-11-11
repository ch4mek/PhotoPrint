using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Requests;

internal sealed class CreateAgreementTemplateRequest : ICommandHandler<CreateAgreementTemplateRequest.Command, int>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public CreateAgreementTemplateRequest(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var inquiryTemplate = await _inquiriesDbContext.InquiryTemplates
            .FirstOrDefaultAsync(template => template.Id == request.InquiryTemplateId, cancellationToken)
            ?? throw new EntityNotFoundException("Inquiry template is not exist");

        if (!UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(inquiryTemplate.InquiryType))
        {
            throw new InvalidDataAppException("Adding agreements is available only for inquiry types such as Vacation and Sick day");
        }

        if (request.IsDefault)
        {
            await _inquiriesDbContext.InquiryTemplateAgreements
                .Where(x => !x.IsDeleted)
                .Where(agreement => agreement.InquiryTemplateId == request.InquiryTemplateId)
                .ExecuteUpdateAsync(agreement => agreement
                    .SetProperty(property => property.IsDefault, false), cancellationToken);
        }

        var isAgreementWithSameNameExist = await _inquiriesDbContext.InquiryTemplateAgreements
                .Where(x => !x.IsDeleted)
                .Where(agreement => agreement.InquiryTemplateId == request.InquiryTemplateId)
                .Where(agreement => agreement.Name == request.Name)
                .AnyAsync(cancellationToken);

        if (isAgreementWithSameNameExist)
        {
            throw new EntityConflictException("Agreement with the same name already exists");
        }

        var agreementType = inquiryTemplate.InquiryType switch
        {
            InquiryType.Vacation => AgreementType.ContractVacationAgreement,
            InquiryType.SickDay => AgreementType.SickDaysAgreement,
            _ => AgreementType.Unknown
        };

        var entity = new InquiryTemplateAgreementEntity
        {
            Name = request.Name,
            DaysPerContractYear = request.DaysPerContractYear,
            IsDefault = request.IsDefault,
            InquiryTemplateId = request.InquiryTemplateId,
            Type = agreementType
        };

        _inquiriesDbContext.InquiryTemplateAgreements.Add(entity);

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
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

    internal sealed record Command : ICommand<int>, ITransactionalRequest
    {
        public string? Name { get; set; }
        public int DaysPerContractYear { get; set; }
        public bool IsDefault { get; set; }
        public int InquiryTemplateId { get; set; }
    }
}
