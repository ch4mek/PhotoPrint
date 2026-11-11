namespace Modsen.Portal.Core.Identity;

public static class RecruitmentClaimTypeNames
{
    public const string GroupId = "RECRUITMENT";

    public static readonly ClaimType AccessToRecruitment = new("AccessToRecruitment", 100);
    public static readonly ClaimType CreateAndEditVacancy = new("CreateAndEditVacancy", 200);
    public static readonly ClaimType ManageAllVacancies = new("ManageAllVacancies", 300);
    public static readonly ClaimType ManageAllCandidates = new("ManageAllCandidates", 400);
    public static readonly ClaimType ManageTags = new("ManageTags", 500);
    public static readonly ClaimType ManageSources = new("ManageSources", 600);
    public static readonly ClaimType ManageRejectionReasons = new("ManageRejectionReasons", 700);
    public static readonly ClaimType ManageHiringCycles = new("ManageHiringCycles", 800);
    public static readonly ClaimType ManageSpecializations = new("ManageSpecializations", 900);
    public static readonly ClaimType ViewAuditLogDetails = new("ViewAuditLogDetails", 1000);
    public static readonly ClaimType RecruitmentVacancies = new("RecruitmentVacancies", 1100);
    public static readonly ClaimType InternVacancies = new("InternVacancies", 1200);
}
