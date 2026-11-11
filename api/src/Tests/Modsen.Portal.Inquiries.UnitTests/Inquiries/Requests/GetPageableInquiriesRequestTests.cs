using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Requests
{
    public class GetPageableInquiriesRequestTests
    {
        private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
        private readonly Mock<IHasUserAccessToInquiryBusinessRule> _hasUserAccessToInquiryBusinessRuleMock = new();
        private readonly Mock<ISetAvailabilityToApproveRejectResolver> _setAvailabilityToApproveRejectResolver = new();

        private readonly GetPageableInquiriesRequest _handler;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public GetPageableInquiriesRequestTests()
        {
            _handler = new GetPageableInquiriesRequest(
                _inquiriesDbContextMock.Object,
                _hasUserAccessToInquiryBusinessRuleMock.Object,
                _setAvailabilityToApproveRejectResolver.Object
            );
        }

        [Fact]
        public async Task HandleShouldThrowEntityNotFoundExceptionWhenAccessDenied()
        {
            // Arrange

            var queryParameters = new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "10",
                [nameof(ODataRawQueryOptions.Skip)] = "0"
            };

            var userId = 7;
            var queryOptions = ODataQueryOptionsMock.BuildMock<UserInquiryCoreDto>(queryParameters);
            var query = new GetPageableInquiriesRequest.Query(userId, queryOptions);

            _hasUserAccessToInquiryBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(query.UserId,
                    AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator, _cancellationToken))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(query, _cancellationToken));
        }

        [Fact]
        public async Task HandleShouldReturnInquiriesWhenRequestIsValid()
        {
            // Arrange
            var userInquiries = new List<UserInquiryEntity>
            {
                new()
                {
                    Id = 1,
                    UserId = 10,
                    AgreementTemplateId = 2,
                    InquiryTemplateId = 3,
                    InquiryTemplate = new InquiryTemplateEntity()
                    {
                        Name = "Name",
                        InquiryTypeOrderWeight = 1,
                        InquiryType = InquiryType.SickLeave,
                    },
                    Status = UserInquiryStatus.Open,
                    StatusOrderWeight = 5,
                    Notes = "Note",
                    DateFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                    DateTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                    TotalDays = 10,
                    TotalHours = 80,
                    TotalAccountantDays = 8,
                    CompensationAmount = 1000.00m,
                    CreatedById = 1,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedById = 2,
                    UpdatedAtUtc = DateTime.UtcNow,
                    CountryId = 1,
                    CreatedByUser = new InquiriesUserEntity()
                    {
                        UserId = 1,
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "TestTest@example.com",
                        DepartmentId = "Net",
                        ResourceGroupId = 1,
                        RegisteredLocationId = 1,
                        AvatarFileId = null,
                        Position = PortalRoleNames.HeadOfDepartment,
                    },
                    User = new InquiriesUserEntity()
                    {
                        UserId = 2,
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "TestTest@example.com",
                        DepartmentId = "Net",
                        ResourceGroupId = 2,
                        RegisteredLocationId = 2,
                        AvatarFileId = null,
                        Position = PortalRoleNames.Employee,
                    },
                    UserInquiryDates =
                    [
                        new UserInquiryDateEntity
                        {
                            Id = 1,
                            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                            RequestedDays = 5,
                            DateType = DateType.WorkingDay,
                        }
                    ],
                    UserInquiryAccepts =
                    [
                        new UserInquiryAcceptEntity()
                        {
                            Id = 1,
                            Status = UserInquiryAcceptStatus.Pending,
                            AcceptUser = new InquiriesUserEntity()
                            {
                                UserId = 3,
                                FirstName = "Test",
                                LastName = "Test",
                        Email = "TestTest@example.com",
                            },
                            AcceptedAtUtc = DateTime.UtcNow
                        }
                    ]
                }
            };

            var queryParameters = new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "10",
                [nameof(ODataRawQueryOptions.Skip)] = "0"
            };

            var userId = 10;
            var queryOptions = ODataQueryOptionsMock.BuildMock<UserInquiryCoreDto>(queryParameters);
            var query = new GetPageableInquiriesRequest.Query(userId, queryOptions);

            _hasUserAccessToInquiryBusinessRuleMock
                .Setup(x => x.IsCorrespondsToBusiness(query.UserId,
                    AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator, _cancellationToken))
                .ReturnsAsync(true);

            _inquiriesDbContextMock
                .Setup(x => x.UserInquiries)
                .Returns(userInquiries.BuildMockDbSet().Object);

            _setAvailabilityToApproveRejectResolver
                .Setup(x => x.ResolveAsync(It.IsAny<List<UserInquiryCoreDto>>(), null, _cancellationToken))
                .ReturnsAsync(
                    (List<UserInquiryCoreDto> inquiries,
                    ICollection<int>? inquiryIdsCanBeApprovedAsSubstitute,
                    CancellationToken cancellationToken) =>
                    inquiries);

            // Act
            var response = await _handler.Handle(query, _cancellationToken);

            // Assert
            response.Total.Should().Be(1);
        }
    }
}
