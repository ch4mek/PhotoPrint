using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TimeTracker
{
    public static class TimeTrackerInputFaker
    {
        public static CreateUserInput FakeCreateUserInput(string color = "#ffffff", LocationType location = LocationType.Minsk,
            string position = PortalRoleNames.Employee, string department = "Net", int countryId = 1,
            int registeredLocationId = PortalSharedConstants.DefaultRegisteredLocationId)
        {
            return new CreateUserInput
            {
                FirstName = Guid.CreateVersion7().ToAlphabetic(),
                LastName = Guid.CreateVersion7().ToAlphabetic(),
                FirstNameRu = "Имя",
                LastNameRu = "Фамилия",
                PatronymicRu = "Отчество",
                Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                Color = color,
                Location = location,
                Position = position,
                DepartmentId = department,
                CountryId = countryId,
                RegisteredLocationId = registeredLocationId
            };
        }

        public static CreateProjectInput FakeCreateProjectInput(int clientId, string color = "#ffffff",
            bool isBillable = false, bool isPublic = false)
        {
            return new CreateProjectInput
            {
                ClientId = clientId,
                Color = "#ffffff",
                ManagerId = null,
                Name = Guid.CreateVersion7().ToAlphabetic(),
                IsBillable = isBillable,
                IsPublic = isPublic
            };
        }
    }
}
