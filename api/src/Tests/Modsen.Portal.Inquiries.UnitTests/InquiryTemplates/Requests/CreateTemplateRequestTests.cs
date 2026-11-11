using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using MockQueryable.Moq;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryTemplates.Requests
{
    public class CreateTemplateRequestTests
    {
        private readonly Mock<IInquiriesDbContext> _dbContextMock;
        private readonly Mock<IInquiryTemplateLoader> _loaderMock;
        private readonly Mock<IInquiryTemplateValidator> _validatorMock;
        private readonly Mock<IAreAcceptPoliciesValidBusinessRule> _acceptPolicesBusinessRuleMock;

        private readonly CreateTemplateRequest _handler;

        public CreateTemplateRequestTests()
        {
            _dbContextMock = new Mock<IInquiriesDbContext>();
            _loaderMock = new Mock<IInquiryTemplateLoader>();
            _validatorMock = new Mock<IInquiryTemplateValidator>();
            _acceptPolicesBusinessRuleMock = new Mock<IAreAcceptPoliciesValidBusinessRule>();
            var communicationBusMock = new Mock<ICommunicationBus>();

            _handler = new CreateTemplateRequest(
                _dbContextMock.Object,
                _loaderMock.Object,
                _validatorMock.Object,
                _acceptPolicesBusinessRuleMock.Object,
                communicationBusMock.Object);
        }

        [Fact]
        public async Task HandleShouldCreateAndReturnTemplate()
        {
            // Arrange
            var command = new CreateTemplateRequest.Command
            {
                Name = "Vacation Template",
                Type = InquiryType.Vacation,
                ActivityType = ActivityType.NonWorkingUnpaid,
                TrackingUnit = TrackingUnit.Days,
                TransferType = TransferType.Transferred,
                AccrualType = AccrualType.ProportianallyEachMonth,
                Comment = "Some comment",
                IsProhibitOnProbationPeriod = false,
                IsRequireAttachments = true,
                IsIncludeHolidays = true,
                IsIncludeWeekends = false
            };

            var entity = new InquiryTemplateEntity
            {
                Id = 1,
                Name = command.Name
            };

            var dto = new InquiryTemplateDto
            {
                Id = 1,
                Name = command.Name
            };

            var dbSetMock = new List<InquiryTemplateEntity>().BuildMockDbSet();
            _dbContextMock
                .Setup(x => x.InquiryTemplates)
                .Returns(dbSetMock.Object);
            _dbContextMock
                .Setup(x => x.InquiryTemplateAcceptPolicies)
                .Returns(new List<InquiryTemplateAcceptPolicyEntity>().BuildMockDbSet().Object);

            _acceptPolicesBusinessRuleMock
                .Setup(x => x.IsCorrespondsToBusiness(
                    It.IsAny<AreAcceptPoliciesValidBusinessRule.Request>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());

            var internalEntry = new InternalEntityEntry(
                new Mock<IStateManager>().Object,
                new RuntimeEntityType("T", typeof(int), false, null, null, null, ChangeTrackingStrategy.Snapshot, null,
                    false, null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                entity);

            var entityEntry = new Mock<EntityEntry<InquiryTemplateEntity>>(internalEntry);
            entityEntry
                .Setup(e => e.Entity)
                .Returns(entity);

            _dbContextMock
                .Setup(x => x.InquiryTemplates.Add(It.IsAny<InquiryTemplateEntity>()))
                .Returns(entityEntry.Object);

            _dbContextMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _validatorMock
                .Setup(x => x.Validate(It.IsAny<InquiryTemplateEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _loaderMock
                .Setup(x => x.LoadAsync(entity.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(dto);

            _dbContextMock.Verify(x => x.InquiryTemplates.Add(It.IsAny<InquiryTemplateEntity>()), Times.Once);
            _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _validatorMock.Verify(x => x.Validate(It.IsAny<InquiryTemplateEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task HandleShouldThrowWhenTransferTypeIsNullForVacation()
        {
            // Arrange
            var command = new CreateTemplateRequest.Command
            {
                Name = "Vacation Template",
                Type = InquiryType.Vacation,
                TrackingUnit = TrackingUnit.Days,
                ActivityType = ActivityType.NonWorkingUnpaid,
                AccrualType = AccrualType.ProportianallyEachMonth,
                TransferType = null,
                Comment = "Comment" 
            };

            var validator = new CreateTemplateRequest.Validator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task HandleShouldCallValidatorAndSaveChanges()
        {
            // Arrange
            var command = new CreateTemplateRequest.Command
            {
                Name = "Vacation Template",
                Type = InquiryType.Vacation,
                ActivityType = ActivityType.NonWorkingUnpaid,
                TrackingUnit = TrackingUnit.Days,
                TransferType = TransferType.Transferred,
                AccrualType = AccrualType.ProportianallyEachMonth
            };

            var entity = new InquiryTemplateEntity { Id = 99, Name = command.Name };
            var dto = new InquiryTemplateDto { Id = 99, Name = command.Name };

            var dbSetMock = new List<InquiryTemplateEntity>().BuildMockDbSet();
            _dbContextMock
                .Setup(x => x.InquiryTemplates)
                .Returns(dbSetMock.Object);
            _dbContextMock
                .Setup(x => x.InquiryTemplateAcceptPolicies)
                .Returns(new List<InquiryTemplateAcceptPolicyEntity>().BuildMockDbSet().Object);

            _acceptPolicesBusinessRuleMock
                .Setup(x => x.IsCorrespondsToBusiness(
                    It.IsAny<AreAcceptPoliciesValidBusinessRule.Request>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());

            var internalEntry = new InternalEntityEntry(
                new Mock<IStateManager>().Object,
                new RuntimeEntityType("T", typeof(int), false, null, null, null, ChangeTrackingStrategy.Snapshot, null,
                    false, null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                entity);

            var entityEntry = new Mock<EntityEntry<InquiryTemplateEntity>>(internalEntry);
            entityEntry.Setup(e => e.Entity).Returns(entity);

            _dbContextMock
                .Setup(x => x.InquiryTemplates.Add(It.IsAny<InquiryTemplateEntity>()))
                .Returns(entityEntry.Object);

            _validatorMock
                .Setup(x => x.Validate(It.IsAny<InquiryTemplateEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _dbContextMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Verifiable();

            _loaderMock
                .Setup(x => x.LoadAsync(entity.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(dto);

            _validatorMock.Verify();
            _dbContextMock.Verify();
        }
    }
}
