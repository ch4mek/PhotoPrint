using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Requests.Users.Requests;

namespace Modsen.Portal.Identity.UnitTests.Users.Validators;

public class UpdateUserRequestValidatorTests
{
    private readonly UpdateUserRequest.Validator _updateUserCommandValidator = new();

    [Theory]
    [InlineData(1, "Sara", "James", "Сара", "Джеймс", "Олеговна",
        "#ffffff", "12345@modsen-software.com", PortalRoleNames.Employee, LocationType.Vitebsk,
        "Net", null,true, PortalRoleNames.HeadOfDepartment, "Net")]
    [InlineData(1, "Sara", "James", null, null, null,
        "string", "12345@modsen-software.com", PortalRoleNames.Employee, LocationType.Vitebsk,
        "Net", null,false, PortalRoleNames.HeadOfDepartment, "Net")]
    [InlineData(1, "Sara", "James", null, null, null,
        "#ffffff", "12345@modsen-software.com", PortalRoleNames.Employee, LocationType.Vitebsk,
        null, null,true, PortalRoleNames.HeadOfDepartment, "Net")]

    public void UserValidatorForCreationTests(int id, string firstName, string lastName, string? firstNameRu,
        string? lastNameRu, string? patronymicRu, string color, string email, string role, LocationType location,
        string? departmentId, int? resourceGroupId, bool result, string currentUserRole, string? currentUserDepartmentId)
    {
        var command = new UpdateUserRequest.Command(
            id,
            firstName,
            lastName,
            firstNameRu,
            lastNameRu,
            color,
            CountryId: 1,
            RegisteredLocationId: PortalSharedConstants.DefaultRegisteredLocationId,
            CurrentLocation: "Minsk",
            email,
            role,
            location,
            departmentId,
            resourceGroupId,
            [currentUserRole],
            currentUserDepartmentId, HireDate: null);

        Assert.Equal(result, _updateUserCommandValidator.Validate(command).IsValid);
    }
}
