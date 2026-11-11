using Bogus;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.Domain;
using Modsen.Portal.TimeTrack.UseCases.Queries.GetUsersLookupWithFilters;

namespace Modsen.Portal.TimeTrack.UnitTests.UseCases.Queries;

public class GetUsersLookupWithFiltersQueryHandlerTests
{
    private readonly Mock<ITimeTrackUserProvider> _usersProviderMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly Faker _faker = new();

    [Fact]
    public async Task HandleWhenDataExistsWithoutFilterPropertiesReturnListOfUserDto()
    {
        //Arrange
        var name1 = _faker.Name.FirstName();
        var name2 = _faker.Name.FirstName();
        var name3 = _faker.Name.FirstName();
        var name4 = _faker.Name.FirstName();

        var lastName1 = _faker.Name.LastName();
        var lastName2 = _faker.Name.LastName();
        var lastName3 = _faker.Name.LastName();
        var lastName4 = _faker.Name.LastName();

        var query = new GetUsersLookupWithFiltersQuery(null, null, null, null);

        var entities = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,Email = "test",
                LastName = lastName1
            },
            new ()
            {
                UserId = 2,
                FirstName = name2,Email = "test",
                LastName = lastName2
            },
            new ()
            {
                UserId = 3,
                FirstName = name3,Email = "test",
                LastName = lastName3
            },
            new ()
            {
                UserId = 4,
                FirstName = name4,Email = "test",
                LastName = lastName4
            }
        };
        var users = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,
                LastName = lastName1,
                Email = "modsen1@modsen-software.com"
            },

            new ()
            {
                UserId = 2,
                FirstName = name2,
                LastName = lastName2,
                Email = "modsen2@modsen-software.com"
            },

            new ()
            {
                UserId = 3,
                FirstName = name3,
                LastName = lastName3,
                Email = "modsen3@modsen-software.com"
            },

            new ()
            {
                UserId = 4,
                FirstName = name4,
                LastName = lastName4,
                 Email = "modsen4@modsen-software.com"
            }
        };

        _usersProviderMock.Setup(x => x.GetLookupWithParametersAsync(It.IsAny<FilterParams>(), It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(users);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);
        _authenticatedUserContext.Setup(x => x.DepartmentId).Returns(null as string);
        var handler = new GetUsersLookupWithFiltersQueryHandler(_usersProviderMock.Object, _authenticatedUserContext.Object);

        //Act
        var result = await handler.Handle(query, _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        _usersProviderMock.Verify(x => x.GetLookupWithParametersAsync(It.IsAny<FilterParams>(), It.IsAny<string>(), _cancellationToken), Times.Once);
        result.Count.Should().Be(entities.Count);
    }

    [Fact]
    public async Task HandleWhenFilterHasProjectIdReturnListOfUserDto()
    {
        //Arrange
        var query = new GetUsersLookupWithFiltersQuery(null, 2, null, null);

        var name1 = _faker.Name.FirstName();
        var name2 = _faker.Name.FirstName();
        var name3 = _faker.Name.FirstName();
        var name4 = _faker.Name.FirstName();

        var lastName1 = _faker.Name.LastName();
        var lastName2 = _faker.Name.LastName();
        var lastName3 = _faker.Name.LastName();
        var lastName4 = _faker.Name.LastName();

        var entities = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,Email = "test",
                LastName = lastName1
            },
            new ()
            {
                UserId = 2,
                FirstName = name2,Email = "test",
                LastName = lastName2
            },
            new ()
            {
                UserId = 3,
                FirstName = name3,Email = "test",
                LastName = lastName3
            },
            new ()
            {
                UserId = 4,
                FirstName = name4,Email = "test",
                LastName = lastName4
            }
        };
        var users = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,
                LastName = lastName1,
                Email = "modsen1@modsen-software.com"
            },

            new ()
            {
                UserId = 3,
                FirstName = name3,
                LastName = lastName3,
                Email = "modsen3@modsen-software.com"
            }
        };

        _usersProviderMock.Setup(x => x.GetLookupWithParametersAsync(It.IsAny<FilterParams>(), It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(users);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);
        _authenticatedUserContext.Setup(x => x.DepartmentId).Returns(null as string);
        var handler = new GetUsersLookupWithFiltersQueryHandler(_usersProviderMock.Object, _authenticatedUserContext.Object);

        //Act
        var result = await handler.Handle(query, _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        _usersProviderMock.Verify(x => x.GetLookupWithParametersAsync(It.IsAny<FilterParams>(), It.IsAny<string>(), _cancellationToken), Times.Once);
        result.Count.Should().NotBe(entities.Count);
    }

    [Fact]
    public async Task HandleWhenAuthUserIsHeadOfJsReturnListOfUserDto()
    {
        //Arrange
        var name1 = _faker.Name.FirstName();
        var name2 = _faker.Name.FirstName();
        var name3 = _faker.Name.FirstName();
        var name4 = _faker.Name.FirstName();

        var lastName1 = _faker.Name.LastName();
        var lastName2 = _faker.Name.LastName();
        var lastName3 = _faker.Name.LastName();
        var lastName4 = _faker.Name.LastName();

        var query = new GetUsersLookupWithFiltersQuery(null, null, null, null);

        var entities = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,Email = "test",
                LastName = lastName1,
                DepartmentId = "Js"
            },
            new ()
            {
                UserId = 2,
                FirstName = name2,Email = "test",
                LastName = lastName2,
                DepartmentId = "Net"
            },
            new ()
            {
                UserId = 3,
                FirstName = name3,Email = "test",
                LastName = lastName3,
                DepartmentId = "Net"
            },
            new ()
            {
                UserId = 4,
                FirstName = name4,Email = "test",
                LastName = lastName4,
                DepartmentId = "Java"
            }
        };

        var users = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,
                LastName = lastName1,
                Email = "modsen1@modsen-software.com"
            }
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContext.Setup(x => x.DepartmentId).Returns("Js");

        _usersProviderMock.Setup(x => x.GetLookupWithParametersAsync(It.IsAny<FilterParams>(), It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(users);

        var handler = new GetUsersLookupWithFiltersQueryHandler(_usersProviderMock.Object, _authenticatedUserContext.Object);

        //Act
        var result = await handler.Handle(query, _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        _usersProviderMock.Verify(x => x.GetLookupWithParametersAsync(It.IsAny<FilterParams>(), It.IsAny<string>(), _cancellationToken), Times.Once);
        result.Count.Should().NotBe(entities.Count);
        entities.Find(x => x.UserId == 1)!.DepartmentId.Should().Be(_authenticatedUserContext.Object.DepartmentId);
    }

    [Fact]
    public async Task HandleWhenKeyWordIsNotEmptyReturnListOfUserDto()
    {
        //Arrange
        var name1 = _faker.Name.FirstName();
        var name2 = _faker.Name.FirstName();
        var name3 = _faker.Name.FirstName();
        var name4 = _faker.Name.FirstName();

        var lastName1 = _faker.Name.LastName();
        var lastName2 = _faker.Name.LastName();
        var lastName3 = _faker.Name.LastName();
        var lastName4 = _faker.Name.LastName();

        var query = new GetUsersLookupWithFiltersQuery(null, null, null, name1);

        var entities = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,Email = "test",
                LastName = lastName1,
                DepartmentId= "Js"
            },
            new ()
            {
                UserId = 2,
                FirstName = name2,Email = "test",
                LastName = lastName2,
                DepartmentId = "Net"
            },
            new ()
            {
                UserId = 3,
                FirstName = name3,Email = "test",
                LastName = lastName3,
                DepartmentId= "Net"
            },
            new ()
            {
                UserId = 4,Email = "test",
                FirstName = name4,
                LastName = lastName4,
                DepartmentId= "Java"
            }
        };

        var users = new List<TimeTrackUser>
        {
           new ()
            {
                UserId = 1,
                FirstName = name1,
                LastName = lastName1,
                Email = "modsen1@modsen-software.com"
            }
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);
        _authenticatedUserContext.Setup(x => x.DepartmentId).Returns(null as string);

        _usersProviderMock.Setup(x => x.GetLookupWithParametersAsync(It.IsAny<FilterParams>(), It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(users);
        var handler = new GetUsersLookupWithFiltersQueryHandler(_usersProviderMock.Object, _authenticatedUserContext.Object);

        //Act
        var result = await handler.Handle(query, _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        _usersProviderMock.Verify(x => x.GetLookupWithParametersAsync(It.IsAny<FilterParams>(), name1, _cancellationToken), Times.Once);
        result.Count.Should().NotBe(entities.Count);
        result[0].FirstName.Should().Be(name1);

    }
}
