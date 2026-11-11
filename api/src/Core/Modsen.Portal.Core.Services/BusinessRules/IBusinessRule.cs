namespace Modsen.Portal.Core.Services.BusinessRules;

public interface IBusinessRule<in TBusinessModel>
{
    Task<bool> IsCorrespondsToBusiness(TBusinessModel? model, CancellationToken cancellationToken);
}

public interface IBusinessRuleWithFailReason<in TBusinessModel>
{
    Task<BusinessRuleResult> IsCorrespondsToBusiness(TBusinessModel? model, CancellationToken cancellationToken);
}
