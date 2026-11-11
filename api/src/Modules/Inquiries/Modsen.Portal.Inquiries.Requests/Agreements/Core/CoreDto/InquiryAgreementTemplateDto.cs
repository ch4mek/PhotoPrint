namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

public class InquiryAgreementTemplateDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int DaysPerContractYear { get; set; }
    public bool IsDefault { get; set; }
    public int AssignedUsersCount { get; set; }
    public bool HasAnyTransactions { get; set; }
}
