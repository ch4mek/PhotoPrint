namespace Modsen.Portal.TimeTrack.UseCases.Dtos;

public class BillableAssignementDate
{
    public DateOnly Date { get; set; }
    public bool IsBillable { get; set; }
}
