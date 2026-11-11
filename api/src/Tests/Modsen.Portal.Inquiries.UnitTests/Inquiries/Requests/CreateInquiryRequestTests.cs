using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Dto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Builders;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Loaders;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Validators;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Requests
{
    public class CreateInquiryRequestTests
    {
        private readonly Mock<IAuthenticatedUserContext> _userContextMock = new();
        private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
        private readonly Mock<IUserInquiryBuilder> _userInquiryBuilderMock = new();
        private readonly Mock<IUserInquiryLoader> _userInquiryLoaderMock = new();
        private readonly Mock<INotificationSender> _notificationSenderMock = new();
        private readonly Mock<IUserAgreementBuilder> _userAgreementBuilderMock = new();
        private readonly Mock<ICreateUserInquiryAgreementRequestValidator> _createUserInquiryAgreementRequestValidatorMock = new();
        private readonly Mock<IIsAgreementValidForInquiryTemplateBusinessRule> _isAgreementValidBusinessRuleMock = new();
        private readonly Mock<IInquiryRejectedStatusResolver> _inquiryRejectedStatusResolver = new();
        private readonly Mock<INextApproversToNotifyResolver> _nextApproversToNotifyResolver = new();

        private readonly CreateUserInquiryAgreementRequest _handler;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public CreateInquiryRequestTests()
        {
            _handler = new CreateUserInquiryAgreementRequest(
                _inquiriesDbContextMock.Object,
                _userInquiryBuilderMock.Object,
                _userInquiryLoaderMock.Object,
                _notificationSenderMock.Object,
                _userAgreementBuilderMock.Object,
                _createUserInquiryAgreementRequestValidatorMock.Object,
                _isAgreementValidBusinessRuleMock.Object,
                _inquiryRejectedStatusResolver.Object,
                _nextApproversToNotifyResolver.Object
            );

            _userContextMock.SetupUserContext(true, true);
        }

        [Fact]
        public async Task HandleWhenUserIsInactiveShouldThrowEntityNotFoundException()
        {
            // Arrange
            var userId = 1;
            var user = new InquiriesUserEntity
            {
                UserId = userId,
                Email = "email",
                FirstName = "name",
                LastName = "name",
                IsActive = false
            };

            _inquiriesDbContextMock
                .SetupGet(x => x.InquiriesUsers)
                .Returns(new List<InquiriesUserEntity>() { user }.BuildMockDbSet().Object);

            var command = new CreateUserInquiryAgreementRequest.Command
            {
                AgreementTemplateId = null,
                DateFrom = DateOnly.FromDateTime(DateTime.UtcNow),
                DateTo = DateOnly.FromDateTime(DateTime.UtcNow),
                InquiryTemplateId = 1,
                Files = [],
                Hours = null,
                Notes = null,
                UserId = userId,
            };

            // Act
            var act = () => _handler.Handle(command, _cancellationToken);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>().WithMessage(PortalMessages.UserNotFound);
        }

        [Fact]
        public async Task HandleDayInquiryWhenAllDataCorrectWithoutAgreementTemplateShouldCreateSuccessfully()
        {
            // Arrange
            var userInquiry = CreateUserInquiryEntity();
            var inquiryTemplate = CreateInquiryTemplateEntity();
            var agreementTemplate = CreateInquiryTemplateAgreementEntity(inquiryTemplate);
            var fileId = Guid.CreateVersion7().ToString();

            MockInquiriesDbContext([userInquiry], [inquiryTemplate], [agreementTemplate], [], [], []);
            MockUserInquiryLoader(inquiryTemplate, userInquiry);

            _isAgreementValidBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                inquiryTemplate.InquiryType,
                inquiryTemplate.Id,
                agreementTemplate.Id,
                _cancellationToken)).ReturnsAsync(true);

            _userAgreementBuilderMock.Setup(x => x.Build(It.IsAny<UserAgreementBuilderRequest>(), _cancellationToken))
                .ReturnsAsync(userInquiry);

            var command = new CreateUserInquiryAgreementRequest.Command()
            {
                UserId = userInquiry.UserId,
                InquiryTemplateId = inquiryTemplate.Id,
                AgreementTemplateId = agreementTemplate.Id,
                Notes = "Test inquiry",
                DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                Files = []
            };

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert
            result.InquiryUserId.Should().Be(userInquiry.Id);
        }

        private UserInquiryEntity CreateUserInquiryEntity()
        {
            return new UserInquiryEntity
            {
                Id = 1,
                UserId = 1,
                User = new InquiriesUserEntity
                {
                    IsActive = true,
                    UserId = 1,
                    DepartmentId = "IT",
                    ResourceGroupId = 100,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                },
                UserInquiryAccepts = [new UserInquiryAcceptEntity { Id = 1, AcceptUserId = 1 }],
                InquiryTemplate = new InquiryTemplateEntity { Id = 1, Name = "Template 1" }
            };
        }

        private InquiryTemplateEntity CreateInquiryTemplateEntity()
        {
            return new InquiryTemplateEntity()
            {
                Id = 1,
                Name = "Template name",
                InquiryType = InquiryType.SickDay,
                TrackingUnitType = TrackingUnit.Days,
                ActivityType = ActivityType.NonWorkingPaid,
                InquiryTypeOrderWeight = 100,
                IsActive = true,
                ProhibitOnProbationPeriod = true,
                RequireAttachments = false,
                IncludeHolidays = true,
                IncludeWeekends = true
            };
        }

        private InquiryTemplateAgreementEntity CreateInquiryTemplateAgreementEntity(
            InquiryTemplateEntity inquiryTemplate)
        {
            return new InquiryTemplateAgreementEntity
            {
                Id = 1,
                Name = "Test agreement",
                DaysPerContractYear = 10,
                IsDefault = false,
                Type = AgreementType.SickDaysAgreement,
                InquiryTemplate = inquiryTemplate,
                InquiryTemplateId = inquiryTemplate.Id,
                UserAgreements = [],
                UserInquiryAgreementAssigns = []
            };
        }

        private void MockInquiriesDbContext(
            IEnumerable<UserInquiryEntity> userInquiries,
            IEnumerable<InquiryTemplateEntity> templates,
            IEnumerable<InquiryTemplateAgreementEntity> templateAgreements,
            IEnumerable<UserInquiryFileEntity> files,
            IEnumerable<UserInquiryAcceptEntity> accepts,
            IEnumerable<UserAgreementEntity> agreements
        )
        {
            var inquiriesUsers = userInquiries.Select(y => y.User);
            _inquiriesDbContextMock.Setup(x => x.UserInquiries).Returns(userInquiries.ToArray().BuildMockDbSet().Object);
            _inquiriesDbContextMock.Setup(x => x.InquiriesUsers).Returns(inquiriesUsers.ToArray().BuildMockDbSet().Object);
            _inquiriesDbContextMock.Setup(x => x.InquiryTemplates).Returns(templates.ToArray().BuildMockDbSet().Object);
            _inquiriesDbContextMock.Setup(x => x.InquiryTemplateAgreements)
                .Returns(templateAgreements.ToArray().BuildMockDbSet().Object);
            _inquiriesDbContextMock.Setup(x => x.UserInquiryFiles).Returns(files.ToArray().BuildMockDbSet().Object);
            _inquiriesDbContextMock.Setup(x => x.UserInquiryAccepts).Returns(accepts.ToArray().BuildMockDbSet().Object);
            _inquiriesDbContextMock.Setup(x => x.UserAgreements).Returns(agreements.ToArray().BuildMockDbSet().Object);
        }

        private void MockUserInquiryLoader(InquiryTemplateEntity inquiryTemplate, UserInquiryEntity userInquiry)
        {
            _userInquiryLoaderMock.Setup(x => x.Load(It.IsAny<int>(), _cancellationToken)).ReturnsAsync(
                new UserInquiryCoreDto
                {
                    Id = 1,
                    InquiryUserId = userInquiry.UserId,
                    AgreementTemplateId = userInquiry.AgreementTemplateId,
                    InquiryTemplateId = inquiryTemplate.Id,
                    InquiryTemplateName = inquiryTemplate.Name,
                    InquiryType = inquiryTemplate.InquiryType.ToString(),
                    InquiryTypeOrderWeight = inquiryTemplate.InquiryTypeOrderWeight,
                    IsAgreement = true,
                    Status = UserInquiryStatus.Open,
                    StatusOrderWeight = 100,
                    Notes = userInquiry.Notes,
                    DateFrom = userInquiry.DateFrom,
                    DateTo = userInquiry.DateTo,
                    TotalHours = null,
                    TotalDays = userInquiry.DateTo.DayNumber - userInquiry.DateFrom.DayNumber,
                    TotalAccountantDays = 25,
                    CompensationAmount = 25,
                    CountryId = userInquiry.CountryId,
                    CreatedById = userInquiry.UserId,
                    CreatedAtUtc = DateTime.Now,
                    UpdatedById = 1,
                    UpdatedAtUtc = DateTime.Now,
                    CreatedByUser = new InquiriesUserDto
                    {
                        Id = userInquiry.User.UserId,
                        FirstName = userInquiry.User.FirstName,
                        LastName = userInquiry.User.LastName,
                        Email = userInquiry.User.Email,
                        DepartmentId = userInquiry.User.DepartmentId,
                        ResourceGroupId = userInquiry.User.ResourceGroupId,
                        RegisteredLocationId = userInquiry.User.RegisteredLocationId,
                        AvatarFileId = userInquiry.User.AvatarFileId,
                        Color = userInquiry.User.Color,
                        Position = userInquiry.User.Position
                    },
                    CreatedForUser = new InquiriesUserDto
                    {
                        Id = 2,
                        FirstName = null,
                        LastName = null,
                        Email = null,
                        DepartmentId = null,
                        ResourceGroupId = null,
                        RegisteredLocationId = null,
                        AvatarFileId = null,
                        Color = null,
                        Position = null
                    }
                });
        }
    }
}
