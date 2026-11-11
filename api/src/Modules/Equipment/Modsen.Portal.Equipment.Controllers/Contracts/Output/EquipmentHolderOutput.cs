using Modsen.Portal.Core.Utils;
using Modsen.Portal.Equipment.Requests.Dto;

namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

public class EquipmentHolderOutput
{
    public int EquipmentHistoryId { get; set; }
    public int Id { get; set; }
    public required string FullName { get; set; }
    public string? FullNameRu { get; set; }
    public int? ApproverId { get; set; }
    public string? ApproverFullName { get; set; }
    public string? ApproverFullNameRu { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    internal static EquipmentHolderOutput? Of(EquipmentHolderDto? source)
    {
        if (source == null)
        {
            return null;
        }

        var fullNameRu = source.LastNameRu == null && source.FirstNameRu == null
            ? null
            : ToFullName.Reverse(source.FirstNameRu, source.LastNameRu).Trim();
        var approverFullName = source.ApproverId == null
            ? null
            : ToFullName.Reverse(source.ApproverFirstName, source.ApproverLastName);

        return new EquipmentHolderOutput
            {
                EquipmentHistoryId = source.EquipmentHistoryId,
                Id = source.Id,
                FullName = ToFullName.Reverse(source.FirstName, source.LastName),
                FullNameRu = fullNameRu,
                ApproverId = source.ApproverId,
                ApproverFullName = approverFullName,
                ApproverFullNameRu = source.ApproverId == null || (source.ApproverLastNameRu == null && source.ApproverFirstNameRu == null)
                    ? null
                    : ToFullName.Reverse(source.ApproverFirstNameRu, source.ApproverLastNameRu),
                ResourceGroupId = source.ResourceGroupId,
                DepartmentId = source.DepartmentId,
                IssueDate = source.IssueDate,
                ReturnDate = source.ReturnDate,
            };
    }
}
