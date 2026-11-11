using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Core;

public class AreAcceptPoliciesValidBusinessRuleTests
{
    private const InquiryAcceptPolicyScopes CeoCtoScope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO;
    private readonly Mock<IInquiriesDbContext> _dbContextMock;
    private readonly AreAcceptPoliciesValidBusinessRule _rule;

    public AreAcceptPoliciesValidBusinessRuleTests()
    {
        _dbContextMock = new Mock<IInquiriesDbContext>();
        _rule = new AreAcceptPoliciesValidBusinessRule(_dbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAcceptPoliciesAreValidReturnSuccessfulBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.GroupManager,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    UserId = null,
                    SubstituteScope = CeoCtoScope,
                    SubstituteUserId = null,
                    OrderWeight = 2,
                }
            ]
        };

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = 1,
                    SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    UserId = null,
                    SubstituteScope = CeoCtoScope,
                    SubstituteUserId = null,
                    OrderWeight = 2,
                }
            ]
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>().BuildMockDbSet().Object);

        // Act
        var act = async () => await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSubstituteUserNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.GroupManager,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = 1,
                    SubstituteScope = CeoCtoScope,
                    SubstituteUserId = null,
                    OrderWeight = 2,
                }
            ]
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>().BuildMockDbSet().Object);

        // Act
        var act = async () => await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAcceptPoliciesAreMoreThanSixReturnFailBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.ProjectManager,
                    UserId = null,
                    SubstituteScope = null,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.GroupManager,
                    UserId = null,
                    SubstituteScope = null,
                    SubstituteUserId = null,
                    OrderWeight = 2,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    UserId = null,
                    SubstituteScope = null,
                    SubstituteUserId = null,
                    OrderWeight = 3,
                },
                new()
                {
                    Scope = CeoCtoScope,
                    UserId = null,
                    SubstituteScope = null,
                    SubstituteUserId = null,
                    OrderWeight = 4,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = 1,
                    SubstituteScope = null,
                    SubstituteUserId = null,
                    OrderWeight = 5,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = 2,
                    SubstituteScope = null,
                    SubstituteUserId = null,
                    OrderWeight = 6,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = 3,
                    SubstituteScope = null,
                    SubstituteUserId = null,
                    OrderWeight = 7,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>()
        {
            new()
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            },
            new()
            {
                UserId = 2,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            },
            new()
            {
                UserId = 3,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            },
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("You cannot add more than 6 approvers");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAcceptPoliciesAreLessThanOneReturnFailBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices = []
        };

        var users = new List<InquiriesUserEntity>();

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("You must add al least 1 approver");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenScopeIsInvalidReturnFailBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.CEO,
                    UserId = null,
                    SubstituteScope = null,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>();

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("Invalid scope value");
    }

    [Theory]
    [InlineData(InquiryAcceptPolicyScopes.CEO)]
    [InlineData(InquiryAcceptPolicyScopes.ProjectManager)]
    public async Task IsCorrespondsToBusinessWhenSubstituteScopeIsInvalidReturnFailBusinessRuleResult(
        InquiryAcceptPolicyScopes invalidSubstituteScope)
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.GroupManager,
                    UserId = null,
                    SubstituteScope = invalidSubstituteScope,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>();

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("Invalid scope value");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsInactiveReturnFailBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = 1,
                    SubstituteScope = InquiryAcceptPolicyScopes.User,
                    SubstituteUserId = 2,
                    OrderWeight = 1,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>()
        {
            new()
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = false,
            },
            new()
            {
                UserId = 2,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            }
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("User is inactive");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSubstituteUserIsInactiveReturnFailBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = 1,
                    SubstituteScope = InquiryAcceptPolicyScopes.User,
                    SubstituteUserId = 2,
                    OrderWeight = 1,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>()
        {
            new()
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            },
            new()
            {
                UserId = 2,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = false,
            }
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("User is inactive");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenRolesDuplicatedReturnFailBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.GroupManager,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.GroupManager,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>();

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("Scope and substitute scope are duplicated");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUsersDuplicatedReturnFailBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = 1,
                    SubstituteScope = InquiryAcceptPolicyScopes.User,
                    SubstituteUserId = 1,
                    OrderWeight = 1,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>()
        {
            new()
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            }
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("Scope and substitute scope are duplicated");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSubstituteScopeIsUserAndUserIdIsNullReturnFailBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.GroupManager,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.User,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>();

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("Accept policy must has substitute user");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSubstituteScopeIsNotUserAndUserIdIsNotNullReturnFailBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.GroupManager,
                    SubstituteUserId = 1,
                    OrderWeight = 1,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>()
        {
            new()
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            }
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("Accept policy must not has substitute user");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSubstituteScopeIsNotAllowedReturnFailBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = false,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.GroupManager,
                    SubstituteUserId = 1,
                    OrderWeight = 1,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>()
        {
            new()
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            }
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("Accept policy must not has substitute scope");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenApprowerScopesAreDuplicatedReturnFailBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.ProjectManager,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.GroupManager,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.ProjectManager,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    SubstituteUserId = null,
                    OrderWeight = 2,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>()
        {
            new()
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            },
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("This approvers are duplicated: ProjectManager");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenApprowersUsersAreDuplicatedReturnFailBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = 1,
                    SubstituteScope = InquiryAcceptPolicyScopes.GroupManager,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = 1,
                    SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    SubstituteUserId = null,
                    OrderWeight = 2,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>()
        {
            new()
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            },
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("This user approvers are duplicated: 1");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSubstituteUsersAreDuplicatedReturnSuccessBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.GroupManager,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.User,
                    SubstituteUserId = 1,
                    OrderWeight = 1,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.ProjectManager,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.User,
                    SubstituteUserId = 1,
                    OrderWeight = 2,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>()
        {
            new()
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            },
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSubstituteScopresAreDuplicatedReturnSuccessBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.GroupManager,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.ProjectManager,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    SubstituteUserId = null,
                    OrderWeight = 2,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>()
        {
            new()
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            },
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenOrderWeightIsDuplicatedReturnSuccessBusinessRuleResult()
    {
        // Arrange
        var request = new AreAcceptPoliciesValidBusinessRule.Request
        {
            IsSubstitutesToApproveAllowed = true,
            AcceptPolices =
            [
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.GroupManager,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                },
                new()
                {
                    Scope = InquiryAcceptPolicyScopes.ProjectManager,
                    UserId = null,
                    SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    SubstituteUserId = null,
                    OrderWeight = 1,
                }
            ]
        };

        var users = new List<InquiriesUserEntity>()
        {
            new()
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "email",
                IsActive = true,
            },
        };

        _dbContextMock
            .Setup(x => x.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("Incorrect order weight");
    }
}
