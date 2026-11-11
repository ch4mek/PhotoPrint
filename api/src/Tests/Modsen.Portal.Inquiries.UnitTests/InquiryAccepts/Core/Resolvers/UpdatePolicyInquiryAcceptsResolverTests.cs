using System.Collections;
using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Dto;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryAccepts.Core.Resolvers;

public class UpdatePolicyInquiryAcceptsResolverTests
{
    private readonly UpdatePolicyInquiryAcceptsResolver _resolver;

    private readonly Mock<IInquiriesDbContext> _inquiriesDbContext = new();
    private readonly Mock<IInquiryAcceptsComposer> _inquiryAcceptsComposer = new();
    private readonly Mock<IInquiryAcceptedStatusResolver> _inquiryAcceptedStatusResolver = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdatePolicyInquiryAcceptsResolverTests()
    {
        _resolver = new(_inquiriesDbContext.Object, _inquiryAcceptsComposer.Object,
            _inquiryAcceptedStatusResolver.Object);
    }

    [Fact]
    public async Task ResolveWhenZeroCreateAcceptsShouldSetAcceptedReturnEmpty()
    {
        // Arrange
        var template = new InquiryTemplateEntity
        {
            Id = 1,
            Name = "Name",
            InquiryTemplateAcceptPolicyPolicies = []
        };

        var inquiries = new List<UserInquiryEntity>
        {
            new UserInquiryEntity
            {
                Id = 1,
                UserId = 1,
                InquiryTemplate = template,
                InquiryTemplateId = template.Id,
                UserInquiryAccepts = [new UserInquiryAcceptEntity() { Id = 1, UserInquiryId = 1 }]
            }
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiryAccepts)
            .Returns(inquiries.SelectMany(x => x.UserInquiryAccepts).ToArray().BuildMockDbSet().Object);

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(inquiries.BuildMockDbSet().Object);

        _inquiryAcceptsComposer.Setup(x => x.ComposeAsync(inquiries[0].UserId, template.Id, _cancellationToken))
            .ReturnsAsync([]);

        // Act
        var result = await _resolver.ResolveAsync(inquiries.Select(x => x.Id).ToArray(), _cancellationToken);

        // Assert        
        _inquiryAcceptsComposer.Verify(x => x.ComposeAsync(inquiries[0].UserId, template.Id, _cancellationToken),
            Times.Once);
        _inquiryAcceptedStatusResolver.Verify(x => x.ResolveAsync(inquiries[0], _cancellationToken), Times.Once);
        _inquiriesDbContext.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ResolveWhenAcceptsAreSameAndApprovedBeforeShouldSetAcceptedReturnEmpty()
    {
        // Arrange
        var template = new InquiryTemplateEntity
        {
            Id = 1,
            Name = "Name",
            InquiryTemplateAcceptPolicyPolicies = [new() { Scope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO }]
        };

        var inquiries = new List<UserInquiryEntity>
        {
            new UserInquiryEntity
            {
                Id = 1,
                UserId = 1,
                InquiryTemplate = template,
                InquiryTemplateId = template.Id,
                UserInquiryAccepts =
                [
                    new() { Id = 1, UserInquiryId = 1, Status = UserInquiryAcceptStatus.Accepted }
                ]
            }
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(inquiries.BuildMockDbSet().Object);

        _inquiriesDbContext.SetupGet(x => x.UserInquiryAccepts)
            .Returns(inquiries.SelectMany(x => x.UserInquiryAccepts).ToArray().BuildMockDbSet().Object);

        var newAccepts = new List<UserInquiryAcceptEntity>()
        {
            new() { Id = 1, UserInquiryId = 1, Status = UserInquiryAcceptStatus.WaitingForApprove }
        };

        _inquiryAcceptsComposer.Setup(x => x.ComposeAsync(inquiries[0].UserId, template.Id, _cancellationToken))
            .ReturnsAsync(newAccepts);

        // Act
        var result = await _resolver.ResolveAsync(inquiries.Select(x => x.Id).ToArray(), _cancellationToken);

        // Assert
        result.Item1.Should().BeEmpty();
        result.Item2.Should().OnlyContain(x => x.UserInquiryId == 1 && x.CreatedForUserId == 1);
        _inquiryAcceptsComposer.Verify(x => x.ComposeAsync(inquiries[0].UserId, template.Id, _cancellationToken),
            Times.Once);
        _inquiryAcceptedStatusResolver.Verify(x => x.ResolveAsync(inquiries[0], _cancellationToken), Times.Once);
        _inquiriesDbContext.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    public class TestDataClass : IEnumerable<object[]>
    {
        private readonly int _hodId = 23;
        private readonly int _gmId = 24;
        private readonly int _ctoId = 25;
        private readonly int _anyUserId = 45;
        private readonly int _userIdInquiryFor = 1000;

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return
            [
                new List<object>
                {
                    new UserInquiryEntity
                    {
                        Id = 1,
                        UserId = _userIdInquiryFor,
                        Status = UserInquiryStatus.Open,
                        UserInquiryAccepts =
                        [
                            new()
                            {
                                Id = 1,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                                Status = UserInquiryAcceptStatus.WaitingForApprove,
                                UserInquiryId = 1,
                                AcceptUserId = _anyUserId,
                            }
                        ],
                    }
                },
                new List<object>
                {
                    new UserInquiryAcceptEntity
                    {
                        Id = 101,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                        AcceptUserId = _gmId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 102,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                        AcceptUserId= _hodId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 103,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        AcceptUserId = _anyUserId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                },
                new List<object>()
            ];

            yield return
            [
                new List<object>()
                {
                    new UserInquiryEntity
                    {
                        Id = 1,
                        UserId = _userIdInquiryFor,
                        Status = UserInquiryStatus.Open,
                        UserInquiryAccepts =
                        [
                            new()
                            {
                                Id = 1,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                                Status = UserInquiryAcceptStatus.WaitingForApprove,
                                AcceptUserId = _gmId,
                                UserInquiryId = 1,
                            },

                            new()
                            {
                                Id = 2,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                                Status = UserInquiryAcceptStatus.Pending,
                                AcceptUserId = _hodId,
                                UserInquiryId = 1,
                            },

                            new()
                            {
                                Id = 3,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                                Status = UserInquiryAcceptStatus.Pending,
                                AcceptUserId = _anyUserId,
                                UserInquiryId = 1,
                            }
                        ],
                    }
                },
                new List<object>
                {
                    new UserInquiryAcceptEntity
                    {
                        Id = 101,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                        AcceptUserId = _hodId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 102,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                        AcceptUserId = _gmId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 103,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        AcceptUserId = _anyUserId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                },
                new List<object>()
            ];
            yield return
            [
                new List<object>()
                {
                    new UserInquiryEntity
                    {
                        Id = 1,
                        UserId = _userIdInquiryFor,
                        Status = UserInquiryStatus.Open,
                        UserInquiryAccepts =
                        [
                            new()
                            {
                                Id = 1,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                                Status = UserInquiryAcceptStatus.Accepted,
                                AcceptUserId = _gmId,
                                UserInquiryId = 1,
                            },

                            new()
                            {
                                Id = 2,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                                Status = UserInquiryAcceptStatus.WaitingForApprove,
                                AcceptUserId = _hodId,
                                UserInquiryId = 1,
                            },

                            new()
                            {
                                Id = 3,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                                Status = UserInquiryAcceptStatus.Pending,
                                AcceptUserId = _anyUserId,
                                UserInquiryId = 1,
                            }
                        ],
                    }
                },
                new List<object>
                {
                    new UserInquiryAcceptEntity
                    {
                        Id = 101,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                        Status = UserInquiryAcceptStatus.Pending,
                        AcceptUserId = _hodId
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 102,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                        AcceptUserId = _gmId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 103,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        AcceptUserId = _anyUserId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                },
                new List<object>
                {
                    new InquiryUserNotifyDto()
                    {
                        AcceptUserId = _gmId,
                        UserInquiryId = 1,
                    }
                }
            ];

            yield return
            [
                new List<object>()
                {
                    new UserInquiryEntity
                    {
                        Id = 1,
                        UserId = _userIdInquiryFor,
                        Status = UserInquiryStatus.Open,
                        UserInquiryAccepts =
                        [
                            new()
                            {
                                Id = 1,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                                Status = UserInquiryAcceptStatus.Accepted,
                                AcceptUserId = _gmId,
                                UserInquiryId = 1,
                            },

                            new()
                            {
                                Id = 2,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                                Status = UserInquiryAcceptStatus.Accepted,
                                AcceptUserId = _hodId,
                                UserInquiryId = 1,
                            },

                            new()
                            {
                                Id = 3,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                                Status = UserInquiryAcceptStatus.Pending,
                                AcceptUserId = _anyUserId,
                                UserInquiryId = 1,
                            }
                        ],
                    }
                },
                new List<object>
                {
                    new UserInquiryAcceptEntity
                    {
                        Id = 102,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
                        AcceptUserId = _gmId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 103,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        AcceptUserId = _anyUserId,
                        Status = UserInquiryAcceptStatus.Pending,
                    },
                },
                new List<object>{ }
            ];

            yield return
            [
                new List<object>()
                {
                    new UserInquiryEntity
                    {
                        Id = 1,
                        UserId = _userIdInquiryFor,
                        Status = UserInquiryStatus.Open,
                        UserInquiryAccepts =
                        [
                            new()
                            {
                                Id = 1,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                                Status = UserInquiryAcceptStatus.Accepted,
                                AcceptUserId = _gmId,
                                UserInquiryId = 1,
                            },

                            new()
                            {
                                Id = 2,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                                Status = UserInquiryAcceptStatus.Accepted,
                                AcceptUserId = _hodId,
                                UserInquiryId = 1,
                            },

                            new()
                            {
                                Id = 3,
                                AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                                Status = UserInquiryAcceptStatus.Pending,
                                AcceptUserId = _anyUserId,
                                UserInquiryId = 1,
                            }
                        ],
                    }
                },
                new List<object>
                {
                    new UserInquiryAcceptEntity
                    {
                        Id = 1,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        Status = UserInquiryAcceptStatus.Pending,
                        AcceptUserId = _anyUserId,
                        UserInquiryId = 1,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 2,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        Status = UserInquiryAcceptStatus.Pending,
                        AcceptUserId = _gmId,
                        UserInquiryId = 1,
                    },
                    new UserInquiryAcceptEntity
                    {
                        Id = 3,
                        AcceptPolicyScope = InquiryAcceptPolicyScopes.User,
                        Status = UserInquiryAcceptStatus.Pending,
                        AcceptUserId = _hodId,
                        UserInquiryId = 1,
                    }
                },
                new List<object>
                {
                    new InquiryUserNotifyDto
                    {
                        AcceptUserId = _hodId,
                        UserInquiryId = 1,
                    },
                    new InquiryUserNotifyDto
                    {
                        AcceptUserId = _gmId,
                        UserInquiryId = 1
                    }
                }
            ];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Theory]
    [ClassData(typeof(TestDataClass))]
    public async Task ResolveWhenRequestIsValidShouldResolveValid(List<object> inquiryObjects,
        List<object> composedAcceptObjects, List<object> approversToNotifyObjects)
    {
        // Arrange
        var inquiries = inquiryObjects.Cast<UserInquiryEntity>().ToList();
        var composedAccepts = composedAcceptObjects.Cast<UserInquiryAcceptEntity>().ToList();
        var approversToNotify = approversToNotifyObjects.Cast<InquiryUserNotifyDto>().ToList();

        var acceptsBefore = inquiries.SelectMany(x => x.UserInquiryAccepts).ToList();

        _inquiriesDbContext.SetupGet(x => x.UserInquiryAccepts)
            .Returns(acceptsBefore.BuildMockDbSet().Object);

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(inquiries.BuildMockDbSet().Object);

        _inquiryAcceptsComposer.Setup(x => x.ComposeAsync(It.IsAny<int>(), It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(composedAccepts);

        _inquiryAcceptedStatusResolver.Setup(x => x.ResolveAsync(It.IsAny<UserInquiryEntity>(), _cancellationToken));        

        // Act
        var result = await _resolver.ResolveAsync(inquiries.Select(x => x.Id).ToArray(), _cancellationToken);

        // Assert
        result.Item1.Should().BeEquivalentTo(approversToNotify, config => config.WithoutStrictOrdering());
        _inquiryAcceptsComposer.Verify(x => x.ComposeAsync(It.IsAny<int>(), It.IsAny<int>(), _cancellationToken),
            Times.Exactly(inquiries.Count));
        _inquiriesDbContext.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
    }
}
