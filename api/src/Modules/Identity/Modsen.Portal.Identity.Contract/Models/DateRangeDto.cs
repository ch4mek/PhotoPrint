namespace Modsen.Portal.Identity.Contract.Models
{
    public class DateRangeDto
    {
        public required DateOnly Start { get; set; }
        public DateOnly? End { get; set; }
    }
}
