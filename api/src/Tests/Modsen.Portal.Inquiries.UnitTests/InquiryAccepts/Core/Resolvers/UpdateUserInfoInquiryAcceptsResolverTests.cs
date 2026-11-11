using System.Collections;
using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryAccepts.Core.Resolvers;

public class UpdateUserInfoInquiryAcceptsResolverTests
{
    private readonly UpdateUserInfoInquiryAcceptsResolver _resolver;
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContext = new();
    private readonly Mock<IInquiryAcceptsComposer> _inquiryAcceptsComposer = new();
    private readonly Mock<IInquiryAcceptedStatusResolver> _inquiryAcceptedStatusResolver = new();
    private readonly Mock<IInquiryRejectedStatusResolver> _inquiryRejectedStatusResolver = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateUserInfoInquiryAcceptsResolverTests()
    {
        _resolver = new(_inquiriesDbContext.Object, _inquiryAcceptsComposer.Object,
            _inquiryAcceptedStatusResolver.Object, _inquiryRejectedStatusResolver.Object);
    }

    [Theory]
    [ClassData(typeof(UpdateUserInfoInquiryAcceptsResolverTestData))]
    public async Task ResolveShouldResolveValid(List<object> inquiryObjects, List<object> composedAcceptObjects,
        List<object> expectedAcceptObjects)
    {
        // Arrange
        var inquiries = inquiryObjects.Cast<UserInquiryEntity>().ToList();
        var inquiry = inquiries.First();
        var composedAccepts = composedAcceptObjects.Cast<UserInquiryAcceptEntity>().ToList();
        var expectedAccepts = expectedAcceptObjects.Cast<UserInquiryAcceptEntity>().ToList();

        _inquiriesDbContext.SetupGet(x => x.UserInquiries).Returns(inquiries.BuildMockDbSet().Object);

        _inquiriesDbContext.SetupGet(x => x.UserInquiryAccepts)
            .Returns(inquiry.UserInquiryAccepts.BuildMockDbSet().Object);

        _inquiriesDbContext.Setup(x => x.SaveChangesAsync(_cancellationToken));

        _inquiryAcceptsComposer.Setup(x => x.ComposeAsync(It.IsAny<int>(), It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(composedAccepts);

        _inquiryAcceptedStatusResolver.Setup(x => x.ResolveAsync(inquiry, _cancellationToken));

        // Act
        await _resolver.ResolveAsync([1], _cancellationToken);

        // Assert
        inquiry.UserInquiryAccepts.Should().BeEquivalentTo(expectedAccepts, config => config
            .Including(x => x.AcceptPolicyScope)
            .Including(x => x.AcceptUserId)
            .Including(x => x.Status)
            .WithStrictOrdering());
    }

    public class UpdateUserInfoInquiryAcceptsResolverTestData : IEnumerable<object[]>
    {
        private const int s_userId = 25;
        private const int s_pm1Id = 26;
        private const int s_pm2Id = 27;
        private const int s_gmId = 28;
        private const int s_hodId = 29;
        private const int s_anyUserId = 30;

        public IEnumerator<object[]> GetEnumerator()
        {

            yield return
            [
                new List<object>
                {
                    new UserInquiryEntity
                    {
                        Id = 1,
                        UserId = s_userId,
                        Status = UserInquiryStatus.Open,
                        UserInquiryAccepts =
                        [
                            new()
                            {
                                Id = 1,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
                                Status = UserInquiryAcceptStatus.Accepted,
                                UserInquiryId = 1,
                                AcceptUserId = s_pm1Id,
                            },

                            new()
                            {
                                Id = 2,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                                Status = UserInquiryAcceptStatus.Accepted,
                                UserInquiryId = 1,
                                AcceptUserId = s_hodId,
                            },

                            new()
                            {
                                Id = 3,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                                Status = UserInquiryAcceptStatus.WaitingForApprove,
                                UserInquiryId = 1,
                                AcceptUserId = s_anyUserId,
                            }

                        ],
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name = "Test",
                            InquiryTemplateAcceptPolicyPolicies =
                            [
                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 1, Scope = InquiryAcceptPolicyScopes.ProjectManager,
                                },

                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 2, Scope = InquiryAcceptPolicyScopes.GroupManager,
                                },

                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 3, Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                                },

                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 4,
                                    Scope = InquiryAcceptPolicyScopes.CTO | InquiryAcceptPolicyScopes.CEO,
                                },

                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 5, Scope = InquiryAcceptPolicyScopes.User, UserId = s_anyUserId,
                                }

                            ]
                        }
                    }
                },
                new List<object>
                {
                    new UserInquiryAcceptEntity()
                    {
                        Id = 100,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        Status = UserInquiryAcceptStatus.Pending,
                        UserInquiryId = 1,
                        AcceptUserId = s_pm1Id,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 101,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                        AcceptUserId = s_gmId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 102,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                        AcceptUserId= s_hodId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 103,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        AcceptUserId = s_anyUserId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                },
                new List<object>
                {

                    new UserInquiryAcceptEntity()
                    {
                        Id = 1,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
                        Status = UserInquiryAcceptStatus.Accepted,
                        UserInquiryId = 1,
                        AcceptUserId = s_pm1Id,
                    },
                    new UserInquiryAcceptEntity()
                    {
                        Id = 2,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                        Status = UserInquiryAcceptStatus.Accepted,
                        UserInquiryId = 1,
                        AcceptUserId = s_hodId,
                    },
                    new UserInquiryAcceptEntity()
                    {
                        Id = 3,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        Status = UserInquiryAcceptStatus.WaitingForApprove,
                        UserInquiryId = 1,
                        AcceptUserId = s_anyUserId,
                    },

                }
            ];

            yield return
            [
                new List<object>
                {
                    new UserInquiryEntity
                    {
                        Id = 1,
                        UserId = s_userId,
                        Status = UserInquiryStatus.Open,
                        UserInquiryAccepts =
                        [
                            new()
                            {
                                Id = 1,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
                                Status = UserInquiryAcceptStatus.WaitingForApprove,
                                UserInquiryId = 1,
                                AcceptUserId = s_pm1Id,
                            },

                            new()
                            {
                                Id = 2,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                                Status = UserInquiryAcceptStatus.Pending,
                                UserInquiryId = 1,
                                AcceptUserId = s_hodId,
                            },

                            new()
                            {
                                Id = 3,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                                Status = UserInquiryAcceptStatus.WaitingForApprove,
                                UserInquiryId = 1,
                                AcceptUserId = s_anyUserId,
                            }

                        ],
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name = "Test11",
                            InquiryTemplateAcceptPolicyPolicies =
                            [
                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 1, Scope = InquiryAcceptPolicyScopes.ProjectManager,
                                },

                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 2, Scope = InquiryAcceptPolicyScopes.GroupManager,
                                },

                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 3, Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                                },

                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 4,
                                    Scope = InquiryAcceptPolicyScopes.CTO | InquiryAcceptPolicyScopes.CEO,
                                },

                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 5, Scope = InquiryAcceptPolicyScopes.User, UserId = s_anyUserId,
                                }

                            ]
                        }
                    }
                },
                new List<object>
                {
                    new UserInquiryAcceptEntity()
                    {
                        Id = 100,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
                        Status = UserInquiryAcceptStatus.Pending,
                        UserInquiryId = 1,
                        AcceptUserId = s_pm1Id,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 101,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                        AcceptUserId = s_gmId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 102,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                        AcceptUserId= s_hodId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 103,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        AcceptUserId = s_anyUserId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                },
                new List<object>
                {

                    new UserInquiryAcceptEntity()
                    {
                        Id = 1,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
                        Status = UserInquiryAcceptStatus.WaitingForApprove,
                        UserInquiryId = 1,
                        AcceptUserId = s_pm1Id,
                    },
                    new UserInquiryAcceptEntity()
                    {
                        Id = 1,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                        Status = UserInquiryAcceptStatus.Pending,
                        UserInquiryId = 1,
                        AcceptUserId = s_gmId,
                    },
                    new UserInquiryAcceptEntity()
                    {
                        Id = 2,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                        Status = UserInquiryAcceptStatus.Pending,
                        UserInquiryId = 1,
                        AcceptUserId = s_hodId,
                    },
                    new UserInquiryAcceptEntity()
                    {
                        Id = 3,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        Status = UserInquiryAcceptStatus.Pending,
                        UserInquiryId = 1,
                        AcceptUserId = s_anyUserId,
                    },

                }
            ];

            yield return
            [
                new List<object>
                {
                    new UserInquiryEntity
                    {
                        Id = 1,
                        UserId = s_userId,
                        Status = UserInquiryStatus.Open,
                        UserInquiryAccepts =
                        [
                            new()
                            {
                                Id = 1,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                                Status = UserInquiryAcceptStatus.Accepted,
                                UserInquiryId = 1,
                                AcceptUserId = s_gmId,
                            },

                            new()
                            {
                                Id = 2,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
                                Status = UserInquiryAcceptStatus.Accepted,
                                UserInquiryId = 1,
                                AcceptUserId = s_pm1Id,
                            },

                            new()
                            {
                                Id = 3,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
                                Status = UserInquiryAcceptStatus.WaitingForApprove,
                                UserInquiryId = 1,
                                AcceptUserId = s_pm2Id,
                            },

                            new()
                            {
                                Id = 4,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                                Status = UserInquiryAcceptStatus.Pending,
                                UserInquiryId = 1,
                                AcceptUserId = s_hodId,
                            },

                            new()
                            {
                                Id = 5,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                                Status = UserInquiryAcceptStatus.Pending,
                                UserInquiryId = 1,
                                AcceptUserId = s_anyUserId,
                            }

                        ],
                        InquiryTemplate = new InquiryTemplateEntity
                        {
                            Name = "Test",
                            InquiryTemplateAcceptPolicyPolicies =
                            [
                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 1, Scope = InquiryAcceptPolicyScopes.GroupManager,
                                },

                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 2, Scope = InquiryAcceptPolicyScopes.ProjectManager,
                                },

                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 3, Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                                },

                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 4,
                                    Scope = InquiryAcceptPolicyScopes.CTO | InquiryAcceptPolicyScopes.CEO,
                                },

                                new InquiryTemplateAcceptPolicyEntity()
                                {
                                    OrderWeight = 5, Scope = InquiryAcceptPolicyScopes.User, UserId = s_anyUserId,
                                }

                            ]
                        }
                    }
                },
                new List<object>
                {
                    new UserInquiryAcceptEntity
                    {
                        Id = 99,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                        AcceptUserId = s_gmId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                    new UserInquiryAcceptEntity()
                    {
                        Id = 100,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
                        Status = UserInquiryAcceptStatus.Pending,
                        UserInquiryId = 1,
                        AcceptUserId = s_pm1Id,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 102,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                        AcceptUserId= s_hodId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 103,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        AcceptUserId = s_anyUserId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                },
                new List<object>
                {
                    new UserInquiryAcceptEntity()
                    {
                        Id = 1,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                        Status = UserInquiryAcceptStatus.Accepted,
                        UserInquiryId = 1,
                        AcceptUserId = s_gmId,
                    },
                    new UserInquiryAcceptEntity()
                    {
                        Id = 1,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
                        Status = UserInquiryAcceptStatus.Accepted,
                        UserInquiryId = 1,
                        AcceptUserId = s_pm1Id,
                    },
                    new UserInquiryAcceptEntity()
                    {
                        Id = 2,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                        Status = UserInquiryAcceptStatus.WaitingForApprove,
                        UserInquiryId = 1,
                        AcceptUserId = s_hodId,
                    },
                    new UserInquiryAcceptEntity()
                    {
                        Id = 3,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        Status = UserInquiryAcceptStatus.Pending,
                        UserInquiryId = 1,
                        AcceptUserId = s_anyUserId,
                    },

                }
            ];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
