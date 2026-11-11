//using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
//using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;

//namespace Modsen.Portal.TimeTrack.UnitTests.BusinessRules;

//public class CanUserProjectBeCreatedBusinessRuleTests
//{
//    private readonly Mock<IGetProjectUsersByUserIdAndProjectIdDbQuery> _getProjectUsersByUserIdAndProjectIdDbQueryMock = new();

//    private readonly CanUserProjectBeCreatedBusinessRule _businessRule;
//    private readonly CancellationToken _cancellationToken = CancellationToken.None;

//    public CanUserProjectBeCreatedBusinessRuleTests()
//    {
//        _businessRule = new CanUserProjectBeCreatedBusinessRule(_getProjectUsersByUserIdAndProjectIdDbQueryMock.Object);
//    }

//    [Theory]
//    [MemberData(nameof(ReturnSuccess))]
//    public async Task IsCorrespondsToBusiness_WhenIntervalsAreNotMatching_ShouldReturnSuccess(object existing,
//        object model)
//    {
//        // Arrange
//        _getProjectUsersByUserIdAndProjectIdDbQueryMock.Setup(x => x.ExecuteAsync(
//                It.IsAny<GetProjectUsersByUserIdAndProjectIdDbRequest>(), _cancellationToken))
//            .ReturnsAsync([(GetProjectUsersByUserIdAndProjectIdDbResponse)existing]);

//        // Act
//        var result = await _businessRule.IsCorrespondsToBusiness((CanUserProjectBeCreatedModel)model, _cancellationToken);

//        // Assert
//        result.IsSuccess.Should().BeTrue();
//    }

//    [Theory]
//    [MemberData(nameof(ReturnFail))]
//    public async Task IsCorrespondsToBusiness_WhenIntervalsAreMatching_ShouldReturnFail(object existing,
//        object model)
//    {
//        // Arrange
//        _getProjectUsersByUserIdAndProjectIdDbQueryMock.Setup(x => x.ExecuteAsync(
//                It.IsAny<GetProjectUsersByUserIdAndProjectIdDbRequest>(), _cancellationToken))
//            .ReturnsAsync([(GetProjectUsersByUserIdAndProjectIdDbResponse)existing]);

//        // Act
//        var result = await _businessRule.IsCorrespondsToBusiness((CanUserProjectBeCreatedModel)model, _cancellationToken);

//        // Assert
//        result.IsSuccess.Should().BeFalse();
//    }

//    public static IEnumerable<object[]> ReturnSuccess =>
//        new List<object[]>
//        {
//            new object[]
//            {
//                new GetProjectUsersByUserIdAndProjectIdDbResponse(Id: 1, UserOnProjectFrom: new DateOnly(year: 2024, month: 4, day: 12),
//                    UserOnProjectTo: new DateOnly(year: 2024, month: 8, day: 12)),

//                new CanUserProjectBeCreatedModel(Id: 2, UserId: 1, ProjectId: 1, UserOnProjectFrom: new DateOnly(2024, month: 2, day: 12),
//                    UserOnProjectTo: new DateOnly(year: 2024, month: 3, day: 12))
//            },

//            new object[]
//            {
//                new GetProjectUsersByUserIdAndProjectIdDbResponse(Id: 1, UserOnProjectFrom: new DateOnly(year: 2024, month: 4, day: 12),
//                    UserOnProjectTo: new DateOnly(year: 2024, month: 8, day: 12)),

//                new CanUserProjectBeCreatedModel(Id: 2, UserId: 1, ProjectId: 1, UserOnProjectFrom: new DateOnly(2024, month: 8, day: 13),
//                    UserOnProjectTo: new DateOnly(year: 2024, month: 9, day: 12))
//            },

//            new object[]
//            {
//                new GetProjectUsersByUserIdAndProjectIdDbResponse(Id: 1, UserOnProjectFrom: new DateOnly(year: 2024, month: 4, day: 12),
//                    UserOnProjectTo: null),

//                new CanUserProjectBeCreatedModel(Id: 2, UserId: 1, ProjectId: 1, UserOnProjectFrom: new DateOnly(2024, month: 2, day: 12),
//                UserOnProjectTo: new DateOnly(year: 2024, month: 3, day: 12))
//            },

//            new object[]
//            {
//                new GetProjectUsersByUserIdAndProjectIdDbResponse(Id: 1, UserOnProjectFrom: new DateOnly(year: 2024, month: 4, day: 12),
//                    UserOnProjectTo: new DateOnly(year: 2024, month: 8, day: 12)),

//                new CanUserProjectBeCreatedModel(Id: 2, UserId: 1, ProjectId: 1, UserOnProjectFrom: new DateOnly(2024, month: 9, day: 13),
//                UserOnProjectTo: null)
//            }
//        };

//    public static IEnumerable<object[]> ReturnFail =>
//        new List<object[]>
//        {
//            new object[]
//            {
//                new GetProjectUsersByUserIdAndProjectIdDbResponse(Id: 1,
//                UserOnProjectFrom: new DateOnly(2024, month: 1, day: 12), UserOnProjectTo: null),

//                new CanUserProjectBeCreatedModel(Id: 2, UserId: 1, ProjectId: 1, UserOnProjectFrom: new DateOnly(2024, month: 2, day: 12),
//                UserOnProjectTo: null)
//            },

//            new object[]
//            {
//                new GetProjectUsersByUserIdAndProjectIdDbResponse(Id: 1,
//                UserOnProjectFrom: new DateOnly(2024, month: 3, day: 12), UserOnProjectTo: null),

//                new CanUserProjectBeCreatedModel(Id: 2, UserId: 1, ProjectId: 1, UserOnProjectFrom: new DateOnly(2024, month: 2, day: 12),
//                UserOnProjectTo: null)
//            },

//            new object[]
//            {
//                new GetProjectUsersByUserIdAndProjectIdDbResponse(Id: 1,
//                UserOnProjectFrom: new DateOnly(year: 2024, month: 4, day: 12), UserOnProjectTo: null),

//                new CanUserProjectBeCreatedModel(Id: 2, UserId: 1, ProjectId: 1, UserOnProjectFrom: new DateOnly(2024, month: 1, day: 13),
//                UserOnProjectTo: new DateOnly(year: 2024, month: 9, day: 12))
//            },

//            new object[]
//            {
//                new GetProjectUsersByUserIdAndProjectIdDbResponse(Id: 1,
//                UserOnProjectFrom: new DateOnly(year: 2024, month: 4, day: 12), UserOnProjectTo: null),

//                new CanUserProjectBeCreatedModel(Id: 2, UserId: 1, ProjectId: 1, UserOnProjectFrom: new DateOnly(2024, month: 4, day: 13),
//                UserOnProjectTo: new DateOnly(year: 2024, month: 9, day: 12))
//            },

//            new object[]
//            {
//                new GetProjectUsersByUserIdAndProjectIdDbResponse(Id: 1,
//                UserOnProjectFrom: new DateOnly(year: 2024, month: 4, day: 12), UserOnProjectTo: new DateOnly(year: 2024, month: 8, day: 12)),

//                new CanUserProjectBeCreatedModel(Id: 2, UserId: 1, ProjectId: 1, UserOnProjectFrom: new DateOnly(2024, month: 2, day: 12),
//                UserOnProjectTo: null)
//            },

//            new object[]
//            {
//                new GetProjectUsersByUserIdAndProjectIdDbResponse(Id: 1,
//                UserOnProjectFrom: new DateOnly(year: 2024, month: 1, day: 12), UserOnProjectTo: new DateOnly(year: 2024, month: 8, day: 12)),

//                new CanUserProjectBeCreatedModel(Id: 2, UserId: 1, ProjectId: 1, UserOnProjectFrom: new DateOnly(2024, month: 2, day: 12),
//                UserOnProjectTo: null)
//            },

//            new object[]
//            {
//                new GetProjectUsersByUserIdAndProjectIdDbResponse(Id: 1,
//                UserOnProjectFrom: new DateOnly(year: 2024, month: 4, day: 12), UserOnProjectTo: new DateOnly(year: 2024, month: 8, day: 12)),

//                new CanUserProjectBeCreatedModel(Id: 2, UserId: 1, ProjectId: 1, UserOnProjectFrom: new DateOnly(2024, month: 3, day: 13),
//                UserOnProjectTo:  new DateOnly(2024, month: 5, day: 13))
//            },

//            new object[]
//            {
//                new GetProjectUsersByUserIdAndProjectIdDbResponse(Id: 1,
//                UserOnProjectFrom: new DateOnly(year: 2024, month: 4, day: 12), UserOnProjectTo: new DateOnly(year: 2024, month: 8, day: 12)),

//                new CanUserProjectBeCreatedModel(Id: 2, UserId: 1, ProjectId: 1, UserOnProjectFrom: new DateOnly(2024, month: 5, day: 13),
//                UserOnProjectTo:  new DateOnly(2024, month: 10, day: 13))
//            }
//        };
//}
