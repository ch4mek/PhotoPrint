using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Seeds;

internal class RoleClaimsSeed : ISeedClaimProvider<IdentityModuleDbContext, Role>
{
    public RoleClaimsSeed(IdentityModuleDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public IdentityModuleDbContext DbContext { get; }
    public string Scope => nameof(RoleClaim);

    public void Seed(Role entity)
    {
        AddRoleClaim(entity.Id, HomeClaimTypeNames.ViewHome, "any", HomeClaimTypeNames.GroupId);

        if (!entity.Name.IsIntern())
        {
            AddRoleClaim(entity.Id, WikiClaimTypeNames.ViewWiki, "any", WikiClaimTypeNames.GroupId);
            AddRoleClaim(entity.Id, DiscountsClaimTypeNames.ViewDiscounts, "any", DiscountsClaimTypeNames.GroupId);
        }

        CeoCtoClaims(entity);

        HeadClaims(entity);
        GmClaims(entity);
        CtoClaims(entity);
    }

    private void GmClaims(Role entity)
    {
        if (entity.Name == PortalRoleNames.GroupManager)
        {
            var cvGroup = CvsClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, CvsClaimTypeNames.ManageCvs, "any", cvGroup);

            var interviewGroup = StaffingClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, StaffingClaimTypeNames.ManageInterviews, "any", interviewGroup);
        }
    }

    private void HeadClaims(Role entity)
    {
        if (entity.Name == PortalRoleNames.HeadOfDepartment)
        {
            var cvGroup = CvsClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, CvsClaimTypeNames.ManageCvs, "any", cvGroup);

            var interviewGroup = StaffingClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, StaffingClaimTypeNames.ManageInterviews, "any", interviewGroup);
        }
    }

    private void CtoClaims(Role entity)
    {
        if (entity.Name == PortalRoleNames.Cto)
        {
            var recruitmentGroup = RecruitmentClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, RecruitmentClaimTypeNames.AccessToRecruitment, "any", recruitmentGroup);
            AddRoleClaim(entity.Id, RecruitmentClaimTypeNames.CreateAndEditVacancy, "any", recruitmentGroup);
            AddRoleClaim(entity.Id, RecruitmentClaimTypeNames.ManageAllVacancies, "any", recruitmentGroup);
            AddRoleClaim(entity.Id, RecruitmentClaimTypeNames.ManageAllCandidates, "any", recruitmentGroup);
            AddRoleClaim(entity.Id, RecruitmentClaimTypeNames.ManageTags, "any", recruitmentGroup);
            AddRoleClaim(entity.Id, RecruitmentClaimTypeNames.ManageSources, "any", recruitmentGroup);
            AddRoleClaim(entity.Id, RecruitmentClaimTypeNames.ManageRejectionReasons, "any", recruitmentGroup);
            AddRoleClaim(entity.Id, RecruitmentClaimTypeNames.ManageHiringCycles, "any", recruitmentGroup);
            AddRoleClaim(entity.Id, RecruitmentClaimTypeNames.ManageSpecializations, "any", recruitmentGroup);
            AddRoleClaim(entity.Id, RecruitmentClaimTypeNames.ViewAuditLogDetails, "any", recruitmentGroup);
            AddRoleClaim(entity.Id, RecruitmentClaimTypeNames.RecruitmentVacancies, "any", recruitmentGroup);
            AddRoleClaim(entity.Id, RecruitmentClaimTypeNames.InternVacancies, "any", recruitmentGroup);

            var merchGroup = MerchClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, MerchClaimTypeNames.ViewMerch, "any", merchGroup);
            AddRoleClaim(entity.Id, MerchClaimTypeNames.ManageMerchTransactions, "any", merchGroup);
            AddRoleClaim(entity.Id, MerchClaimTypeNames.ManageItemsAndKits, "any", merchGroup);
        }
    }

    private void CeoCtoClaims(Role entity)
    {
        if (entity.Name == PortalRoleNames.Cto ||
                    entity.Name == PortalRoleNames.Ceo)
        {
            var wikiGroup = WikiClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, WikiClaimTypeNames.ManageArticle, "any", wikiGroup);
            AddRoleClaim(entity.Id, WikiClaimTypeNames.ManageGroup, "any", wikiGroup);
            AddRoleClaim(entity.Id, WikiClaimTypeNames.ManageSubgroup, "any", wikiGroup);
            AddRoleClaim(entity.Id, WikiClaimTypeNames.ManageOrder, "any", wikiGroup);
            AddRoleClaim(entity.Id, WikiClaimTypeNames.ManageWikiExport, "any", wikiGroup);

            var discountGroup = DiscountsClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, DiscountsClaimTypeNames.ManageDiscountCategory, "any", discountGroup);
            AddRoleClaim(entity.Id, DiscountsClaimTypeNames.ManageDiscountCard, "any", discountGroup);
            AddRoleClaim(entity.Id, DiscountsClaimTypeNames.ManageDiscountOrder, "any", discountGroup);
            AddRoleClaim(entity.Id, DiscountsClaimTypeNames.ManageDiscountsExport, "any", discountGroup);

            AddRoleClaim(entity.Id, HangfireClaimTypeNames.HangfireDashboardAccess, "any", HangfireClaimTypeNames.GroupId);

            var requestGroup = RequestsClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, RequestsClaimTypeNames.ManageRequests, "any", requestGroup);
            AddRoleClaim(entity.Id, RequestsClaimTypeNames.ManageAllRequests, "any", requestGroup);

            var absenceGroup = AbsencesClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, AbsencesClaimTypeNames.ManageAbsenceSettings, "any", absenceGroup);

            var cvGroup = CvsClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, CvsClaimTypeNames.ManageCvs, "any", cvGroup);

            var interviewGroup = StaffingClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, StaffingClaimTypeNames.ManageAllInterviews, "any", interviewGroup);

            var LabboardGroup = LabBoardClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, LabBoardClaimTypeNames.ManageAllInterns, "any", LabboardGroup);

            var financeGroup = FinanceClaimTypeNames.GroupId;
            AddRoleClaim(entity.Id, FinanceClaimTypeNames.ViewAllRates, "any", financeGroup);
            AddRoleClaim(entity.Id, FinanceClaimTypeNames.ManageAllRates, "any", financeGroup);
            AddRoleClaim(entity.Id, FinanceClaimTypeNames.ViewManagementTab, "any", financeGroup);
            AddRoleClaim(entity.Id, FinanceClaimTypeNames.ManageExpensesTab, "any", financeGroup);
        }
    }

    private void AddRoleClaim(
        int roleId,
        ClaimType claimType,
        string claimValue,
        string? groupName = null,
        AllowedValueType valueType = AllowedValueType.String)
    {
        if (!DbContext.RoleClaims.Any(x => x.RoleId == roleId && x.ClaimType == claimType.Name))
        {
            if (!DbContext.Claims.Any(x => x.ClaimType == claimType.Name))
            {

                DbContext.RoleClaims.Add(new RoleClaim
                {
                    RoleId = roleId,
                    ClaimType = claimType.Name,
                    Claim = new ClaimEntity
                    {
                        ClaimType = claimType.Name,
                        GroupId = GetGroup(groupName)?.Id,
                        OrderWeight = claimType.OrderWeight,
                        AllowedValueType = valueType,
                    },
                    ClaimValue = claimValue,
                });

            }
            else
            {
                DbContext.RoleClaims.Add(new RoleClaim
                {
                    RoleId = roleId,
                    ClaimType = claimType.Name,
                    ClaimValue = claimValue,
                });
            }

            DbContext.SaveChanges();
        }
    }

    private ClaimGroup? GetGroup(string? groupName)
    {
        if (groupName == null)
        {
            return null;
        }
        var group = DbContext.ClaimGroups.FirstOrDefault(x => x.Id == groupName);
        return group!;
    }
}
