namespace Modsen.Portal.Booking.Requests.Dto;

internal class BookingDto
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public bool IsForGuest { get; set; }
    public string? GuestName { get; set; }
    public string? BookedBy { get; set; }
    public List<DateTime> DateList { get; set; } = [];
    public DateTime From { get; set; }
    public bool IsBlock { get; set; }
    public int WorkplaceId { get; set; }
    public int WorkplaceOfficeId { get; set; }
    public int WorkplaceNumber { get; set; }
    public int? WorkspaceId { get; set; }
    public int WeeksInInterval { get; set; }

    public static BookingDto OfDatabaseEntity(DataAccess.Npgsql.DatabaseEntities.BookingEntry source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new BookingDto
        {
            BookingId = source.Id,
            UserId = source.UserId,
            FirstName = source.User?.FirstName ?? string.Empty,
            LastName = source.User?.LastName ?? string.Empty,
            FirstNameRu = source.User?.FirstNameRu,
            LastNameRu = source.User?.LastNameRu,
            IsForGuest = source.IsForGuest,
            GuestName = source.GuestName,
            BookedBy = source.CreatedByUser?.LastName + " " + source.CreatedByUser?.FirstName,
            DateList = source.Reservations
                .Where(r => r.BookingEntryId.Equals(source.Id))
                .Select(r => r.ReservationDate)
                .ToList(),
            IsBlock = source.IsBlock,
            WorkplaceId = source.WorkplaceId,
            WorkplaceOfficeId = source.Workplace?.OfficeId ?? 0,
            WorkplaceNumber = source.Workplace?.WorkplaceNumber ?? 0,
            WorkspaceId = source.Workplace?.WorkspaceId,
            WeeksInInterval = source.WeeksInInterval,
        };
    }
}
