using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Assigns.Requests
{
    public class GetUserAssignsRequestTests
    {

        private readonly Mock<IInquiriesDbContext> _dbContextMock = new();
        private readonly Mock<ICanAccessUserAssignsBusinessRule> _businessRuleMock = new();
        private readonly Mock<IClockService> _clockServiceMock = new();
        private readonly Mock<IUserAgreementInquiryAvailableDaysCalculator> _availableDaysCalculatorMock = new();
        private readonly GetUserAssignsRequest _handler;

        public GetUserAssignsRequestTests()
        {
            _clockServiceMock.Setup(x => x.TodayUtc()).Returns(DateOnly.FromDateTime(DateTime.UtcNow));

            _handler = new GetUserAssignsRequest(
                _dbContextMock.Object,
                _businessRuleMock.Object,
                _clockServiceMock.Object,
                _availableDaysCalculatorMock.Object);
        }

        [Fact]
        public async Task HandleShouldThrowWhenAccessNotAllowed()
        {
            // Arrange
            _businessRuleMock.Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var query = new GetUserAssignsRequest.Query(1);

            // Act & Assert
            await Assert.ThrowsAsync<ActionNotAllowedException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task HandleShouldReturnUserInquiriesWhenAccessAllowed()
        {
            // Arrange
            var accessOptions = AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;
            _businessRuleMock.Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), accessOptions, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var userInquiryTemplates = new List<UserInquiryTemplateEntity>
            {
                new UserInquiryTemplateEntity
                {
                    AssignedToUserId = 1,
                    DateFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                    DateTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                    InquiryTemplateId = 10,
                    InquiryTemplate = new InquiryTemplateEntity
                    {
                        Name = "Test",
                        TrackingUnitType = TrackingUnit.Days,
                        InquiryType = InquiryType.OtherType,
                        InquiryTemplateAgreements = []
                    }
                }
            };

            var userInquiries = new List<UserInquiryEntity>
            {
                new UserInquiryEntity
                {
                    UserId = 1,
                    InquiryTemplateId = 10,
                    Status = UserInquiryStatus.Approved,
                    TotalAccountantDays = 5
                }
            };

            _dbContextMock.Setup(x => x.UserInquiryTemplates).Returns(userInquiryTemplates.BuildMockDbSet().Object);
            _dbContextMock.Setup(x => x.UserInquiries).Returns(userInquiries.BuildMockDbSet().Object);
            _dbContextMock.Setup(x => x.UserAgreements).Returns(Array.Empty<UserAgreementEntity>().BuildMockDbSet().Object);

            var handler = new GetUserAssignsRequest(
                _dbContextMock.Object,
                _businessRuleMock.Object,
                _clockServiceMock.Object,
                _availableDaysCalculatorMock.Object);

            var query = new GetUserAssignsRequest.Query(1);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal(10, result.First().InquiryTemplateId);
            Assert.Equal(5, result.First().TrackingUnitCount);
        }
    }
}
