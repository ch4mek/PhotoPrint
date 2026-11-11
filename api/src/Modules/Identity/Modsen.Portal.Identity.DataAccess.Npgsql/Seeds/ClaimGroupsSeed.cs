using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Seeds
{
    internal class ClaimGroupsSeed : ISeedEntitiesProvider<IdentityModuleDbContext>
    {
        private readonly IdentityModuleDbContext _dbContext;

        public ClaimGroupsSeed(IdentityModuleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            foreach (var claimGroup in GetClaimsGroups())
            {
                CreateIfNotExists(claimGroup);
            }

            _dbContext.SaveChanges();
        }

        private void CreateIfNotExists(ClaimGroup group)
        {
            var hasAny = _dbContext.Set<ClaimGroup>().Any(x => x.Id == group.Id);
            if (!hasAny)
            {
                _dbContext.Set<ClaimGroup>().Add(group);
            }
        }

        private static IEnumerable<ClaimGroup> GetClaimsGroups()
        {
            yield return Create(WikiClaimTypeNames.GroupId, "Wiki");
            yield return Create(EquipmentClaimTypeNames.GroupId, "Equipment");
            yield return Create(IdentityClaimTypeNames.GroupId, "Identity");
            yield return Create(DiscountsClaimTypeNames.GroupId, "Discounts");
            yield return Create(MerchClaimTypeNames.GroupId, "Merch");
            yield return Create(HangfireClaimTypeNames.GroupId, "Hangfire");
            yield return Create(RecruitmentClaimTypeNames.GroupId, "Recruitment");
            yield return Create(DashboardClaimTypeNames.GroupId, "Dashboard");
            yield return Create(HomeClaimTypeNames.GroupId, "Home");
            yield return Create(RequestsClaimTypeNames.GroupId, "Requests");
            yield return Create(AbsencesClaimTypeNames.GroupId, "Absences");
            yield return Create(BookingClaimTypeNames.GroupId, "Booking");
            yield return Create(CvsClaimTypeNames.GroupId, "Cvs");
            yield return Create(MicrofrontendsClaimTypeNames.GroupId, "Microfrontend");
            yield return Create(StaffingClaimTypeNames.GroupId, "Staffing");
            yield return Create(HrClaimTypeNames.GroupId, "HR");
            yield return Create(AccountantClaimTypeNames.GroupId, "Accountant");
            yield return Create(CoordinatorClaimTypeNames.GroupId, "Project Coordinator");
            yield return Create(FinanceClaimTypeNames.GroupId, "P&L");
            yield return Create(JiraClaimTypeNames.GroupId, "Jira");
            yield return Create(LabBoardClaimTypeNames.GroupId, "LabBoard");
            yield break;

            static ClaimGroup Create(string id, string name)
            {
                return new ClaimGroup
                {
                    Id = id,
                    Name = name,
                };
            }
        }
    }
}
