namespace Modsen.Portal.TimeTrack.Contract.Contract.Input
{
    public class DateRangeDto
    {
        public required DateOnly Start { get; set; }
        public DateOnly? End { get; set; }
    }
}
