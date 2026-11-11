using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Assigns.Requests
{
    public class GetUsersByTemplateLookupRequestTests
    {
        private readonly Mock<IInquiriesDbContext> _dbContextMock = new();

        private readonly GetUsersByTemplateLookupRequest _handler;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public GetUsersByTemplateLookupRequestTests()
        {
            _handler = new GetUsersByTemplateLookupRequest(
                _dbContextMock.Object
            );
        }


     [Fact]
    public async Task HandleShouldThrowEntityNotFoundExceptionWhenTemplateNotFound()
        {
            // Arrange
            var query = new GetUsersByTemplateLookupRequest.Query(1, null, null);
            _dbContextMock.Setup(x => x.InquiryTemplates)
                          .Returns((new List<InquiryTemplateEntity>()).BuildMockDbSet().Object);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task HandleShouldReturnAssignedUsersFromUserInquiryTemplatesWhenTemplateIsNormal()
        {
            // Arrange
            var template = new InquiryTemplateEntity()
            {
                Id = 1,
                Name ="Test",
                InquiryType = Portal.Core.Inquiries.InquiryType.OtherType,
                IsActive = true,
            };
            var templates = new List<InquiryTemplateEntity> { template };
            _dbContextMock.Setup(x => x.InquiryTemplates)
                          .Returns(templates.BuildMockDbSet().Object);

            var assignedUser = new UserInquiryTemplateEntity
            {
                InquiryTemplateId = 1,
                AssignedToUser = new InquiriesUserEntity { UserId = 10, FirstName = "John", LastName = "Doe", Email = "test1@mail.ru", IsActive = true }
            };

            var userInquiryTemplates = new List<UserInquiryTemplateEntity> { assignedUser };
            _dbContextMock.Setup(x => x.UserInquiryTemplates)
                          .Returns(userInquiryTemplates.BuildMockDbSet().Object);

            var allUsers = new List<InquiriesUserEntity>
            {
                new InquiriesUserEntity
                {
                    UserId = 10,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test1@mail.ru",
                    IsActive = true
                },
                new InquiriesUserEntity
                {
                    UserId = 20,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "test2@mail.ru",
                    IsActive = true
                }
            };
            _dbContextMock.Setup(x => x.InquiriesUsers)
                          .Returns(allUsers.BuildMockDbSet().Object);

            var query = new GetUsersByTemplateLookupRequest.Query(1, null, null);

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.Single(result.AssignedUsers);
            Assert.Single(result.UnassignedUsers);
            Assert.Equal(10, result.AssignedUsers.First().Id);
            Assert.Equal(20, result.UnassignedUsers.First().Id);
        }

        [Fact]
        public async Task HandleShouldReturnAssignedUsersFromUserAgreementsWhenTemplateIsVacation()
        {
            // Arrange
            var template = new InquiryTemplateEntity
            {
                Id = 1,
                Name = "Test",
                InquiryType = Portal.Core.Inquiries.InquiryType.Vacation,
                IsActive = true,
            };

            _dbContextMock.Setup(x => x.InquiryTemplates)
                          .Returns((new List<InquiryTemplateEntity> { template }).BuildMockDbSet().Object);

            var agreement = new UserAgreementEntity
            {
                InquiryAgreementTemplateId = template.Id,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = template.Id },
                User = new InquiriesUserEntity { UserId = 30, FirstName = "Alex", LastName = "Brown", Email = "test1@mail.ru", IsActive = true }
            };

            _dbContextMock.Setup(x => x.UserAgreements)
                          .Returns((new List<UserAgreementEntity> { agreement }).BuildMockDbSet().Object);

            var agreementTemplate = new InquiryTemplateAgreementEntity()
            {
                Id = 1,
                InquiryTemplate = template,
                InquiryTemplateId = template.Id,
            };

            _dbContextMock.Setup(x => x.InquiryTemplateAgreements)
                          .Returns((new List<InquiryTemplateAgreementEntity> { agreementTemplate }).BuildMockDbSet().Object);

            _dbContextMock.Setup(x => x.UserInquiryTemplates)
                          .Returns((new List<UserInquiryTemplateEntity>()).BuildMockDbSet().Object);

            _dbContextMock.Setup(x => x.InquiriesUsers)
                          .Returns((new List<InquiriesUserEntity>()).BuildMockDbSet().Object);

            var query = new GetUsersByTemplateLookupRequest.Query(1, 1, null);

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.Single(result.AssignedUsers);
            Assert.Equal(30, result.AssignedUsers.First().Id);
        }

        [Fact]
        public async Task HandleShouldFilterUsersByFullname()
        {
            // Arrange
            var template = new InquiryTemplateEntity
            {
                Id = 1,
                Name = "Test",
                InquiryType = Portal.Core.Inquiries.InquiryType.OtherType,
                IsActive = true,
            };
            _dbContextMock.Setup(x => x.InquiryTemplates)
                          .Returns((new List<InquiryTemplateEntity> { template }).BuildMockDbSet().Object);

            var assignedUser = new UserInquiryTemplateEntity
            {
                InquiryTemplateId = 1,
                AssignedToUser = new InquiriesUserEntity { UserId = 10, FirstName = "John", LastName = "Doe", Email = "test1@mail.ru", IsActive = true }
            };

            _dbContextMock.Setup(x => x.UserInquiryTemplates)
                          .Returns(new List<UserInquiryTemplateEntity> { assignedUser }.BuildMockDbSet().Object);

            _dbContextMock.Setup(x => x.InquiriesUsers)
                          .Returns((new List<InquiriesUserEntity>()).BuildMockDbSet().Object);

            var query = new GetUsersByTemplateLookupRequest.Query(1, null, "john");

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            Assert.Single(result.AssignedUsers);
            Assert.Equal("Doe John", result.AssignedUsers.First().FullName);
        }
    }
}
