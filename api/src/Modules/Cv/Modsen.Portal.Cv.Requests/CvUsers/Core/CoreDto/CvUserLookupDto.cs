namespace Modsen.Portal.Cv.Requests.CvUsers.Core.CoreDto;

public record CvUserLookupDto
{
    public int Id { get; set; }
    public required string FullName  { get; set; }
    public string? FullNameRu  { get; set; }
}
