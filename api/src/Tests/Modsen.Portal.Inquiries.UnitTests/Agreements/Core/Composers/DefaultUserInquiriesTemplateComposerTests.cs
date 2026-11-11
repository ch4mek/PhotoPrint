//using MockQueryable.Moq;
//using Modsen.Portal.Core.Inquiries;
//using Modsen.Portal.Core.Exceptions;
//using Modsen.Portal.Inquiries.DataAccess.Npgsql;
//using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
//using Modsen.Portal.Inquiries.Requests.Agreements.Core.Composers;
//using Modsen.Portal.TeamBoard.Contract;
//using Modsen.Portal.TeamBoard.Contract.Contract.Output;
//using Moq;

//namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Composers;

//public class DefaultUserInquiriesTemplateComposerTests
//{
//    private readonly IDefaultUserInquiriesTemplateComposer _composer;
//    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock;
//    private readonly Mock<ITeamBoardContract> _teamBoardContractMock;
//    private readonly CancellationToken _cancellationToken;

//    public DefaultUserInquiriesTemplateComposerTests()
//    {
//        _inquiriesDbContextMock = new Mock<IInquiriesDbContext>();
//        _teamBoardContractMock = new Mock<ITeamBoardContract>();
//        _composer = new DefaultUserInquiriesTemplateComposer(
//            _inquiriesDbContextMock.Object,
//            _teamBoardContractMock.Object);
//        _cancellationToken = CancellationToken.None;
//    }

//    [Fact]
//    public async Task ComposeWhenDefaultTemplatesWithoutAgreementsExistReturnsCorrectUserInquiryTemplates()
//    {
//        // Arrange
//        var userId = 3;
//        var hireDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-5));
//        var fireDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6));

//        var defaultTemplates = new List<InquiryTemplateEntity>
//        {
//            new()
//            {
//                Id = 1,
//                Name = "Social vacation",
//                InquiryType = InquiryType.SocialVacation,
//                IsDefault = true,
//                IsActive = true,
//                InquiryTemplateAgreements = []
//            },
//            new()
//            {
//                Id = 2,
//                Name = "Military enlistment office",
//                InquiryType = InquiryType.MilitaryEnlistmentOffice,
//                IsDefault = true,
//                IsActive = true,
//                InquiryTemplateAgreements = []
//            }
//        };

//        _teamBoardContractMock
//            .Setup(x => x.GetUserContractDates(userId, _cancellationToken))
//            .ReturnsAsync(new UserCotractDatesOutput
//            {
//                UserId = userId,
//                HireDate = hireDate,
//                FireDate = fireDate,
//                ContractEndDate = fireDate.AddMonths(1)
//            });

//        _inquiriesDbContextMock
//            .Setup(x => x.InquiryTemplates)
//            .Returns(defaultTemplates.BuildMockDbSet().Object);

//        // Act
//        var result = await _composer.Compose(userId, _cancellationToken);

//        // Assert
//        result.Count.Should().Be(2);
//        result.Should().ContainSingle(x => x.InquiryTemplateId == 1)
//            .Which.Should().BeEquivalentTo(new UserInquiryTemplateEntity
//            {
//                AssignedToUserId = userId,
//                DateFrom = hireDate,
//                DateTo = fireDate,
//                InquiryTemplateId = 1
//            }, options => options.Excluding(x => x.Id));
//        result.Should().ContainSingle(x => x.InquiryTemplateId == 2)
//            .Which.Should().BeEquivalentTo(new UserInquiryTemplateEntity
//            {
//                AssignedToUserId = userId,
//                DateFrom = hireDate,
//                DateTo = fireDate,
//                InquiryTemplateId = 2
//            }, options => options.Excluding(x => x.Id));
//    }

//    [Fact]
//    public async Task ComposeWhenNoDefaultTemplatesWithoutAgreementsExistReturnsEmptyList()
//    {
//        // Arrange
//        var userId = 3;
//        var hireDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-5));
//        var fireDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6));

//        _teamBoardContractMock
//            .Setup(x => x.GetUserContractDates(userId, _cancellationToken))
//            .ReturnsAsync(new UserCotractDatesOutput
//            {
//                UserId = userId,
//                HireDate = hireDate,
//                FireDate = fireDate,
//                ContractEndDate = fireDate.AddMonths(1)
//            });

//        _inquiriesDbContextMock
//            .Setup(x => x.InquiryTemplates)
//            .Returns(new List<InquiryTemplateEntity>().BuildMockDbSet().Object);

//        // Act
//        var result = await _composer.Compose(userId, _cancellationToken);

//        // Assert
//        result.Should().BeEmpty();
//    }

//    [Fact]
//    public async Task ComposeWhenHireDateIsNullThrowsEntityConflictException()
//    {
//        // Arrange
//        var userId = 3;

//        _inquiriesDbContextMock
//            .Setup(x => x.InquiryTemplates)
//            .Returns(new List<InquiryTemplateEntity>().BuildMockDbSet().Object);

//        _teamBoardContractMock
//            .Setup(x => x.GetUserContractDates(userId, _cancellationToken))
//            .ReturnsAsync(new UserCotractDatesOutput
//            {
//                UserId = userId,
//                HireDate = null,
//                FireDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)),
//                ContractEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(7))
//            });

//        // Act & Assert
//        await Assert.ThrowsAsync<EntityConflictException>(() =>
//            _composer.Compose(userId, _cancellationToken));
//    }

//    [Fact]
//    public async Task ComposeWhenUserContractDatesIsNullThrowsEntityConflictException()
//    {
//        // Arrange
//        var userId = 3;

//        _inquiriesDbContextMock
//            .Setup(x => x.InquiryTemplates)
//            .Returns(new List<InquiryTemplateEntity>().BuildMockDbSet().Object);

//        _teamBoardContractMock
//            .Setup(x => x.GetUserContractDates(userId, _cancellationToken))
//            .ReturnsAsync((UserCotractDatesOutput?)null);

//        // Act & Assert
//        await Assert.ThrowsAsync<EntityConflictException>(() =>
//            _composer.Compose(userId, _cancellationToken));
//    }

//}
