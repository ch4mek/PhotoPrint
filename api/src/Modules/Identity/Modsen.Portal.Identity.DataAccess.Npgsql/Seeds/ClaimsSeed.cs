using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Seeds
{
    // Seeds unassigned claims
    internal class ClaimsSeed : ISeedEntitiesProvider<IdentityModuleDbContext>
    {
        private readonly IdentityModuleDbContext _dbContext;

        public ClaimsSeed(IdentityModuleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            foreach (var claim in GetClaims())
            {
                CreateIfNotExists(claim);
            }

            _dbContext.SaveChanges();
        }

        private void CreateIfNotExists(ClaimEntity claim)
        {
            var hasAny = _dbContext.Set<ClaimEntity>().Any(x => x.ClaimType == claim.ClaimType);
            if (!hasAny)
            {
                _dbContext.Set<ClaimEntity>().Add(claim);
            }
        }

        private IEnumerable<ClaimEntity> GetClaims()
        {
            yield return Create(
                HrClaimTypeNames.ManageHrModule.Name,
                HrClaimTypeNames.GroupId,
                HrClaimTypeNames.ManageHrModule.OrderWeight);
            yield return Create(
                EquipmentClaimTypeNames.ManageAllEquipment.Name,
                EquipmentClaimTypeNames.GroupId,
                EquipmentClaimTypeNames.ManageAllEquipment.OrderWeight);
            yield return Create(
                AbsencesClaimTypeNames.ViewAbsences.Name,
                AbsencesClaimTypeNames.GroupId,
                AbsencesClaimTypeNames.ViewAbsences.OrderWeight);
            yield return Create(
                DashboardClaimTypeNames.ViewDashboard.Name,
                DashboardClaimTypeNames.GroupId,
                DashboardClaimTypeNames.ViewDashboard.OrderWeight);
            yield return Create(
                RequestsClaimTypeNames.ViewAllRequests.Name,
                RequestsClaimTypeNames.GroupId,
                RequestsClaimTypeNames.ViewAllRequests.OrderWeight);
            yield return Create(
                AccountantClaimTypeNames.ManageAccountant.Name,
                AccountantClaimTypeNames.GroupId,
                AccountantClaimTypeNames.ManageAccountant.OrderWeight);
            yield return Create(
                CoordinatorClaimTypeNames.ManageProjectCoordinator.Name,
                CoordinatorClaimTypeNames.GroupId,
                CoordinatorClaimTypeNames.ManageProjectCoordinator.OrderWeight);
            yield return Create(
                DiscountsClaimTypeNames.ManageDiscountsExport.Name,
                DiscountsClaimTypeNames.GroupId,
                DiscountsClaimTypeNames.ManageDiscountsExport.OrderWeight);
            yield return Create(
                StaffingClaimTypeNames.ViewProjectExits.Name,
                StaffingClaimTypeNames.GroupId,
                StaffingClaimTypeNames.ViewProjectExits.OrderWeight);
            yield return Create(
                StaffingClaimTypeNames.ManageAllProjectExits.Name,
                StaffingClaimTypeNames.GroupId,
                StaffingClaimTypeNames.ManageAllProjectExits.OrderWeight);
            yield return Create(
                FinanceClaimTypeNames.ViewRates.Name,
                FinanceClaimTypeNames.GroupId,
                FinanceClaimTypeNames.ViewRates.OrderWeight);
            yield return Create(
                FinanceClaimTypeNames.ManageRates.Name,
                FinanceClaimTypeNames.GroupId,
                FinanceClaimTypeNames.ManageRates.OrderWeight);
            yield return Create(
                JiraClaimTypeNames.ViewJiraImport.Name,
                JiraClaimTypeNames.GroupId,
                JiraClaimTypeNames.ViewJiraImport.OrderWeight);
            yield return Create(
                LabBoardClaimTypeNames.ManageInterns.Name,
                LabBoardClaimTypeNames.GroupId,
                LabBoardClaimTypeNames.ManageInterns.OrderWeight);
            yield break;

            ClaimEntity Create(
                string claimType,
                string groupName,
                int orderWeight,
                AllowedValueType valueType = AllowedValueType.String)
            {
                return new ClaimEntity
                {
                    ClaimType = claimType,
                    GroupId = GetGroup(groupName)?.Id,
                    OrderWeight = orderWeight,
                    AllowedValueType = valueType
                };
            }
        }

        private ClaimGroup? GetGroup(string? groupName)
        {
            if (groupName == null)
            {
                return null;
            }

            return _dbContext.ClaimGroups.FirstOrDefault(x => x.Id == groupName);
        }
    }
}
