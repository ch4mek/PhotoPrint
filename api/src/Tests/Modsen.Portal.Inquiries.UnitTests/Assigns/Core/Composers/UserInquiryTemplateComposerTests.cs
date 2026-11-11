using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Composers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Assigns.Core.Composers;

public class UserInquiryTemplateComposerTests
{
    private readonly Mock<IInquiriesDbContext> _dbContextMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly UserInquiryTemplateComposer _composer;

    public UserInquiryTemplateComposerTests()
    {
        _composer = new UserInquiryTemplateComposer(_dbContextMock.Object);
    }

    [Fact]
    public async Task ComposeAsyncShouldCreateValidUserInquiryTemplates()
    {
        //Arrange
        var userIds = new[] { 1, 2, 3, 4 };
        var date = DateOnly.FromDateTime(DateTime.Today);

        var inquiryTemplate = new InquiryTemplateEntity
        {
            Id = 1,
            InquiryType = InquiryType.Vacation,
            Name = "test",
        };

        var userAlreadyHasThisTemplate = new UserInquiryTemplateEntity
        {
            AssignedToUserId = 1,
            InquiryTemplateId = 1,
            InquiryTemplate = new InquiryTemplateEntity
            {
                InquiryType = InquiryType.Vacation,
                Name = "test",
            },
            DateTo = null,
        };

        var userHasAnotherTemplate = new UserInquiryTemplateEntity
        {
            AssignedToUserId = 2,
            InquiryTemplateId = 2,
            InquiryTemplate = new InquiryTemplateEntity()
            {
                InquiryType = InquiryType.HomeOffice,
                Name = "test",
            },
            DateTo = null
        };

        var secondUserHasAnotherTemplate = new UserInquiryTemplateEntity
        {
            AssignedToUserId = 3,
            InquiryTemplateId = 4,
            InquiryTemplate = new InquiryTemplateEntity()
            {
                InquiryType = InquiryType.SickDay,
                Name = "test",
            },
            DateTo = null
        };

        var userHasAgreementOnSameTemplateTypeButAnotherTemplateId = new UserInquiryTemplateEntity
        {
            AssignedToUserId = 4,
            InquiryTemplateId = 2,
            InquiryTemplate = new InquiryTemplateEntity
            {
                InquiryType = inquiryTemplate.InquiryType,
                Id = inquiryTemplate.Id + 1,
                Name = "test",
            },
            DateTo = null
        };

        _dbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>
            {
                userAlreadyHasThisTemplate,
                userHasAnotherTemplate,
                secondUserHasAnotherTemplate,
                userHasAgreementOnSameTemplateTypeButAnotherTemplateId
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new()
                {
                    Id = 1,
                    InquiryType  = InquiryType.Vacation,
                    Name = "test",
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new()
                {
                    UserId = 1,Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
                new()
                {
                    UserId = 2,Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
                new()
                {
                    UserId = 3,Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(context => context.SaveChangesAsync(_token));

        _dbContextMock.Setup(context => context.UserInquiryTemplates.AddRange(It.IsAny<List<UserInquiryTemplateEntity>>()));

        //Act
        var result = await _composer.Compose(userIds, date, inquiryTemplate.Id, _token);

        //Assert
        result.Count.Should().Be(4);
    }
}
