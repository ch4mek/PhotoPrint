namespace Modsen.Portal.Inquiries.Requests.InquiryUsers.Core.CoreDto;

public record InquiryUserLookupDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
}
