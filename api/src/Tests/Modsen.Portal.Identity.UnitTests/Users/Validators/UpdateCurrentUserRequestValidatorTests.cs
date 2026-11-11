using Modsen.Portal.Core;
using Modsen.Portal.Identity.Requests.Users.Requests;

namespace Modsen.Portal.Identity.UnitTests.Users.Validators;

public class UpdateCurrentUserRequestValidatorTests
{
    private readonly UpdateCurrentUserRequest.Validator _updateCurrentUserValidator = new();

    [Theory]
    [InlineData(LocationType.Vitebsk, "Net", "BY", true)]
    [InlineData(LocationType.Vitebsk, null, "BY", true)]
    [InlineData(LocationType.Vitebsk, "Net", "OK", false)]
    public void UserValidatorForUpdatingTests(LocationType location, string? departmentId,
        string timeZone, bool result)
    {
        var command = new UpdateCurrentUserRequest.Command(
            timeZone,
            null,
            null,
            departmentId,
            location)
        {
            Location = location,
            DepartmentId = departmentId,
            TimeZone = timeZone
        };

        Assert.Equal(result, _updateCurrentUserValidator.Validate(command).IsValid);
    }
}
