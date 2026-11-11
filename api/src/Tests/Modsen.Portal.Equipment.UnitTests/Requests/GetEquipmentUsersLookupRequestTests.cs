using MockQueryable.Moq;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Users.Requests;
using Modsen.Portal.Equipment.Requests.Users.Requests.RequestDtos;

namespace Modsen.Portal.Equipment.UnitTests.Requests;

public class GetEquipmentUsersLookupRequestTests
{
    private readonly Mock<IEquipmentDbContext> _equipmentDbContextMock = new();
    private readonly GetUserLookupRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetEquipmentUsersLookupRequestTests()
    {
        _handler = new(_equipmentDbContextMock.Object);
    }

    [Fact]
    public async Task HandleShouldReturnActiveEquipmentUsers()
    {
        //Arrange
        var firstName = Guid.CreateVersion7().ToAlphabetic();
        var lastName = Guid.CreateVersion7().ToAlphabetic();

        var expectedUsers = new List<UserLookupDto>
        {
            new()
            {
                Id = 1,
                FirstName = firstName,
                LastName = lastName,
            }
        };

        var users = new List<EquipmentUser>
        {
            new()
            {
                UserId = 1,
                FirstName = firstName,
                LastName = lastName,
                Email = "test",
                IsActive = true
            }
        };

        _equipmentDbContextMock
            .Setup(mock => mock.EquipmentUsers)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var result = await _handler.Handle(new([]), _cancellationToken);

        //Assert

        result.Should().BeEquivalentTo(expectedUsers);
    }
}
