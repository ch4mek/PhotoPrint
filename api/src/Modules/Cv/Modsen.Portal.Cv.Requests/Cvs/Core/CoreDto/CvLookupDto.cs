namespace Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

public record CvLookupDto
{
    public int Id { get; set; }
    public required string CvName { get; set; }
}
