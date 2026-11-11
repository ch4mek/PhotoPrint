namespace Modsen.Portal.Identity.Requests.Claims.Requests.RequestDto
{
    public class UserClaimsDto
    {
        public int UserId { get; set; }
        public string? Color { get; set; }
        public string? UserFullName { get; set; }
        public string? UserFullNameRu { get; set; }
        public string? DepartmentId { get; set; }
        public IReadOnlyCollection<ClaimGroupDto> Groups { get; set; } = [];
    }
}
