using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.OData;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Skills.UserSnapshots.Requests;

public class GetUserSnapshotsByUserIdRequestTests
{
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _mockCanTeamBoardUserBeRetrievedBusinessRule;
    private readonly Mock<ITeamBoardDbContext> _mockTeamBoardDbContext;
    private readonly Mock<IUserSkillsSnapshotsLoader> _mockUserSkillsSnapshotsLoader;
    private readonly GetUserSnapshotsByUserIdRequest _handler;

    public GetUserSnapshotsByUserIdRequestTests()
    {
        _mockCanTeamBoardUserBeRetrievedBusinessRule = new Mock<ICanTeamBoardUserBeRetrievedBusinessRule>();
        _mockTeamBoardDbContext = new Mock<ITeamBoardDbContext>();
        _mockUserSkillsSnapshotsLoader = new Mock<IUserSkillsSnapshotsLoader>();

        _handler = new GetUserSnapshotsByUserIdRequest(
            _mockTeamBoardDbContext.Object,
            _mockCanTeamBoardUserBeRetrievedBusinessRule.Object,
            _mockUserSkillsSnapshotsLoader.Object);
    }

    [Fact]
    public async Task HandleShouldThrowNotFoundExceptionWhenBusinessRuleFails()
    {
        // Arrange
        var query = new GetUserSnapshotsByUserIdRequest.Query(It.IsAny<int>(),
            It.IsAny<ODataQueryOptions<UserSpecializationTemplateWithSnapshotDto>>());

        _mockCanTeamBoardUserBeRetrievedBusinessRule.Setup(x =>
                x.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = () => _handler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleShouldReturnUserSnapshots()
    {
        // Arrange
        var userId = 10;
        var specTemplateId = 5;

        var queryParameters = new Dictionary<string, string?>()
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock
            .BuildMock<UserSpecializationTemplateWithSnapshotDto>(queryParameters);

        var query = new GetUserSnapshotsByUserIdRequest.Query(userId, queryOptions);

        var userSnapshots = new List<UserSnapshotCoreDto>
        {
            new UserSnapshotCoreDto
            {
                Id = 1,
                UserId = userId,
                SnapshotDate = new DateOnly(2026, 3, 9),
                UserSpecializations = new List<UserSnapshotSpecializationCoreDto>
                {
                    new UserSnapshotSpecializationCoreDto
                    {
                        Id = 1,
                        SpecializationTemplateId = specTemplateId,
                        SpecializationTemplateName = "Backend",
                        SeniorityLevelId = "S1",
                        SeniorityLevelName = "Senior",
                        CreatedAtUtc = DateTime.UtcNow,
                        IsPrimary = true,
                        HardSkills = []
                    }
                }
            }
        };

         var specializationData = new List<DataAccess.Npgsql.DatabaseEntities.UserSpecialization>
            {
                new DataAccess.Npgsql.DatabaseEntities.UserSpecialization
                {
                    Id = 1,
                    UserId = userId,
                    SpecializationTemplateId = specTemplateId,
                    IsPrimary = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    User = new TeamBoardUser
                    {
                        UserId = userId,
                        IsActive = true,
                        FirstName = null,
                        LastName = null,
                        Email = null
                    },
                    SeniorityLevel = new SeniorityLevel
                    {
                        Id = "S1",
                        InternalName = "Senior",
                        ShortName = null,
                        ExternalName = null,
                        Description = null
                    },
                    SpecializationTemplate = new SpecializationTemplate
                    {
                        Id = specTemplateId,
                        Name = "Backend"
                    }
                }
            };

        _mockCanTeamBoardUserBeRetrievedBusinessRule.Setup(x =>
                x.IsCorrespondsToBusiness(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserSkillsSnapshotsLoader.Setup(x => x.Load(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userSnapshots);

        _mockTeamBoardDbContext.Setup(x => x.UserSpecializations)
            .Returns(specializationData.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.First().SpecializationTemplateId.Should().Be(specTemplateId);
        result.Items.First().SpecializationSnapshots.Should().NotBeEmpty();
    }
}
