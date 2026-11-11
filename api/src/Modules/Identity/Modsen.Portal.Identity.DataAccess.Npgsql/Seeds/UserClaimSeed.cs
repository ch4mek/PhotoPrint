using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Seeds
{
    internal class UserClaimSeed : ISeedClaimProvider<IdentityModuleDbContext, User>
    {
        public UserClaimSeed(IdentityModuleDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IdentityModuleDbContext DbContext { get; }
        public string Scope => nameof(UserClaim);

        public void Seed(User entity)
        {
            if(entity.UserRoles.Any(e=> e.Role!.Name == PortalRoleNames.Ceo)
                || entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Cto))
            {
                var wikiGroup = WikiClaimTypeNames.GroupId;
                AddUserClaim(entity.Id, WikiClaimTypeNames.ViewWiki, "any", wikiGroup);
                AddUserClaim(entity.Id, WikiClaimTypeNames.ManageArticle, "any",  wikiGroup);
                AddUserClaim(entity.Id, WikiClaimTypeNames.ManageGroup, "any", wikiGroup);
                AddUserClaim(entity.Id, WikiClaimTypeNames.ManageSubgroup, "any", wikiGroup);
                AddUserClaim(entity.Id, WikiClaimTypeNames.ManageOrder, "any", wikiGroup);
                AddUserClaim(entity.Id, WikiClaimTypeNames.ManageWikiExport, "any", wikiGroup);
            }

            if(entity.UserRoles.Any(e=> e.Role!.Name == PortalRoleNames.Ceo)
                || entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Cto))
            {
                var discountGroup = DiscountsClaimTypeNames.GroupId;
                AddUserClaim(entity.Id, DiscountsClaimTypeNames.ViewDiscounts, "any", discountGroup);
                AddUserClaim(entity.Id, DiscountsClaimTypeNames.ManageDiscountCategory, "any", discountGroup);
                AddUserClaim(entity.Id, DiscountsClaimTypeNames.ManageDiscountCard, "any", discountGroup);
                AddUserClaim(entity.Id, DiscountsClaimTypeNames.ManageDiscountOrder, "any", discountGroup);
            }

            if (entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Ceo)
                || entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Cto))
            {
                var absenceGroup = AbsencesClaimTypeNames.GroupId;
                AddUserClaim(entity.Id, AbsencesClaimTypeNames.ManageAbsenceSettings, "any", absenceGroup);
            }

            if (entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Ceo)
                || entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Cto))
            {
                var requestGroup = RequestsClaimTypeNames.GroupId;
                AddUserClaim(entity.Id, RequestsClaimTypeNames.ManageRequests, "any", requestGroup);
            }

            if (entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Ceo)
                || entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Cto))
            {
                var cvGroup = CvsClaimTypeNames.GroupId;
                AddUserClaim(entity.Id, CvsClaimTypeNames.ManageCvs, "any", cvGroup);
            }

            if (entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Ceo)
                || entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Cto))
            {
                var LabboardGroup = LabBoardClaimTypeNames.GroupId;
                AddUserClaim(entity.Id, LabBoardClaimTypeNames.ManageAllInterns, "any", LabboardGroup);
            }

            if (entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Ceo)
               || entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Cto)
               || entity.Email == "vadim.prokopchuk@modsen-software.com")
            {
                AddUserClaim(entity.Id, HangfireClaimTypeNames.HangfireDashboardAccess, "any", HangfireClaimTypeNames.GroupId);
            }

            CtoClaims(entity);
            InquiriesClaims(entity);
        }

        // Users approving inquiry requests
        private void InquiriesClaims(User entity)
        {
            if (entity.Email == "yuliya.lapo@modsen-software.com" ||
                entity.Email == "volha.buber@modsen-software.com" ||
                entity.Email == "hanna.brit@modsen-software.com" ||
                entity.Email == "tatiana.gvardeitseva@modsen-software.com")
            {
                AddUserClaim(entity.Id, RequestsClaimTypeNames.ManageRequests, "any", RequestsClaimTypeNames.GroupId);
            }
        }

        private void CtoClaims(User entity)
        {
            if (entity.UserRoles.Any(e => e.Role!.Name == PortalRoleNames.Cto))
            {
                AddUserClaim(entity.Id, CvsClaimTypeNames.ViewCvs, "any", CvsClaimTypeNames.GroupId);
                AddUserClaim(entity.Id, MicrofrontendsClaimTypeNames.ManageMfe, "any", MicrofrontendsClaimTypeNames.GroupId);

                var recruitmentGroup = RecruitmentClaimTypeNames.GroupId;
                AddUserClaim(entity.Id, RecruitmentClaimTypeNames.AccessToRecruitment, "any", recruitmentGroup);
                AddUserClaim(entity.Id, RecruitmentClaimTypeNames.CreateAndEditVacancy, "any", recruitmentGroup);
                AddUserClaim(entity.Id, RecruitmentClaimTypeNames.ManageAllVacancies, "any", recruitmentGroup);
                AddUserClaim(entity.Id, RecruitmentClaimTypeNames.ManageAllCandidates, "any", recruitmentGroup);
                AddUserClaim(entity.Id, RecruitmentClaimTypeNames.ManageTags, "any", recruitmentGroup);
                AddUserClaim(entity.Id, RecruitmentClaimTypeNames.ManageSources, "any", recruitmentGroup);
                AddUserClaim(entity.Id, RecruitmentClaimTypeNames.ManageRejectionReasons, "any", recruitmentGroup);
                AddUserClaim(entity.Id, RecruitmentClaimTypeNames.ManageHiringCycles, "any", recruitmentGroup);
                AddUserClaim(entity.Id, RecruitmentClaimTypeNames.ManageSpecializations, "any", recruitmentGroup);
                AddUserClaim(entity.Id, RecruitmentClaimTypeNames.ViewAuditLogDetails, "any", recruitmentGroup);
                AddUserClaim(entity.Id, RecruitmentClaimTypeNames.RecruitmentVacancies, "any", recruitmentGroup);
                AddUserClaim(entity.Id, RecruitmentClaimTypeNames.InternVacancies, "any", recruitmentGroup);

                var merchGroup = MerchClaimTypeNames.GroupId;
                AddUserClaim(entity.Id, MerchClaimTypeNames.ViewMerch, "any", merchGroup);
                AddUserClaim(entity.Id, MerchClaimTypeNames.ManageMerchTransactions, "any", merchGroup);
                AddUserClaim(entity.Id, MerchClaimTypeNames.ManageItemsAndKits, "any", merchGroup);
            }
        }

        private void AddUserClaim(int userId,
            ClaimType claimType,
            string claimValue,
            string? groupName = null,
            AllowedValueType valueType = AllowedValueType.String)
        {
            if (!DbContext.UserClaims.Any(x => x.UserId == userId && x.ClaimType == claimType.Name))
            {
                if (!DbContext.Claims.Any(x => x.ClaimType == claimType.Name))
                {
                    DbContext.UserClaims.Add(new UserClaim
                    {
                        UserId = userId,
                        ClaimType = claimType.Name,
                        Claim = new ClaimEntity
                        {
                            ClaimType = claimType.Name,
                            GroupId = GetGroup(groupName)?.Id,
                            OrderWeight = claimType.OrderWeight,
                            AllowedValueType = valueType
                        },
                        ClaimValue = claimValue,
                    });

                }
                else
                {
                    DbContext.UserClaims.Add(new UserClaim
                    {
                        UserId = userId,
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

            return DbContext.ClaimGroups.FirstOrDefault(x => x.Id == groupName);
        }
    }
}
