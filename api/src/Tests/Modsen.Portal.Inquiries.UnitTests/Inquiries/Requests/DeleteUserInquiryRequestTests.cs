using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Dto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Loaders;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Requests;

public class DeleteUserInquiryRequestTests
{
    private readonly Mock<IUserInquiryLoader> _userInquiryLoader = new();
    private readonly Mock<IUserLoader> _userLoader = new();
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContext = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<IHasUserAccessToMutateInquiryBusinessRule> _hasUserAccessToMutateInquiryBusinessRule = new();

    private readonly DeleteUserInquiryRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteUserInquiryRequestTests()
    {
        _handler = new DeleteUserInquiryRequest(
            _userInquiryLoader.Object,
            _userLoader.Object,
            _inquiriesDbContext.Object,
            _authenticatedUserContext.Object,
            _hasUserAccessToMutateInquiryBusinessRule.Object
        );

        _authenticatedUserContext.SetupUserContext(true, true);
    }

    [Fact]
    public async Task HandleWhenUserDeleteOwnInquiryShouldBeSuccess()
    {
        // Arrange
        var userId = InquiryRequestTestsHelper.AuthenticatedUserId;
        var userDto = CreateUserDto(userId);
        var (userInquiryEntity, userInquiryDto) = CreateUserInquiry(userDto, UserInquiryStatus.Open);
        var userInquiryAccept = CreateUserInquiryAcceptEntity(userDto, userInquiryEntity);
        var acceptsList = new List<UserInquiryAcceptEntity> { userInquiryAccept };
        var commentsList = new List<UserInquiryCommentEntity>();

        MockInquiriesDbContext([userInquiryEntity], [userInquiryAccept], []);
        _hasUserAccessToMutateInquiryBusinessRule.Setup(x => x.IsCorrespondsToBusiness(userId, _cancellationToken))
            .ReturnsAsync(true);
        _userLoader.Setup(x => x.GetUserByIdAsync(userId, _cancellationToken)).ReturnsAsync(userDto);
        _userInquiryLoader.Setup(x => x.Load(userInquiryEntity.Id, _cancellationToken)).ReturnsAsync(userInquiryDto);
        _inquiriesDbContext
            .Setup(x => x.UserInquiryComments.Add(It.IsAny<UserInquiryCommentEntity>()))
            .Callback<UserInquiryCommentEntity>(s => commentsList.Add(s));
        _inquiriesDbContext
            .Setup(x => x.UserInquiryAccepts.RemoveRange(It.IsAny<IEnumerable<UserInquiryAcceptEntity>>()))
            .Callback<IEnumerable<UserInquiryAcceptEntity>>(s => acceptsList.RemoveAll(x => s.Any(y => y.Id == x.Id)));

        var command = new DeleteUserInquiryRequest.Command(userInquiryEntity.Id);

        // Act
        _ = await _handler.Handle(command, _cancellationToken);

        // Assert
        commentsList.Should().HaveCount(1);
        acceptsList.Should().HaveCount(0);
    }

    [Fact]
    public async Task HandleWhenUserDeleteClosedInquiryShouldBeFailed()
    {
        // Arrange
        var userId = InquiryRequestTestsHelper.AuthenticatedUserId;
        var userDto = CreateUserDto(userId);
        var (userInquiryEntity, userInquiryDto) = CreateUserInquiry(userDto, UserInquiryStatus.Rejected);
        MockInquiriesDbContext([userInquiryEntity], [], []);

        var command = new DeleteUserInquiryRequest.Command(userInquiryEntity.Id);

        // Act & Assert
        await Assert.ThrowsAsync<ActionNotAllowedException>(async () =>
            await _handler.Handle(command, _cancellationToken));
    }

    [Fact]
    public async Task HandleWhenUserDeleteNotAllowedInquiryShouldBeFailed()
    {
        // Arrange
        var userId = InquiryRequestTestsHelper.AuthenticatedUserId;
        var userDto = CreateUserDto(userId);
        var (userInquiryEntity, userInquiryDto) = CreateUserInquiry(userDto, UserInquiryStatus.Open);
        MockInquiriesDbContext([userInquiryEntity], [], []);
        _hasUserAccessToMutateInquiryBusinessRule.Setup(x => x.IsCorrespondsToBusiness(userId, _cancellationToken))
            .ReturnsAsync(false);

        var command = new DeleteUserInquiryRequest.Command(userInquiryEntity.Id);

        // Act & Assert
        await Assert.ThrowsAsync<ActionNotAllowedException>(async () =>
            await _handler.Handle(command, _cancellationToken));
    }

    [Fact]
    public async Task HandleWhenUserDeleteNotHisOwnInquiryShouldBeFailed()
    {
        // Arrange
        var userId = 2;
        var userDto = CreateUserDto(userId);
        var (userInquiryEntity, userInquiryDto) = CreateUserInquiry(userDto, UserInquiryStatus.Open);
        MockInquiriesDbContext([userInquiryEntity], [], []);
        _hasUserAccessToMutateInquiryBusinessRule.Setup(x => x.IsCorrespondsToBusiness(userId, _cancellationToken))
            .ReturnsAsync(true);

        var command = new DeleteUserInquiryRequest.Command(userInquiryEntity.Id);

        // Act & Assert
        await Assert.ThrowsAsync<ActionNotAllowedException>(async () =>
            await _handler.Handle(command, _cancellationToken));
    }

    private InquiriesUserDto CreateUserDto(int userId)
    {
        return new InquiriesUserDto
        {
            Id = userId,
            FirstName = "First",
            LastName = "Last",
            Email = "Test@email.com@",
            DepartmentId = "Department",
            ResourceGroupId = 1,
            RegisteredLocationId = 1,
            AvatarFileId = "Avatar",
            Color = "Color",
            Position = "Position"
        };
    }

    private (UserInquiryEntity entity, UserInquiryCoreDto dto) CreateUserInquiry(InquiriesUserDto user,
        UserInquiryStatus status)
    {
        var inquiryTemplate = new InquiryTemplateEntity { Id = 1, Name = "Template 1" };
        return (
            new UserInquiryEntity
            {
                Id = 1,
                UserId = user.Id,
                Status = status,
                User = new InquiriesUserEntity
                {
                    UserId = user.Id,
                    DepartmentId = user.DepartmentId,
                    ResourceGroupId = user.ResourceGroupId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                },
                UserInquiryAccepts = [new UserInquiryAcceptEntity { Id = 1, AcceptUserId = 1 }],
                InquiryTemplate = inquiryTemplate,
            },
            new UserInquiryCoreDto
            {
                Id = 1,
                InquiryTemplateName = inquiryTemplate.Name,
                InquiryType = nameof(InquiryType.SickDay),
                CreatedByUser = user,
                CreatedForUser = user
            });
    }

    private UserInquiryAcceptEntity CreateUserInquiryAcceptEntity(InquiriesUserDto userDto,
        UserInquiryEntity userInquiry)
    {
        return new UserInquiryAcceptEntity
        {
            CreatedById = userDto.Id,
            CreatedAtUtc = DateTime.Now,
            UpdatedById = null,
            UpdatedAtUtc = null,
            Id = 1,
            UserInquiryId = userInquiry.Id,
            UserInquiryAgreementAssign = null,
            Status = UserInquiryAcceptStatus.Pending,
        };
    }

    private void MockInquiriesDbContext(
        IEnumerable<UserInquiryEntity> userInquiries,
        IEnumerable<UserInquiryAcceptEntity> userInquiryAccept,
        IEnumerable<UserInquiryCommentEntity> userInquiryComments
    )
    {
        _inquiriesDbContext.Setup(x => x.UserInquiries).Returns(userInquiries.ToArray().BuildMockDbSet().Object);
        _inquiriesDbContext.Setup(x => x.UserInquiryAccepts).Returns(userInquiryAccept.ToArray().BuildMockDbSet().Object);
        _inquiriesDbContext.Setup(x => x.UserInquiryComments).Returns(userInquiryComments.ToArray().BuildMockDbSet().Object);
        _inquiriesDbContext.Setup(x => x.UserInquiryComments).Returns(userInquiryComments.ToArray().BuildMockDbSet().Object);
    }
}
