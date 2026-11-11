namespace Modsen.Portal.Identity.Requests.Users.Core.CoreDto
{
    internal class UserLookupLoaderRequestDto
    {
        public string? KeyWord { get; set; }
        public bool? IsActive { get; set; }
        public string? DepartmentId { get; set; }
        public int? ResourceGroupId { get; set; }
        public IReadOnlyCollection<string> Roles { get; set; } = [];
        public IReadOnlyCollection<int>? UserIds { get; set; }
        public IReadOnlyCollection<int>? ExcludedUsers { get; set; }
    }
}
