using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.OData;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UnitTests.Clients;

public class GetClientsRequestTests
{
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContext = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetClientsRequest _handler;

    public GetClientsRequestTests()
    {
        _handler = new GetClientsRequest(
            _timeTrackDbContext.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task HandleClientsDoNotExistShouldReturnEmptyList()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpClient>(queryParameters);

        var request = new GetClientsRequest.Query(queryOptions);

        SetupOdpClients([]);

        // Act
        var result = await _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.Page.Should().Be(0);
        result.PageSize.Should().Be(0);
    }

    [Fact]
    public async Task HandleWhen2ClientsExistShouldReturnListOf2Clients()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpClient>(queryParameters);

        var request = new GetClientsRequest.Query(queryOptions);

        var expectedResult = new List<ClientDto>
        {
            new()
            {
                Id = 1,
                Name = "Client 1",
                Color = null,
                Comments = null,
                FactTrackedTimeInMinutes = 100,
                IsActive = true,
                IsProjectExist = true,
                HasActiveProject = true,
                BillableTrackedTimeInMinutes = 100,
                RegisteredLocation = "Belarus",
            },
            new()
            {
                Id = 2,
                Name = "Client 2",
                Color = null,
                Comments = null,
                FactTrackedTimeInMinutes = 120,
                IsActive = true,
                IsProjectExist = true,
                HasActiveProject = true,
                BillableTrackedTimeInMinutes = 120,
                RegisteredLocation = "Belarus",
            }
        };

        var odpClients = new List<Client>()
        {
            new()
            {
                Id = 1,
                Name = "Client 1",
                Color = null,
                Comments = null,
                IsActive = true,
                Projects =
                [
                    new Project()
                    {
                        Id = 1,
                        Name = "Project",
                        IsActive = true,
                        Timesheets =
                        [
                            new ()
                            {
                                TaskName = "qwe",
                                FactTrackedInMinutes = 100,
                                BillableTrackedInMinutes = 100
                            }
                        ]
                    }
                ],
                RegisteredLocation = "Belarus",
            },
            new()
            {
                Id = 2,
                Name = "Client 2",
                Color = null,
                Comments = null,
                IsActive = true,
                Projects = [
                    new Project()
                    {
                        Id = 2,
                        Name = "Project2",
                        IsActive = true,
                        Timesheets =
                        [
                            new ()
                            {
                                TaskName = "qwe",
                                FactTrackedInMinutes = 120,
                                BillableTrackedInMinutes = 120
                            }
                        ]
                    }
                ],
                RegisteredLocation = "Belarus",
            }
        };

        SetupOdpClients(odpClients);

        // Act
        var result = await _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(expectedResult, opt => opt.WithStrictOrdering());
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(50);
    }

    private void SetupOdpClients(List<Client> clients)
    {
        _timeTrackDbContext
            .Setup(mock => mock.Clients)
            .Returns(clients.BuildMockDbSet().Object);
    }
}
