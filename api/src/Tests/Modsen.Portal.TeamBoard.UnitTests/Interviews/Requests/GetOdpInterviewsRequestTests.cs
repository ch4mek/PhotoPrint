using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.OData;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.AuthenticationFilters;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Interviews.Requests;

public class GetOdpInterviewsRequestTests
{
    private readonly GetOdpInterviewsRequest _handler;
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContext = new();
    private readonly Mock<IInterviewsAuthenticationFilter> _interviewsAuthenticationFilter = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetOdpInterviewsRequestTests()
    {
        _handler = new GetOdpInterviewsRequest(_teamBoardDbContext.Object, _interviewsAuthenticationFilter.Object);

        _groupIds =
        [
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7()
        ];

        _userInterviews = SetupDbContext();
    }

    private readonly List<Guid> _groupIds;
    private readonly List<UserInterview> _userInterviews;

    private List<UserInterview> SetupDbContext()
    {
        var userInterviews = new List<UserInterview>()
        {
            new UserInterview
            {
                Id = 1,
                UserId = 1,
                User = new TeamBoardUser
                {
                    FirstName = "Dep Net",
                    LastName = "User one",
                    FirstNameRu = "CreatedBy 1000",
                    Email = "Dep Net",
                    DepartmentId = "Net",
                    ResourceGroupId = null,
                },
                Specialization = "Spec 1",
                GroupId = _groupIds[0],
                IsActive = true,
                CreatedById = 1000,
            },
            new UserInterview
            {
                Id = 2,
                UserId = 1,
                User = new TeamBoardUser
                {
                    FirstName = "Dep Net",
                    LastName = "User one",
                    FirstNameRu = "CreatedBy 1000",
                    Email = "Dep Net",
                    DepartmentId = "Net",
                    ResourceGroupId = null,
                },
                Specialization = "Spec 1",
                GroupId = _groupIds[0],
                IsActive = true,
                CreatedById = 1000,
            },
            new UserInterview
            {
                Id = 3,
                UserId = 2,
                User = new TeamBoardUser
                {
                    FirstName = "Dep Js",
                    LastName = "User two",
                    FirstNameRu = "Group 1",
                    Email = "Dep Js",
                    DepartmentId = "Js",
                    ResourceGroupId = 1,
                },
                Specialization = "Spec 1",
                GroupId = _groupIds[1],
                IsActive = true,
            },
            new UserInterview
            {
                Id = 4,
                UserId = 2,
                User = new TeamBoardUser
                {
                    FirstName = "Dep Js",
                    LastName = "User two",
                    FirstNameRu = "Group 1",
                    Email = "Dep Js",
                    DepartmentId = "Js",
                    ResourceGroupId = 1,
                },
                Specialization = "Spec 1",
                GroupId = _groupIds[1],
                IsActive = true,
            },
        };

        _teamBoardDbContext.SetupGet(x => x.UserInterviews)
            .Returns(userInterviews.BuildMockDbSet().Object);

        return userInterviews;
    }

    [Fact]
    public async Task HandleWhenUserHasManageAllInterviewsReturnAllInterviews()
    {
        // Arrange
        _interviewsAuthenticationFilter.Setup(x => x.Apply(It.IsAny<IQueryable<UserInterview>>()))
            .Returns(_userInterviews.BuildMockDbSet().Object);

        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpInterview>(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "20",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        });

        // Act
        var result = await _handler.Handle(new GetOdpInterviewsRequest.Query(queryOptions),
            _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(2);
        result.Items.Should().Contain(x => x.Main.Id == 2 && x.Stages.Select(y => y.Id).Contains(1));
        result.Items.Should().Contain(x => x.Main.Id == 4 && x.Stages.Select(y => y.Id).Contains(3));
    }
}
