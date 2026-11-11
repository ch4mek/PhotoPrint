namespace Modsen.Portal.Identity.Contract.Models
{
    public class ClaimDto
    {
        public required string ClaimType { get; set; }
        public required string ClaimValue { get; set; }
    }
}
