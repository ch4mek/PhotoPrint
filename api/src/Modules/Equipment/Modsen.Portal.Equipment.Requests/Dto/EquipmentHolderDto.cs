using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.Requests.Dto;

internal class EquipmentHolderDto
{
    public int EquipmentHistoryId { get; set; }
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public int? ApproverId { get; set; }
    public string? ApproverFirstName { get; set; }
    public string? ApproverLastName { get; set; }
    public string? ApproverFirstNameRu { get; set; }
    public string? ApproverLastNameRu { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? DepartmentId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public static EquipmentHolderDto Of(EquipmentAssignEntity source)
    {
        return new EquipmentHolderDto
        {
            EquipmentHistoryId = source.Id,
            Id = source.AssignedToUserId,
            FirstName = source.User?.FirstName ?? string.Empty,
            LastName = source.User?.LastName ?? string.Empty,
            FirstNameRu = source.User?.FirstNameRu,
            LastNameRu = source.User?.LastNameRu,
            ApproverId = source.ApproverId,
            ApproverFirstName = source.Approver?.FirstName,
            ApproverLastName = source.Approver?.LastName,
            ApproverFirstNameRu = source.Approver?.FirstNameRu,
            ApproverLastNameRu = source.Approver?.LastNameRu,
            ResourceGroupId = source.User?.ResourceGroupId,
            DepartmentId = source.User?.DepartmentId,
            IssueDate = source.IssueDate,
            ReturnDate = source.ReturnDate
        };
    }
}
