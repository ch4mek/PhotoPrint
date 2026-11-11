using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Seeds
{
    internal class SeniorityLevelsSeed : ISeedEntitiesProvider<TeamBoardDbContext>
    {
        private readonly TeamBoardDbContext _dbContext;

        public SeniorityLevelsSeed(TeamBoardDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            foreach (var seniorityLevel in GetSeniorityLevels())
            {
                if (!_dbContext.SeniorityLevels.Any(x => x.Id == seniorityLevel.Id))
                {
                    _dbContext.SeniorityLevels.Add(seniorityLevel);
                }
            }

            _dbContext.SaveChanges();
        }

        private static IEnumerable<SeniorityLevel> GetSeniorityLevels()
        {
            var extendedSeniorityLevelScope = SeniorityLevelScopes.HardSkill
                | SeniorityLevelScopes.UserSpecialization
                | SeniorityLevelScopes.ProjectUser
                | SeniorityLevelScopes.TeamBoardUser;

            var generalSeniorityLevelScope = SeniorityLevelScopes.GeneralSeniority;

            var seniorityLevelScope = SeniorityLevelScopes.SoftSkill;

            yield return Create("None", "N", "None", "None", 0, 0, 0.54,
                seniorityLevelScope: extendedSeniorityLevelScope |  generalSeniorityLevelScope);

            yield return Create("TraineeL1", "T1", "Trainee -", "Trainee", 5, 1, 0.99, extendedSeniorityLevelScope);
            yield return Create("TraineeL2", "T2", "Trainee =", "Trainee", 15, 1, 1.33, extendedSeniorityLevelScope);
            yield return Create("Trainee", "T", "Trainee", "Trainee", 15, 1, 0,
                seniorityLevelScope: seniorityLevelScope | generalSeniorityLevelScope);
            yield return Create("TraineeL3", "T3", "Trainee +", "Trainee", 25, 1, 1.66, extendedSeniorityLevelScope);

            yield return Create("JuniorL1", "J1", "Junior -", "Junior", 30, 2, 1.99, extendedSeniorityLevelScope);
            yield return Create("Junior", "J", "Junior", "Junior", 50, 2, 0,
                seniorityLevelScope: seniorityLevelScope | generalSeniorityLevelScope);
            yield return Create("JuniorL2", "J2", "Junior =", "Junior", 50, 2, 2.33, extendedSeniorityLevelScope);
            yield return Create("JuniorL3", "J3", "Junior +", "Junior", 100,  2, 2.66, extendedSeniorityLevelScope);

            yield return Create("MiddleL1", "M1", "Middle -", "Middle", 130, 3, 2.99, extendedSeniorityLevelScope);
            yield return Create("Middle", "M", "Middle", "Middle", 150, 3, 0,
                seniorityLevelScope: seniorityLevelScope | generalSeniorityLevelScope);
            yield return Create("MiddleL2", "M2", "Middle =", "Middle", 150, 3, 3.33, extendedSeniorityLevelScope);
            yield return Create("MiddleL3", "M3", "Middle +", "Middle", 200, 3, 3.66, extendedSeniorityLevelScope);

            yield return Create("SeniorL1", "S1", "Senior -", "Senior", 230, 4, 3.99, extendedSeniorityLevelScope);
            yield return Create("Senior", "S", "Senior", "Senior", 250, 4, 0,
                seniorityLevelScope: seniorityLevelScope | generalSeniorityLevelScope);
            yield return Create("SeniorL2", "S2", "Senior =", "Senior", 250, 4, 4.33, extendedSeniorityLevelScope);
            yield return Create("SeniorL3", "S3", "Senior +", "Senior", 300,4, 4.66, extendedSeniorityLevelScope);

            yield return Create("LeadL1", "L1", "Lead -", "Lead", 350, 5, 4.99, extendedSeniorityLevelScope);
            yield return Create("Lead", "L", "Lead", "Lead", 400, 5, 0,
                seniorityLevelScope: seniorityLevelScope | generalSeniorityLevelScope);
            yield return Create("LeadL2", "L2", "Lead =", "Lead", 400, 5, 5.33, extendedSeniorityLevelScope);
            yield return Create("LeadL3", "L3", "Lead +", "Lead", 430, 5, 5.66, extendedSeniorityLevelScope);

            yield return Create("ArchitectL1", "A1", "Architect -", "Architect", 450, 6, 5.99, extendedSeniorityLevelScope);
            yield return Create("ArchitectL2", "A2", "Architect =", "Architect", 500, 6, 6, extendedSeniorityLevelScope);
            yield return Create("Architect", "A", "Architect", "Architect", 500, 6, 0,
                seniorityLevelScope: generalSeniorityLevelScope);
        }

        private static SeniorityLevel Create(string id, string shortName, string internalName, string externalName,
            int orderWeight, int coefficient, double specializationRangeTo, SeniorityLevelScopes seniorityLevelScope)
        {
            return new SeniorityLevel
            {
                Id = id,
                ShortName = shortName,
                InternalName = internalName,
                ExternalName = externalName,
                Description = externalName,
                OrderWeight = orderWeight,
                SeniorityLevelScope = seniorityLevelScope,
                Coefficient = coefficient,
                SpecializationRangeTo = specializationRangeTo
            };
        }
    }
}
