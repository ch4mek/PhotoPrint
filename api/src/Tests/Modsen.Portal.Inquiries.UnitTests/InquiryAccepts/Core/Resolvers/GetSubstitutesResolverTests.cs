using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Dto;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryAccepts.Core.Resolvers;

public class GetSubstitutesResolverTests
{
    private readonly GetSubstitutesResolver _resolver;

    private readonly Mock<IInquiriesDbContext> _inquiriesDbContext = new();
    private readonly Mock<IIdentityContract> _identityContract = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetSubstitutesResolverTests()
    {
        _resolver = new(_inquiriesDbContext.Object, _identityContract.Object);
    }

    [Fact]
    public async Task ResolveWhenEmptyInputCollectionReturnEmptyCollection()
    {
        // Act
        var result = await _resolver.ResolveAsync([], _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public async Task ResolveReturnValid(object[] dtoObjects, object[] userInquiryObjects, object[] acceptObjects,
        object[] policyObjects, object[] usersManagerObjects, object[] expectedObjects)
    {
        // Arrange
        var inquiryUserNotifyDtos = dtoObjects.OfType<InquiryUserNotifyDto>().ToArray();
        var userInquiries = userInquiryObjects.OfType<UserInquiryEntity>().ToArray();
        var accepts = acceptObjects.OfType<UserInquiryAcceptEntity>().ToArray();
        var policies = policyObjects.OfType<InquiryTemplateAcceptPolicyEntity>().ToArray();
        var usersManagers = usersManagerObjects.OfType<UserWithManagersDto>().ToList();

        var userIdsFor = userInquiries.Select(x => x.UserId).ToArray();

        _inquiriesDbContext.SetupGet(x => x.UserInquiries).Returns(userInquiries.BuildMockDbSet().Object);
        _inquiriesDbContext.SetupGet(x => x.UserInquiryAccepts).Returns(accepts.BuildMockDbSet().Object);
        _inquiriesDbContext.SetupGet(x => x.InquiryTemplateAcceptPolicies).Returns(policies.BuildMockDbSet().Object);
        _identityContract.Setup(x => x.GetUsersWithManagersAsync(userIdsFor, _cancellationToken))
            .ReturnsAsync(usersManagers);

        // Act
        var result = await _resolver.ResolveAsync(inquiryUserNotifyDtos, _cancellationToken);

        // Assert
        var expected = expectedObjects.OfType<InquiryUserNotifyDto>();

        result.Should().BeEquivalentTo(expected, config => config.WithoutStrictOrdering());
    }

    public static IEnumerable<object[]> TestData =>
        [
            // Common case
            [
                new[]
                {
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 1,
                        AcceptUserId = 1,
                        CreatedForUserId = 101
                    },
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 2,
                        AcceptUserId = 2,
                        CreatedForUserId = 102
                    }
                },
                new[]
                {
                    new UserInquiryEntity
                    {
                        Id = 1,
                        InquiryTemplateId = 1,
                        UserId = 101,
                    },
                    new UserInquiryEntity
                    {
                        Id = 2,
                        InquiryTemplateId = 2,
                        UserId = 102
                    }
                },
                new[]
                {
                    new UserInquiryAcceptEntity
                    {
                        Id = 1,
                        UserInquiryId = 1,
                        AcceptUserId = 1,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                        Status = UserInquiryAcceptStatus.WaitingForApprove,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 2,
                        UserInquiryId = 2,
                        AcceptUserId = 2,
                        Status = UserInquiryAcceptStatus.WaitingForApprove,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment
                    }
                },
                new[]
                {
                    new InquiryTemplateAcceptPolicyEntity
                    {
                        Id = 1,
                        InquiryTemplateId = 1,
                        Scope = InquiryAcceptPolicyScopes.GroupManager,
                        SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    },
                    new InquiryTemplateAcceptPolicyEntity
                    {
                        Id = 2,
                        InquiryTemplateId = 2,
                        Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                        SubstituteScope = InquiryAcceptPolicyScopes.User,
                        SubstituteUserId = 501
                    }
                },
                new List<UserWithManagersDto>
                {
                    new UserWithManagersDto
                    {
                        UserId = 101,
                        GroupManagerId = 1,
                        HeadOfDepartmentUserId = 500,
                    },
                    new UserWithManagersDto
                    {
                        UserId = 102,
                        GroupManagerId = 0,
                        HeadOfDepartmentUserId = 2
                    }
                },
                new[]
                {
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 1,
                        AcceptUserId = 500,
                        CreatedForUserId = 101
                    },
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 2,
                        AcceptUserId = 501,
                        CreatedForUserId = 102
                    }
                }
            ],

            // Single Accept User - Multiple inquiries
            [
                new[]
                {
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 1,
                        AcceptUserId = 1,
                        CreatedForUserId = 101
                    },
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 2,
                        AcceptUserId = 1,
                        CreatedForUserId = 102
                    }
                },
                new[]
                {
                    new UserInquiryEntity
                    {
                        Id = 1,
                        InquiryTemplateId = 1,
                        UserId = 101,
                    },
                    new UserInquiryEntity
                    {
                        Id = 2,
                        InquiryTemplateId = 2,
                        UserId = 102
                    }
                },
                new[]
                {
                    new UserInquiryAcceptEntity
                    {
                        Id = 1,
                        UserInquiryId = 1,
                        AcceptUserId = 1,
                        Status = UserInquiryAcceptStatus.WaitingForApprove,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 2,
                        UserInquiryId = 2,
                        AcceptUserId = 1,
                        Status = UserInquiryAcceptStatus.WaitingForApprove,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User
                    }
                },
                new[]
                {
                    new InquiryTemplateAcceptPolicyEntity
                    {
                        Id = 1,
                        InquiryTemplateId = 1,
                        Scope = InquiryAcceptPolicyScopes.GroupManager,
                        SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    },
                    new InquiryTemplateAcceptPolicyEntity
                    {
                        Id = 2,
                        InquiryTemplateId = 2,
                        Scope = InquiryAcceptPolicyScopes.User,
                        UserId = 1,
                        SubstituteScope = InquiryAcceptPolicyScopes.User,
                        SubstituteUserId = 501
                    }
                },
                new List<UserWithManagersDto>
                {
                    new UserWithManagersDto
                    {
                        UserId = 101,
                        GroupManagerId = 1,
                        HeadOfDepartmentUserId = 500,
                    },
                    new UserWithManagersDto
                    {
                        UserId = 102,
                        GroupManagerId = 0,
                        HeadOfDepartmentUserId = 2
                    }
                },
                new[]
                {
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 1,
                        AcceptUserId = 500,
                        CreatedForUserId = 101
                    },
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 2,
                        AcceptUserId = 501,
                        CreatedForUserId = 102
                    }
                }
            ],

            // Single Accept User - Single inquiry
            [
                new[]
                {
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 1,
                        AcceptUserId = 1,
                        CreatedForUserId = 101
                    },
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 1,
                        AcceptUserId = 1,
                        CreatedForUserId = 102
                    }
                },
                new[]
                {
                    new UserInquiryEntity
                    {
                        Id = 1,
                        InquiryTemplateId = 1,
                        UserId = 101,
                    },
                },
                new[]
                {
                    new UserInquiryAcceptEntity
                    {
                        Id = 1,
                        UserInquiryId = 1,
                        AcceptUserId = 1,
                        Status = UserInquiryAcceptStatus.WaitingForApprove,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 2,
                        UserInquiryId = 1,
                        AcceptUserId = 1,
                        Status = UserInquiryAcceptStatus.WaitingForApprove,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User
                    }
                },
                new[]
                {
                    new InquiryTemplateAcceptPolicyEntity
                    {
                        Id = 1,
                        InquiryTemplateId = 1,
                        Scope = InquiryAcceptPolicyScopes.GroupManager,
                        SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    },
                    new InquiryTemplateAcceptPolicyEntity
                    {
                        Id = 2,
                        InquiryTemplateId = 2,
                        Scope = InquiryAcceptPolicyScopes.User,
                        UserId = 1,
                        SubstituteScope = InquiryAcceptPolicyScopes.User,
                        SubstituteUserId = 501
                    }
                },
                new List<UserWithManagersDto>
                {
                    new UserWithManagersDto
                    {
                        UserId = 101,
                        GroupManagerId = 1,
                        HeadOfDepartmentUserId = 500,
                    },
                },
                new[]
                {
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 1,
                        AcceptUserId = 500,
                        CreatedForUserId = 101
                    },
                }
            ],

            //No Substitutes
            [
                new[]
                {
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 1,
                        AcceptUserId = 1,
                        CreatedForUserId = 101
                    },
                    new InquiryUserNotifyDto
                    {
                        UserInquiryId = 2,
                        AcceptUserId = 2,
                        CreatedForUserId = 102
                    }
                },
                new[]
                {
                    new UserInquiryEntity
                    {
                        Id = 1,
                        InquiryTemplateId = 1,
                        UserId = 101,
                    },
                    new UserInquiryEntity
                    {
                        Id = 2,
                        InquiryTemplateId = 2,
                        UserId = 102
                    }
                },
                new[]
                {
                    new UserInquiryAcceptEntity
                    {
                        Id = 1,
                        UserInquiryId = 1,
                        AcceptUserId = 1,
                        Status = UserInquiryAcceptStatus.WaitingForApprove,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 2,
                        UserInquiryId = 2,
                        AcceptUserId = 2,
                        Status = UserInquiryAcceptStatus.WaitingForApprove,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User
                    }
                },
                new[]
                {
                    new InquiryTemplateAcceptPolicyEntity
                    {
                        Id = 1,
                        InquiryTemplateId = 1,
                        Scope = InquiryAcceptPolicyScopes.GroupManager,
                        SubstituteScope = null
                    },
                    new InquiryTemplateAcceptPolicyEntity
                    {
                        Id = 2,
                        InquiryTemplateId = 2,
                        Scope = InquiryAcceptPolicyScopes.User,
                        UserId = 2,
                        SubstituteScope = null
                    }
                },
                new List<UserWithManagersDto> { },
                new InquiryUserNotifyDto[] { }
            ]
        ];
}
