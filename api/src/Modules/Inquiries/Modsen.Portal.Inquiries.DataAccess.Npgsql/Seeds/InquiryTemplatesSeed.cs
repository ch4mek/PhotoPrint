using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Seeds;

internal class InquiryTemplatesSeed : ISeedEntitiesProvider<InquiriesDbContext>
{
    private const string HeadOfPmoEmail = "aliaksandr.yakavets@modsen-software.com";

    private readonly InquiriesDbContext _dbContext;

    private int? _headOfPmoId;
    private int? HeadOfPmoId
    {
        get
        {
            _headOfPmoId ??= _dbContext.InquiriesUsers.FirstOrDefault(x => x.Email == HeadOfPmoEmail)?.UserId;

            return _headOfPmoId;
        }
    }

    public InquiryTemplatesSeed(InquiriesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static InquiryTemplateEntity InquiryTemplate(string name,
        InquiryType type,
        TrackingUnit trackingUnit,
        ActivityType activityType,
        List<InquiryTemplateAcceptPolicyEntity> policies,
        List<InquiryTemplateAgreementEntity> agreementTemplates,
        bool prohibitProbation,
        bool attachments,
        bool holidays,
        bool weekends,
        AccrualType? accrualType = null,
        TransferType? transferType = null)
    {
        return new InquiryTemplateEntity
        {
            Name = name,
            InquiryType = type,
            IsActive = true,
            IsDefault = true,
            ActivityType = activityType,
            AccrualType = accrualType,
            IsStaked = transferType == null
                ? null
                : transferType == TransferType.Transferred,
            TrackingUnitType = trackingUnit,
            ProhibitOnProbationPeriod = prohibitProbation,
            RequireAttachments = attachments,
            IncludeHolidays = holidays,
            IncludeWeekends = weekends,
            InquiryTemplateAgreements = agreementTemplates,
            InquiryTemplateAcceptPolicyPolicies = policies,
        };
    }

    private static InquiryTemplateEntity DocumentDefaultTemplate(string name)
    {
        return InquiryTemplate(name,
            InquiryType.Documents,
            TrackingUnit.Days,
            ActivityType.WorkingPaid,
            policies: [new()
                {
                    OrderWeight = 1,
                    Scope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO,
                }],
            prohibitProbation: false,
            attachments: false,
            holidays: false,
            agreementTemplates: [],
            weekends: false);
    }

    private List<InquiryTemplateAcceptPolicyEntity> DefaultPolicy(params int?[] userIds)
    {
        var policies = new List<InquiryTemplateAcceptPolicyEntity>()
        {
            new()
            {
                OrderWeight = 1,
                Scope = InquiryAcceptPolicyScopes.ProjectManager,
                SubstituteScope = InquiryAcceptPolicyScopes.User,
                SubstituteUserId = HeadOfPmoId
            },
            new()
            {
                OrderWeight = 2,
                Scope = InquiryAcceptPolicyScopes.GroupManager,
                SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
            },
            new()
            {
                OrderWeight = 3,
                Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                SubstituteScope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO,
            },
            new()
            {
                OrderWeight = 4,
                Scope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO,
            }
        };

        var i = 5;

        foreach (var userId in userIds)
        {
            if (userId != null)
            {
                policies.Add(new()
                {
                    OrderWeight = i++,
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = userId,
                });
            }
        }

        return policies;
    }

    private static InquiryTemplateAgreementEntity VacationAgreement25()
    {
        return new InquiryTemplateAgreementEntity
        {
            Name = "25 days",
            Type = AgreementType.ContractVacationAgreement,
            DaysPerContractYear = 25,
            IsDefault = true
        };
    }

    private static InquiryTemplateAgreementEntity VacationAgreement31()
    {
        return new InquiryTemplateAgreementEntity
        {
            Name = "31 days",
            Type = AgreementType.ContractVacationAgreement,
            DaysPerContractYear = 31,
            IsDefault = false
        };
    }

    private static InquiryTemplateAgreementEntity SickDayAgreement()
    {
        return new InquiryTemplateAgreementEntity
        {
            Name = "5 days",
            Type = AgreementType.SickDaysAgreement,
            DaysPerContractYear = 5,
            IsDefault = true
        };
    }

    private InquiryTemplateEntity[] InquiryTemplates()
    {
        var hannaBritId = _dbContext.InquiriesUsers
            .FirstOrDefault(x => x.Email == "hanna.brit@modsen-software.com")?.UserId;
        var volhaBuberId = _dbContext.InquiriesUsers
            .FirstOrDefault(x => x.Email == "volha.buber@modsen-software.com")?.UserId;
        var yuliyaLapoId = _dbContext.InquiriesUsers
            .FirstOrDefault(x => x.Email == "yuliya.lapo@modsen-software.com")?.UserId;
        var tatianaGvardeitsevaId = _dbContext.InquiriesUsers
            .FirstOrDefault(x => x.Email == "tatiana.gvardeitseva@modsen-software.com")?.UserId;

        return
        [
            InquiryTemplate("Social vacation",
            InquiryType.SocialVacation,
            TrackingUnit.Days,
            ActivityType.NonWorkingPaid,
            DefaultPolicy(tatianaGvardeitsevaId),
            prohibitProbation: false,
            attachments: false,
            holidays: false,
            agreementTemplates: [],
            weekends: false),

            InquiryTemplate("Military enlistment office",
            InquiryType.MilitaryEnlistmentOffice,
            TrackingUnit.Hours,
            ActivityType.NonWorkingPaid,
            DefaultPolicy(yuliyaLapoId, hannaBritId),
            prohibitProbation: false,
            attachments: true,
            holidays: false,
            agreementTemplates: [],
            weekends: false),

            InquiryTemplate("Weekend transfer",
            InquiryType.WeekendTransfer,
            TrackingUnit.Days,
            ActivityType.NonWorkingPaid,
            DefaultPolicy(),
            prohibitProbation: false,
            attachments: false,
            holidays: false,
            agreementTemplates: [],
            weekends: false),

            InquiryTemplate("Home office",
            InquiryType.HomeOffice,
            TrackingUnit.Hours,
            ActivityType.WorkingPaid,
            DefaultPolicy(yuliyaLapoId),
            prohibitProbation: false,
            attachments: false,
            holidays: false,
            agreementTemplates: [],
            weekends: false),

            InquiryTemplate("Own expense",
            InquiryType.OwnExpense,
            TrackingUnit.Days,
            ActivityType.NonWorkingUnpaid,
            DefaultPolicy(yuliyaLapoId),
            prohibitProbation: false,
            attachments: false,
            holidays: false,
            agreementTemplates: [],
            weekends: false),

            InquiryTemplate("Sick leave",
            InquiryType.SickLeave,
            TrackingUnit.Days,
            ActivityType.NonWorkingPaid,
            DefaultPolicy(volhaBuberId, tatianaGvardeitsevaId, hannaBritId),
            prohibitProbation: false,
            attachments: true,
            holidays: false,
            agreementTemplates: [],
            weekends: false),

            InquiryTemplate("Sick day",
            InquiryType.SickDay,
            TrackingUnit.Days,
            ActivityType.NonWorkingPaid,
            DefaultPolicy(yuliyaLapoId),
            prohibitProbation: true,
            attachments: false,
            holidays: false,
            agreementTemplates: [SickDayAgreement()],
            weekends: false,
            accrualType: AccrualType.OnceInYear,
            transferType: TransferType.Reset),

            InquiryTemplate("Vacation",
            InquiryType.Vacation,
            TrackingUnit.Days,
            ActivityType.NonWorkingPaid,
            DefaultPolicy(yuliyaLapoId),
            prohibitProbation: true,
            attachments: false,
            holidays: false,
            agreementTemplates: [VacationAgreement31(), VacationAgreement25()],
            weekends: true,
            accrualType: AccrualType.ProportianallyEachMonth,
            transferType: TransferType.Transferred),

            DocumentDefaultTemplate("Sport benefit"),
            DocumentDefaultTemplate("Employment certificate"),
            DocumentDefaultTemplate("Reference letter"),
            DocumentDefaultTemplate("Copy of employment record book"),
            DocumentDefaultTemplate("Request for flexible university attendance"),
            DocumentDefaultTemplate("Certificate of salary"),
            DocumentDefaultTemplate("Other"),
        ];
    }

    private void CreateIfNotExists(InquiryTemplateEntity template)
    {
        var hasAny = _dbContext.InquiryTemplates.Any(x => x.Name == template.Name);

        if (!hasAny)
        {
            _dbContext.InquiryTemplates.Add(template);
        }
    }

    public void Seed()
    {
        var templates = InquiryTemplates();
        foreach (var entity in templates)
        {
            CreateIfNotExists(entity);
        }
        _dbContext.SaveChanges();
    }
}
