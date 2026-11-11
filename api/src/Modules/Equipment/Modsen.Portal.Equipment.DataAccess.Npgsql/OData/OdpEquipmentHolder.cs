using System.Linq.Expressions;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.OData;

public class OdpEquipmentHolder
{
    public int EquipmentHistoryId { get; set; }
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? FullName { get; set; }
    public string? FullNameRu { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }

    public int? ApproverId { get; set; }
    public string? ApproverFirstName { get; set; }
    public string? ApproverLastName { get; set; }
    public string? ApproverFullName { get; set; }
    public string? ApproverFirstNameRu { get; set; }
    public string? ApproverLastNameRu { get; set; }
    public string? ApproverFullNameRu { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public bool IsDeleted { get; set; }

    internal static readonly Expression<Func<EquipmentAssignEntity, OdpEquipmentHolder>> Map =
        x => new OdpEquipmentHolder
        {
            EquipmentHistoryId = x.Id,
            Id = x.AssignedToUserId,
            FirstName = x.User!.FirstName,
            LastName = x.User.LastName,
            FullName = x.User.LastName + ' ' + x.User.FirstName,
            FirstNameRu = x.User.FirstNameRu,
            LastNameRu = x.User.LastNameRu,

            FullNameRu = x.User.LastNameRu != null && x.User.FirstNameRu != null
                ? x.User.LastNameRu + ' ' + x.User.FirstNameRu
                : null,

            DepartmentId = x.User.DepartmentId,
            ResourceGroupId = x.User.ResourceGroupId,

            ApproverId = x.ApproverId,
            ApproverFirstName = x.Approver != null ? x.Approver.FirstName : null,
            ApproverLastName = x.Approver != null ? x.Approver.LastName : null,
            ApproverFirstNameRu = x.Approver != null ? x.Approver.FirstNameRu : null,
            ApproverLastNameRu = x.Approver != null ? x.Approver.LastNameRu : null,

            ApproverFullName = x.Approver != null
                ? x.Approver.LastName + ' ' + x.Approver.FirstName
                : null,

            ApproverFullNameRu = x.Approver != null && x.Approver.FirstNameRu != null && x.Approver.LastNameRu != null
                ? x.Approver.LastNameRu + ' ' + x.Approver.FirstNameRu
                : null,

            IssueDate = x.IssueDate,
            ReturnDate = x.ReturnDate,
            IsDeleted = x.IsDeleted,
        };
}
