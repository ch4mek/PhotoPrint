using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.Requests.Dto;

internal class ApproverDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }

    public static ApproverDto? Of(EquipmentUser? source)
    {
        return source == null
            ? null
            : new ApproverDto
        {
            Id = source.UserId,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FirstNameRu = source.FirstNameRu,
            LastNameRu = source.LastNameRu,
        };
    }
}
