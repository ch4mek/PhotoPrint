namespace Modsen.Portal.Core.Services.BusinessRules;

public sealed class BusinessRuleResult
{
    public bool IsSuccess { get; }
    public string? Reason { get; }

    private BusinessRuleResult(bool isSuccess, string? reason)
    {
        IsSuccess = isSuccess;
        Reason = reason;
    }

    public static BusinessRuleResult Success()
    {
        return new BusinessRuleResult(true, null);
    }

    public static BusinessRuleResult Fail(string reason)
    {
        return new BusinessRuleResult(false, reason);
    }
}
