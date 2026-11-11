using Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output
{
    internal class LeaveOutput
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Position { get; set; }
        public string? AvatarFieldId { get; set; }
        public IReadOnlyCollection<LeaveDayOutput> LeaveDays { get; set; } = [];

        public static LeaveOutput Of(LeaveDto leaveDto)
        {
            return new LeaveOutput
            {
                UserId = leaveDto.UserId,
                Name = leaveDto.Name,
                Position = leaveDto.Position,
                AvatarFieldId = leaveDto.AvatarFieldId,
                LeaveDays = leaveDto.LeaveDays.Select(LeaveDayOutput.Of).ToList(),
            };
        }
    }
}
