using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.AuthenticationFilters;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Conversations.Core
{
    public class ConversationsAuthenticationFilterTests
    {
        private readonly ConversationsAuthenticationFilter _filter;
        private readonly Mock<IAuthenticatedUserContext> _mockAuthenticatedUserContext;

        public ConversationsAuthenticationFilterTests()
        {
            _mockAuthenticatedUserContext = new Mock<IAuthenticatedUserContext>();
            _filter = new ConversationsAuthenticationFilter(_mockAuthenticatedUserContext.Object);
        }

        [Fact]
        public void ApplyWhenUserIsHeadOfDepartmentShouldReturnCorrectConversations()
        {
            // Arrange
            var conversations = new List<Conversation>
            {
                new() {
                    User = new TeamBoardUser
                    {
                        DepartmentId = "Net",
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.Conversation
                },
                new() {
                    User = new TeamBoardUser
                    {
                        DepartmentId = "Net",
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.HRConversation
                },
                new() {
                    User = new TeamBoardUser
                    {
                        DepartmentId = "Java",
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.PerformanceReview
                },
            }.AsQueryable();

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.DepartmentId) .Returns("Net");
            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRoles()).Returns(["HeadOfDepartment"]);
            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);
            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRequiredUserId()).Returns(1);

            // Act
            var result = _filter.Apply(conversations);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public void ApplyWhenUserIsGroupManagerShouldReturnCorrectConversations()
        {
            // Arrange
            var conversations = new List<Conversation>
            {
                new() {
                    User = new TeamBoardUser
                    {
                        ResourceGroupId = 1,
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.Conversation
                },
                new() {
                    User = new TeamBoardUser
                    {
                        ResourceGroupId = 1,
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.HRConversation
                },
                new() {
                    User = new TeamBoardUser
                    {
                        ResourceGroupId = 2,
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.PerformanceReview
                },
            }.AsQueryable();

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.ResourceGroupId).Returns(1);
            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRoles()).Returns(["GroupManager"]);
            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);
            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRequiredUserId()).Returns(1);

            // Act
            var result = _filter.Apply(conversations);

            // Assert
            result.Should().HaveCount(2); 
        }

        [Fact]
        public void ApplyWhenUserIsHrShouldReturnHRConversations()
        {
            // Arrange
            var conversations = new List<Conversation>
            {
                new() {
                    User = new TeamBoardUser
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.HRConversation
                },
                new() {
                    User = new TeamBoardUser
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.Conversation
                },
                new() {
                    User = new TeamBoardUser
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.HRConversation
                },
            }.AsQueryable();

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRoles()).Returns(["Employee"]);
            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(true);
            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRequiredUserId()).Returns(1);

            // Act
            var result = _filter.Apply(conversations);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public void ApplyWhenUserIsEmployeeShouldReturnEmptyList()
        {
            // Arrange
            var conversations = new List<Conversation>
            {
                new() {
                    User = new TeamBoardUser
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.HRConversation
                },
                new() {
                    User = new TeamBoardUser
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.Conversation
                },
            }.AsQueryable();

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRoles()).Returns(["Employee"]);
            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

            // Act
            var result = _filter.Apply(conversations);

            // Assert
            result.Should().HaveCount(0);
        }

        [Fact]
        public void ApplyWhenUserIsSystemAdministratorShouldReturnEmptyList()
        {
            // Arrange
            var conversations = new List<Conversation>
            {
                new() {
                    User = new TeamBoardUser
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.HRConversation
                },
                new() {
                    User = new TeamBoardUser
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test"
                    },
                    ConversationType = ConversationType.Conversation
                },
            }.AsQueryable();

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRoles()).Returns(["SystemAdministrator"]);
            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

            // Act
            var result = _filter.Apply(conversations);

            // Assert
            result.Should().HaveCount(0);
        }

        [Fact]
        public void ApplyWhenUserIsEmployeeWithManageAllInternsShouldReturnOnlyInterns()
        {
            // Arrange
            var conversations = new List<Conversation>
            {
                new()
                {
                    User = new TeamBoardUser
                    {
                        UserId = 1,
                        FirstName = "Intern",
                        LastName = "Test",
                        Email = "intern@test.com",
                        Position = PortalRoleNames.Intern,
                    },
                    ConversationType = ConversationType.Conversation
                },
                new()
                {
                    User = new TeamBoardUser
                    {
                        UserId = 2,
                        FirstName = "Employee",
                        LastName = "Test",
                        Email = "employee@test.com",
                        Position = PortalRoleNames.Employee,
                    },
                    ConversationType = ConversationType.Conversation
                }
            }.AsQueryable();

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRoles()).Returns([PortalRoleNames.Employee]);

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRequiredUserId()).Returns(999);

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.HasClaim(LabBoardClaimTypeNames.ManageAllInterns.Name))
                .Returns(true);

            // Act
            var result = _filter.Apply(conversations);

            // Assert
            result.Should().HaveCount(1);
            result.Single().User!.Position.Should().Be(PortalRoleNames.Intern);
        }

        [Fact]
        public void ApplyWhenUserIsEmployeeWithoutManageAllInternsShouldReturnEmptyList()
        {
            // Arrange
            var conversations = new List<Conversation>
            {
                new()
                {
                    User = new TeamBoardUser
                    {
                        UserId = 1,
                        FirstName = "Intern",
                        LastName = "Test",
                        Email = "intern@test.com",
                        Position = PortalRoleNames.Intern
                    },
                    ConversationType = ConversationType.Conversation
                }
            }.AsQueryable();

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRoles()).Returns(["Employee"]);

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.HasClaim(LabBoardClaimTypeNames.ManageAllInterns.Name))
                .Returns(false);

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRequiredUserId()).Returns(999);

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
                .Returns(false);

            // Act
            var result = _filter.Apply(conversations);

            // Assert
            result.Should().HaveCount(0);
        }

        [Fact]
        public void ApplyWhenUserIsGroupManagerWithManageInternsShouldReturnOnlyInternsFromOwnGroup()
        {
            // Arrange
            var resourceGroupId = 1;

            var conversations = new List<Conversation>
            {
                new()
                {
                    User = new TeamBoardUser
                    {
                        UserId = 1,
                        FirstName = "Intern1",
                        LastName = "Test",
                        Email = "intern1@test.com",
                        ResourceGroupId = resourceGroupId,
                        Position = PortalRoleNames.Intern
                    },
                    ConversationType = ConversationType.Conversation
                },
                new()
                {
                    User = new TeamBoardUser
                    {
                        UserId = 2,
                        FirstName = "Intern2",
                        LastName = "Test",
                        Email = "intern2@test.com",
                        ResourceGroupId = 2,
                        Position = PortalRoleNames.Intern
                    },
                    ConversationType = ConversationType.Conversation
                }
            }.AsQueryable();

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRoles()).Returns(["GroupManager"]);

            _mockAuthenticatedUserContext
                .SetupGet(ctx => ctx.ResourceGroupId)
                .Returns(resourceGroupId);

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.GetRequiredUserId()).Returns(999);

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.HasClaim(LabBoardClaimTypeNames.ManageInterns.Name))
                .Returns(true);

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.HasClaim(LabBoardClaimTypeNames.ManageAllInterns.Name))
                .Returns(false);

            _mockAuthenticatedUserContext
                .Setup(ctx => ctx.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
                .Returns(false);

            // Act
            var result = _filter.Apply(conversations);

            // Assert
            result.Should().HaveCount(1);
            result.Single().User!.UserId.Should().Be(1);
        }
    }
}
