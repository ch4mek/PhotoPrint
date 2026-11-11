namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.BusinessRules;

internal interface IIsUserAssignedBusinessRule
{
    bool IsUserAssigned(DateOnly userOnProjectFrom, DateOnly? userOnProjectTo, int month, int year);
}

internal sealed class IsUserAssignedBusinessRule : IIsUserAssignedBusinessRule
{
    public bool IsUserAssigned(DateOnly userOnProjectFrom, DateOnly? userOnProjectTo, int month, int year)
    {
        var maxMonthDay = DateTime.DaysInMonth(year, month);
        var minMonthDateDay = maxMonthDay < userOnProjectFrom.Day ? maxMonthDay : userOnProjectFrom.Day;
        var minMonthDate = new DateOnly(year, month, minMonthDateDay);

        return (userOnProjectFrom <= minMonthDate &&
               (userOnProjectTo is null || userOnProjectTo.Value >= minMonthDate)) ||
               (month == userOnProjectFrom.Month && userOnProjectFrom.Year == year) ||
               (userOnProjectTo is not null && month == userOnProjectTo.Value.Month
                    && userOnProjectTo.Value.Year == year);
    }
}
