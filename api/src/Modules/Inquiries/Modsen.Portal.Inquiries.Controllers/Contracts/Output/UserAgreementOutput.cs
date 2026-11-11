using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal class UserAgreementOutput
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public bool IsActive { get; init; }

    public AgreementType Type { get; init; }
    public AccrualType AccrualType { get; init; }
    public DateOnly DateFrom { get; init; }
    public DateOnly DateTo { get; init; }
    public DateOnly? FreezeUntilDate { get; init; }
    public int DaysPerContractYear { get; init; }

    public bool IsStaked { get; set; }
    public bool AllowNegativeBalanceInDays { get; set; }
    public int? NegativeBalanceInDaysMax { get; set; }

    public static UserAgreementOutput Of(UserAgreementCoreDto dto)
    {
        return new UserAgreementOutput
        {
            Id = dto.Id,
            UserId = dto.UserId,
            IsActive = dto.IsActive,
            Type = dto.Type,
            AccrualType = dto.AccrualType,
            DateFrom = dto.DateFrom,
            DateTo = dto.DateTo,
            FreezeUntilDate = dto.FreezeUntilDate,
            DaysPerContractYear = dto.DaysPerContractYear,
            IsStaked = dto.IsStaked,
            AllowNegativeBalanceInDays = dto.AllowNegativeBalanceInDays,
            NegativeBalanceInDaysMax = dto.NegativeBalanceInDaysMax
        };
    }
}
