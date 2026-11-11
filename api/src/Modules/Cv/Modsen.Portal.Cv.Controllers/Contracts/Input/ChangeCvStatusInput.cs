using Modsen.Portal.Core.Cv;

namespace Modsen.Portal.Cv.Controllers.Contracts.Input
{
    internal class ChangeCvStatusInput
    {
        public string? ClientId { get; set; }
        public string? ProjectId { get; set; }
        public string? ClientName { get; set; }
        public string? ProjectName { get; set; }
        public CvStatus Status { get; set; }
    }
}
