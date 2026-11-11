using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryAccepts.Core.Resolvers;

public class UpdateUserAsApproverAcceptResolverTests
{
    private readonly UpdateUserAsApproverAcceptResolver _resolver;

    private readonly Mock<IInquiriesDbContext> _dbContext = new();
    private readonly Mock<IUpdateUserInfoInquiryAcceptsResolver> _updateOpenInquiriesAcceptsResolver = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateUserAsApproverAcceptResolverTests()
    {
        _resolver = new(_dbContext.Object, _updateOpenInquiriesAcceptsResolver.Object);
    }

    [Fact]
    public async Task ResolveWhenUserFieldsNotUpdatedShouldNotUpdateInquiryAccepts()
    {
        // Arrange
        var prevPosition = PortalRoleNames.Employee;
        var prevDepartmentId = "Net";
        var isPrevActive = true;
        var prevResourceGroupId = (int?)null;

        var user = new InquiriesUserEntity
        {
            Email = "email",
            FirstName = "First",
            LastName = "Last",
            DepartmentId = prevDepartmentId,
            ResourceGroupId = prevResourceGroupId,
            IsActive = isPrevActive,
            Position = prevPosition
        };

        var request = new UpdateUserAcceptResolverRequest
        {
            User = user,
            PrevDepartmentId = prevDepartmentId,
            PrevPosition = prevPosition,
            PrevResourceGroupId = prevResourceGroupId,
            IsPrevActive = isPrevActive,
        };

        // Act
        var act = () => _resolver.ResolveAsync(request, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
        _dbContext.VerifyGet(x => x.UserInquiries, Times.Never);
        _dbContext.VerifyGet(x => x.InquiryTemplateAcceptPolicies, Times.Never);
        _updateOpenInquiriesAcceptsResolver.Verify(x =>
            x.ResolveAsync(It.IsAny<int[]>(), _cancellationToken),
            Times.Never);
    }

    [Fact]
    public async Task ResolveWhenUserBecomeNonManagerShouldUpdatePrevInquiryAccepts()
    {
        // Arrange
        var prevPosition = PortalRoleNames.GroupManager;
        var prevDepartmentId = "Net";
        var isPrevActive = true;
        var prevResourceGroupId = (int?)null;

        var user = new InquiriesUserEntity
        {
            UserId = 1,
            Email = "email",
            FirstName = "First",
            LastName = "Last",
            DepartmentId = prevDepartmentId,
            ResourceGroupId = 1,
            IsActive = isPrevActive,
            Position = PortalRoleNames.Employee
        };

        var openUserInquiries = new List<UserInquiryEntity>()
        {
            new UserInquiryEntity
            {
                Status = UserInquiryStatus.Open,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity
                    {
                        AcceptUserId = user.UserId, Status = UserInquiryAcceptStatus.WaitingForApprove
                    }


                ]
            },
            new UserInquiryEntity
            {
                Status = UserInquiryStatus.Open,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity { AcceptUserId = user.UserId, Status = UserInquiryAcceptStatus.Pending }


                ]
            },
            new UserInquiryEntity
            {
                Status = UserInquiryStatus.Open,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity
                    {
                        AcceptUserId = user.UserId, Status = UserInquiryAcceptStatus.Accepted
                    }


                ]
            },
            new UserInquiryEntity
            {
                Status = UserInquiryStatus.Open,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity
                    {
                        AcceptUserId = user.UserId, Status = UserInquiryAcceptStatus.Rejected
                    }


                ]
            },
        };

        _dbContext.SetupGet(x => x.UserInquiries)
            .Returns(openUserInquiries.BuildMockDbSet().Object);

        var request = new UpdateUserAcceptResolverRequest
        {
            User = user,
            PrevDepartmentId = prevDepartmentId,
            PrevPosition = prevPosition,
            PrevResourceGroupId = prevResourceGroupId,
            IsPrevActive = isPrevActive,
        };

        var inquiryIdsToVerify = openUserInquiries[0..2].Select(x => x.Id).ToArray();        

        // Act
        var act = () => _resolver.ResolveAsync(request, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
        _dbContext.VerifyGet(x => x.UserInquiries, Times.Once);
        _dbContext.VerifyGet(x => x.InquiryTemplateAcceptPolicies, Times.Never);
        _updateOpenInquiriesAcceptsResolver.Verify(x =>
            x.ResolveAsync(inquiryIdsToVerify, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ResolveWhenUserWasActiveBecomeInactiveShouldUpdatePrevInquiryAccepts()
    {
        // Arrange
        var prevPosition = PortalRoleNames.HeadOfDepartment;
        var prevDepartmentId = "Net";
        var isPrevActive = true;
        var prevResourceGroupId = (int?)null;

        var user = new InquiriesUserEntity
        {
            UserId = 1,
            Email = "email",
            FirstName = "First",
            LastName = "Last",
            DepartmentId = prevDepartmentId,
            ResourceGroupId = 1,
            IsActive = false,
            Position = prevPosition
        };

        var openUserInquiries = new List<UserInquiryEntity>()
        {
            new UserInquiryEntity
            {
                Status = UserInquiryStatus.Open,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity
                    {
                        AcceptUserId = user.UserId, Status = UserInquiryAcceptStatus.WaitingForApprove
                    }


                ]
            },
            new UserInquiryEntity
            {
                Status = UserInquiryStatus.Open,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity { AcceptUserId = user.UserId, Status = UserInquiryAcceptStatus.Pending }


                ]
            },
            new UserInquiryEntity
            {
                Status = UserInquiryStatus.Open,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity
                    {
                        AcceptUserId = user.UserId, Status = UserInquiryAcceptStatus.Accepted
                    }


                ]
            },
            new UserInquiryEntity
            {
                Status = UserInquiryStatus.Open,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity
                    {
                        AcceptUserId = user.UserId, Status = UserInquiryAcceptStatus.Rejected
                    }


                ]
            },
        };

        _dbContext.SetupGet(x => x.UserInquiries)
            .Returns(openUserInquiries.BuildMockDbSet().Object);

        var request = new UpdateUserAcceptResolverRequest
        {
            User = user,
            PrevDepartmentId = prevDepartmentId,
            PrevPosition = prevPosition,
            PrevResourceGroupId = prevResourceGroupId,
            IsPrevActive = isPrevActive,
        };

        var inquiriesToVerify = openUserInquiries[0..2];
        var inquiryIdsToVerify = inquiriesToVerify.Select(x => x.Id).ToArray();

        // Act
        var act = () => _resolver.ResolveAsync(request, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
        _dbContext.VerifyGet(x => x.UserInquiries, Times.Once);
        _dbContext.VerifyGet(x => x.InquiryTemplateAcceptPolicies, Times.Never);
        _updateOpenInquiriesAcceptsResolver.Verify(x => x.ResolveAsync(inquiryIdsToVerify, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ResolveWhenUserBecomeCtoShouldUpdateCorrespondingInquiryAccepts()
    {
        // Arrange
        var prevPosition = PortalRoleNames.SystemAdministrator;
        var prevDepartmentId = "SysAdmin";
        var isPrevActive = true;
        var prevResourceGroupId = (int?)null;
        var templateIds = new List<int>() { 1, 2 };

        var user = new InquiriesUserEntity
        {
            UserId = 1,
            Email = "email",
            FirstName = "First",
            LastName = "Last",
            DepartmentId = null,
            ResourceGroupId = null,
            IsActive = isPrevActive,
            Position = PortalRoleNames.Cto
        };

        var acceptPolicies = new List<InquiryTemplateAcceptPolicyEntity>()
        {
            new InquiryTemplateAcceptPolicyEntity
            {
                InquiryTemplateId = templateIds[0],
                Scope = InquiryAcceptPolicyScopes.CTO | InquiryAcceptPolicyScopes.CEO,
                UserId = null
            },
            new InquiryTemplateAcceptPolicyEntity
            {
                InquiryTemplateId = templateIds[1],
                Scope = InquiryAcceptPolicyScopes.CTO | InquiryAcceptPolicyScopes.CEO,
                UserId = null
            }
        };

        _dbContext.SetupGet(x => x.InquiryTemplateAcceptPolicies)
            .Returns(acceptPolicies.BuildMockDbSet().Object);

        var inquiries = new List<UserInquiryEntity>()
        {
            new UserInquiryEntity
            {
                InquiryTemplateId = templateIds[0],
            },
            new UserInquiryEntity
            {
                InquiryTemplateId = templateIds[0],
            },
            new UserInquiryEntity
            {
                InquiryTemplateId = templateIds[1],
            },
            new UserInquiryEntity
            {
                InquiryTemplateId = 1000,
            }
        };

        _dbContext.SetupGet(x => x.UserInquiries)
            .Returns(inquiries.BuildMockDbSet().Object);

        var request = new UpdateUserAcceptResolverRequest
        {
            User = user,
            PrevDepartmentId = prevDepartmentId,
            PrevPosition = prevPosition,
            PrevResourceGroupId = prevResourceGroupId,
            IsPrevActive = isPrevActive,
        };

        var inquiriesToUpdate = inquiries[0..3];
        var inquiryIdsToUpdate = inquiriesToUpdate.Select(x => x.Id).ToArray();

        // Act
        var act = () => _resolver.ResolveAsync(request, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
        _dbContext.VerifyGet(x => x.UserInquiries, Times.Exactly(2));
        _dbContext.VerifyGet(x => x.InquiryTemplateAcceptPolicies, Times.Once);
        _updateOpenInquiriesAcceptsResolver.Verify(x => x.ResolveAsync(inquiryIdsToUpdate, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ResolveWhenUserBecomeHodShouldUpdateCorrespondingInquiryAccepts()
    {
        // Arrange
        var prevPosition = PortalRoleNames.SystemAdministrator;
        var prevDepartmentId = "SysAdmin";
        var isPrevActive = true;
        var prevResourceGroupId = (int?)null;
        var templateIds = new List<int>() { 1, 2 };

        var user = new InquiriesUserEntity
        {
            UserId = 1,
            Email = "email",
            FirstName = "First",
            LastName = "Last",
            DepartmentId = prevDepartmentId,
            ResourceGroupId = prevResourceGroupId,
            IsActive = isPrevActive,
            Position = PortalRoleNames.HeadOfDepartment
        };

        var acceptPolicies = new List<InquiryTemplateAcceptPolicyEntity>()
        {
            new InquiryTemplateAcceptPolicyEntity
            {
                InquiryTemplateId = templateIds[0],
                Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                UserId = null
            },
            new InquiryTemplateAcceptPolicyEntity
            {
                InquiryTemplateId = templateIds[1],
                Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                UserId = null
            }
        };

        _dbContext.SetupGet(x => x.InquiryTemplateAcceptPolicies)
            .Returns(acceptPolicies.BuildMockDbSet().Object);

        var inquiries = new List<UserInquiryEntity>()
        {
            new UserInquiryEntity
            {
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = prevDepartmentId,
                    Position = PortalRoleNames.SystemAdministrator
                },
                InquiryTemplateId = templateIds[0],
            },
            new UserInquiryEntity
            {
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = prevDepartmentId,
                    Position = PortalRoleNames.GroupManager
                },
                InquiryTemplateId = templateIds[0],
            },
            new UserInquiryEntity
            {
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = prevDepartmentId,
                    Position = PortalRoleNames.Employee
                },
                InquiryTemplateId = templateIds[1],
            },
            new UserInquiryEntity
            {
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = "another",
                    Position = PortalRoleNames.Employee
                },
                InquiryTemplateId = templateIds[1],
            },
            new UserInquiryEntity
            {
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = "another",
                    Position = PortalRoleNames.GroupManager
                },
                InquiryTemplateId = templateIds[1],
            }
        };

        _dbContext.SetupGet(x => x.UserInquiries)
            .Returns(inquiries.BuildMockDbSet().Object);

        var request = new UpdateUserAcceptResolverRequest
        {
            User = user,
            PrevDepartmentId = prevDepartmentId,
            PrevPosition = prevPosition,
            PrevResourceGroupId = prevResourceGroupId,
            IsPrevActive = isPrevActive,
        };

        var inquiriesToUpdate = inquiries[0..3];
        var inquiryIdsToUpdate = inquiriesToUpdate.Select(x => x.Id).ToArray();

        // Act
        var act = () => _resolver.ResolveAsync(request, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
        _dbContext.VerifyGet(x => x.UserInquiries, Times.Exactly(2));
        _dbContext.VerifyGet(x => x.InquiryTemplateAcceptPolicies, Times.Once);
        _updateOpenInquiriesAcceptsResolver.Verify(x => x.ResolveAsync(inquiryIdsToUpdate, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ResolveWhenUserBecomeGmShouldUpdateCorrespondingInquiryAccepts()
    {
        // Arrange
        var prevPosition = PortalRoleNames.Employee;
        var prevDepartmentId = "Net";
        var isPrevActive = true;
        var prevResourceGroupId = 1;
        var templateIds = new List<int>() { 1, 2 };

        var user = new InquiriesUserEntity
        {
            UserId = 1,
            Email = "email",
            FirstName = "First",
            LastName = "Last",
            DepartmentId = prevDepartmentId,
            ResourceGroupId = prevResourceGroupId,
            IsActive = isPrevActive,
            Position = PortalRoleNames.GroupManager
        };

        var acceptPolicies = new List<InquiryTemplateAcceptPolicyEntity>()
        {
            new InquiryTemplateAcceptPolicyEntity
            {
                InquiryTemplateId = templateIds[0],
                Scope = InquiryAcceptPolicyScopes.GroupManager,
                UserId = null
            },
            new InquiryTemplateAcceptPolicyEntity
            {
                InquiryTemplateId = templateIds[1],
                Scope = InquiryAcceptPolicyScopes.GroupManager,
                UserId = null
            }
        };

        _dbContext.SetupGet(x => x.InquiryTemplateAcceptPolicies)
            .Returns(acceptPolicies.BuildMockDbSet().Object);

        var inquiries = new List<UserInquiryEntity>()
        {
            new UserInquiryEntity
            {
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = prevDepartmentId,
                    ResourceGroupId = prevResourceGroupId,
                    Position = PortalRoleNames.Employee
                },
                InquiryTemplateId = templateIds[0],
            },
            new UserInquiryEntity
            {
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = prevDepartmentId,
                    Position = PortalRoleNames.GroupManager
                },
                InquiryTemplateId = templateIds[0],
            },
            new UserInquiryEntity
            {
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = "Another",
                    Position = PortalRoleNames.Employee
                },
                InquiryTemplateId = templateIds[1],
            },
            new UserInquiryEntity
            {
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = prevDepartmentId,
                    Position = PortalRoleNames.HeadOfDepartment
                },
                InquiryTemplateId = templateIds[1],
            }
        };

        _dbContext.SetupGet(x => x.UserInquiries)
            .Returns(inquiries.BuildMockDbSet().Object);

        var request = new UpdateUserAcceptResolverRequest
        {
            User = user,
            PrevDepartmentId = prevDepartmentId,
            PrevPosition = prevPosition,
            PrevResourceGroupId = prevResourceGroupId,
            IsPrevActive = isPrevActive,
        };

        var inquiriesToUpdate = inquiries[0..1];
        var inquiryIdsToUpdate = inquiriesToUpdate.Select(x => x.Id).ToArray();

        // Act
        var act = () => _resolver.ResolveAsync(request, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
        _dbContext.VerifyGet(x => x.UserInquiries, Times.Exactly(2));
        _dbContext.VerifyGet(x => x.InquiryTemplateAcceptPolicies, Times.Once);
        _updateOpenInquiriesAcceptsResolver.Verify(x => x.ResolveAsync(inquiryIdsToUpdate, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ResolveWhenUserWasGmBecomeHodShouldUpdateOpenAndCorrespondingInquiryAccepts()
    {
        // Arrange
        var prevPosition = PortalRoleNames.GroupManager;
        var prevDepartmentId = "SysAdmin";
        var isPrevActive = true;
        var prevResourceGroupId = 1;
        var templateIds = new List<int>() { 1, 2 };

        var user = new InquiriesUserEntity
        {
            UserId = 1,
            Email = "email",
            FirstName = "First",
            LastName = "Last",
            DepartmentId = prevDepartmentId,
            ResourceGroupId = null,
            IsActive = isPrevActive,
            Position = PortalRoleNames.HeadOfDepartment
        };

        var openUserInquiries = new List<UserInquiryEntity>()
        {
            new UserInquiryEntity
            {
                Id = 1,
                Status = UserInquiryStatus.Open,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity
                    {
                        AcceptUserId = user.UserId, Status = UserInquiryAcceptStatus.WaitingForApprove
                    }


                ]
            },
            new UserInquiryEntity
            {
                Id = 2,
                Status = UserInquiryStatus.Open,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity { AcceptUserId = user.UserId, Status = UserInquiryAcceptStatus.Pending }


                ]
            },
            new UserInquiryEntity
            {
                Id = 3,
                Status = UserInquiryStatus.Open,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity
                    {
                        AcceptUserId = user.UserId, Status = UserInquiryAcceptStatus.Accepted
                    }


                ]
            },
            new UserInquiryEntity
            {
                Id = 4,
                Status = UserInquiryStatus.Open,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity
                    {
                        AcceptUserId = user.UserId, Status = UserInquiryAcceptStatus.Rejected
                    }


                ]
            },
        };

        var acceptPolicies = new List<InquiryTemplateAcceptPolicyEntity>()
        {
            new InquiryTemplateAcceptPolicyEntity
            {
                InquiryTemplateId = templateIds[0],
                Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                UserId = null
            },
            new InquiryTemplateAcceptPolicyEntity
            {
                InquiryTemplateId = templateIds[1],
                Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                UserId = null
            }
        };

        
        var inquiries = new List<UserInquiryEntity>()
        {
            new UserInquiryEntity
            {
                Id = 5,
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = prevDepartmentId,
                    Position = PortalRoleNames.SystemAdministrator
                },
                InquiryTemplateId = templateIds[0],
            },
            new UserInquiryEntity
            {
                Id = 6,
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = prevDepartmentId,
                    Position = PortalRoleNames.GroupManager
                },
                InquiryTemplateId = templateIds[0],
            },
            new UserInquiryEntity
            {
                Id = 7,
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = prevDepartmentId,
                    Position = PortalRoleNames.Employee
                },
                InquiryTemplateId = templateIds[1],
            },
            new UserInquiryEntity
            {
                Id = 8,
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = "another",
                    Position = PortalRoleNames.Employee
                },
                InquiryTemplateId = templateIds[1],
            },
            new UserInquiryEntity
            {
                Id = 9,
                User = new InquiriesUserEntity
                {
                    Email = "email",
                    FirstName = "First",
                    LastName = "Last",
                    DepartmentId = "another",
                    Position = PortalRoleNames.GroupManager
                },
                InquiryTemplateId = templateIds[1],
            }
        };

        _dbContext.SetupGet(x => x.InquiryTemplateAcceptPolicies)
            .Returns(acceptPolicies.BuildMockDbSet().Object);

        inquiries.AddRange(openUserInquiries);

        _dbContext.SetupGet(x => x.UserInquiries)
            .Returns(inquiries.BuildMockDbSet().Object);

        var request = new UpdateUserAcceptResolverRequest
        {
            User = user,
            PrevDepartmentId = prevDepartmentId,
            PrevPosition = prevPosition,
            PrevResourceGroupId = prevResourceGroupId,
            IsPrevActive = isPrevActive,
        };

        var inquiryIdsToUpdate = new[] { 1, 2, 5, 6, 7 };

        // Act
        var act = () => _resolver.ResolveAsync(request, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
        _dbContext.VerifyGet(x => x.UserInquiries, Times.Exactly(2));
        _dbContext.VerifyGet(x => x.InquiryTemplateAcceptPolicies, Times.Once);
        _updateOpenInquiriesAcceptsResolver.Verify(x =>
            x.ResolveAsync(inquiryIdsToUpdate, _cancellationToken),            
            Times.Once);
    }
}
