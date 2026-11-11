namespace Modsen.Portal.Inquiries.Controllers.Contracts.Input
{
    internal class UserAssignmentLookupInput
    {
        public int TemplateId { get; set; }
        public int? AgreementId { get; set; }
        public string? FullName { get; set; }
    }
}
