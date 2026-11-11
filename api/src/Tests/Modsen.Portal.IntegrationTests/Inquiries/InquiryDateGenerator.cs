namespace Modsen.Portal.IntegrationTests.Inquiries;

internal static class InquiryDateGenerator
{
    internal static DateOnly InquiryDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    internal static DateOnly GetDateWithAddDays(int days)
    {
        InquiryDate = InquiryDate.AddDays(days);
        return InquiryDate;
    }

    internal static DateOnly GetDateWithAddOneDay()
    {
        return GetDateWithAddDays(1);
    }
}
