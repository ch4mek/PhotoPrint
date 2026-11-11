namespace Modsen.Portal.Core.PolicyNames;

public static class PortalPolicyNames
{
    public const string ManageArticlePolicy = "ManageArticlePolicy";
    public const string ViewDashboardPolicy = "ViewDashboard";
    public const string ViewHomePolicy = "ViewHomePolicy";
    public const string ChiefHeadGmEmployeeSysAdminAndViewHomeOnlyPolicy = "ChiefHeadGmEmployeeSysAdminAndViewHomeOnlyPolicy";
    public const string ViewWikiPolicy = "ViewWikiPolicy";
    public const string ManageGroupOrSubGroupPolicy = "ManageGroupOrSubGroupPolicy";
    public const string ManageOrderPolicy = "ManageOrderPolicy";

    public const string ViewDiscountsPolicy = "ViewDiscountsPolicy";
    public const string ManageDiscountCategoryPolicy = "ManageDiscountCategoryPolicy";
    public const string ManageDiscountCardPolicy = "ManageDiscountCardPolicy";
    public const string ManageDiscountOrderPolicy = "ManageDiscountOrderPolicy";

    public const string ViewMerchPolicy = "ViewMerchPolicy";
    public const string ManageTransactionsPolicy = "ManageTransactionsPolicy";
    public const string ManageItemsAndKitsPolicy = "ManageItemsAndKitsPolicy";

    public const string ManageRequestsPolicy = "ManageRequestsPolicy";
    public const string ManageAllRequestsPolicy = "ManageAllRequestsPolicy";
    public const string ViewOrManageRequestsPolicy = "ViewOrManageRequestsPolicy";

    public const string ManageAbsenceSettingsPolicy = "ManageAbsenceSettingsPolicy";
    public const string ManageAssignsPolicy = "ManageAssignsPolicy";

    public const string ManageCvsPolicy = "ManageCvsPolicy";

    public const string ManageAllEquipmentOrChiefOrSysAdminPolicy = "ManageAllEquipmentOrChiefOrSysAdminPolicy";
    public const string ManageAllEquipmentOrChiefOrHeadOrGmOrSysAdminPolicy = "ManageAllEquipmentOrChiefOrHeadOrGmOrSysAdminPolicy";

    public const string ManageInterviewsPolicy = "ManageInterviewsPolicy";
    public const string ViewOrManageProjectExitsPolicy = "ViewOrManageStaffingPolicy";
    public const string ManageProjectExitsPolicy = "ManageProjectExitsPolicy";
    public const string ChiefHeadGmSysAdminOrProjectExitsPolicyName = "ChiefHeadGmSysAdminOrProjectExitsPolicy";

    public const string ChiefHeadHrPolicyName = "ChiefHeadHrPolicy";
    public const string ChiefHeadGmSysAdminHRPolicyName = "ChiefHeadGmSysAdminHRPolicy";
    public const string ChiefHeadGmSysAdminHRCoordinatorPolicyName = "ChiefHeadGmSysAdminHRCoordinatorPolicy";
    public const string ChiefHeadGmHRPolicyName = "ChiefHeadGmHRPolicy";
    public const string ChiefHeadGmHRManageInternsPolicyName = "ChiefHeadGmHRManageInternsPolicyName";
    public const string ChiefAndHRPolicyName = "ChiefAndHRPolicy";

    public const string ChiefHeadGmSysAdminHRAccountantCoordinatorPolicyName = "ChiefHeadGmSysAdminHRAccountantCoordinatorPolicy";
    public const string ChiefHeadGmSysAdminHRAccountantPolicyName = "ChiefHeadGmSysAdminHRAccountantPolicyName";
    public const string ChiefHeadGmHRAccountantPolicyName = "ChiefAndHRAccountantPolicyName";
    public const string ViewExportDiscountsPolicy = "ViewExportDiscountsPolicy";

    //Finance policies
    public const string ManageProjectRatesPolicy = "ManageProjectRatesPolicy";
    public const string ViewProjectRatesPolicy = "ViewProjectRatesPolicy";
    public const string ManageExpensesPolicy = "ManageExpensesPolicy";
    public const string ViewManagementTabPolicy = "ViewManagementTabPolicy";

    public const string ViewLabBoardPolicy = "ViewLabBoardPolicy";
    public const string ViewInternshipPolicy = "ViewInternshipPolicy";

    public static IEnumerable<(string ClaimName, IReadOnlyCollection<string> Values)> RolePolicies()
    {
        yield return (ChiefOnlyPolicyName.Name, ChiefOnlyPolicyName.Values);
        yield return (ChiefAndHeadOnlyPolicyName.Name, ChiefAndHeadOnlyPolicyName.Values);
        yield return (ChiefHeadAndGmOnlyPolicyName.Name, ChiefHeadAndGmOnlyPolicyName.Values);
        yield return (ChiefHeadGmSysAdminOnlyPolicyName.Name, ChiefHeadGmSysAdminOnlyPolicyName.Values);
        yield return (ChiefHeadSysAdminOnlyPolicyName.Name, ChiefHeadSysAdminOnlyPolicyName.Values);
        yield return (ChiefHeadGmEmployeeOnlyPolicyName.Name, ChiefHeadGmEmployeeOnlyPolicyName.Values);
        yield return (HeadGmSysAdminEmployeeOnlyPolicyName.Name, HeadGmSysAdminEmployeeOnlyPolicyName.Values);
        yield return (DefaultPolicyName.Name, DefaultPolicyName.Values);
    }
}
