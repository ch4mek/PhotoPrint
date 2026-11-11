namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto
{
    public class UserDepartmentHistoryCoreDto
    {
        public required string DepartmentId { get; set; }
        public DateOnly DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
    }
}
