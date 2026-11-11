//using MockQueryable.Moq;
//using Modsen.Portal.Core.Identity;
//using Modsen.Portal.Core.Inquiries;
//using Modsen.Portal.Core.Exceptions;
//using Modsen.Portal.Identity.Contract;
//using Modsen.Portal.Inquiries.DataAccess.Npgsql;
//using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
//using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;
//using Moq;

//namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.Composers;

//public class InquiryAcceptsComposerTests
//{
//    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
//    private readonly Mock<IIdentityContract> _identityContractMock = new();

//    private readonly InquiryAcceptsComposer _composer;

//    private readonly CancellationToken _token =  CancellationToken.None;

//    public InquiryAcceptsComposerTests()
//    {
//        _composer = new InquiryAcceptsComposer(_inquiriesDbContextMock.Object, _identityContractMock.Object);
//    }

//    [Fact]
//    public async Task ComposeAsyncWhenThereAreNoPoliciesReturnsEmptyList()
//    {
//        //Arrange
//        const int userId = 1;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>().BuildMockDbSet().Object);

//        //Act
//        var result = await _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        result.Should().BeEmpty();

//    }

//    [Fact]
//    public async Task ComposeAsyncWhenThereIsNoHeadInUserDepartmentSkipsHead()
//    {
//        //Arrange
//        const int userId = 1;
//        const int ctoUserId = 3;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = InquiryAcceptPolicyScope.HeadOfDepartment,
//                    UserId = 2,
//                    OrderWeight = 2
//                },
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 2,
//                    Scope = InquiryAcceptPolicyScope.CTO,
//                    UserId = ctoUserId,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _identityContractMock
//            .Setup(contract => contract.GetUserHeadId(userId, _token))
//            .ReturnsAsync((int?)null);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>
//            {
//                new()
//                {
//                    UserId = ctoUserId,
//                    Position = PortalRoleNames.Cto
//                }
//            }.BuildMockDbSet().Object);

//        //Act
//        var result = await _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        result.Count.Should().Be(1);

//        result[0].AcceptUserId.Should().Be(3);
//    }

//    [Fact]
//    public async Task ComposeAsyncWhenHeadIdIsZeroSkipsHead()
//    {
//        //Arrange
//        const int userId = 1;
//        const int ctoUserId = 3;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = InquiryAcceptPolicyScope.HeadOfDepartment,
//                    UserId = 2,
//                    OrderWeight = 2
//                },
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 2,
//                    Scope = InquiryAcceptPolicyScope.CTO,
//                    UserId = ctoUserId,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _identityContractMock
//            .Setup(contract => contract.GetUserHeadId(userId, _token))
//            .ReturnsAsync(0);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>
//            {
//                new()
//                {
//                    UserId = ctoUserId,
//                    Position = PortalRoleNames.Cto
//                }
//            }.BuildMockDbSet().Object);

//        //Act
//        var result = await _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        result.Count.Should().Be(1);

//        result[0].AcceptUserId.Should().Be(3);
//    }

//    [Fact]
//    public async Task ComposeAsyncWhenHeadInUserDepartmentExistsAddsHead()
//    {
//        //Arrange
//        const int userId = 1;
//        const int headUserId = 2;
//        const int ctoUserId = 3;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = InquiryAcceptPolicyScope.HeadOfDepartment,
//                    UserId = headUserId,
//                    OrderWeight = 2
//                },
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 2,
//                    Scope = InquiryAcceptPolicyScope.CTO,
//                    UserId = ctoUserId,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _identityContractMock
//            .Setup(contract => contract.GetUserHeadId(userId, _token))
//            .ReturnsAsync(headUserId);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>
//            {
//                new()
//                {
//                    UserId = ctoUserId,
//                    Position = PortalRoleNames.Cto
//                }
//            }.BuildMockDbSet().Object);

//        //Act
//        var result = await _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        result.Count.Should().Be(2);

//        result[0].AcceptUserId.Should().Be(headUserId);
//        result[1].AcceptUserId.Should().Be(ctoUserId);
//    }

//    [Fact]
//    public async Task ComposeAsyncWhenThereIsNoGmForUserSkipsGroupManager()
//    {
//        //Arrange
//        const int userId = 1;
//        const int ctoUserId = 3;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = InquiryAcceptPolicyScope.GroupManager,
//                    UserId = 2,
//                    OrderWeight = 2
//                },
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 2,
//                    Scope = InquiryAcceptPolicyScope.CTO,
//                    UserId = ctoUserId,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _identityContractMock
//            .Setup(contract => contract.GetUserGroupManagerId(userId, _token))
//            .ReturnsAsync((int?)null);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>
//            {
//                new()
//                {
//                    UserId = ctoUserId,
//                    Position = PortalRoleNames.Cto
//                }
//            }.BuildMockDbSet().Object);

//        //Act
//        var result = await _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        result.Count.Should().Be(1);

//        result[0].AcceptUserId.Should().Be(3);
//    }

//    [Fact]
//    public async Task ComposeAsyncWhenGmIdIsZeroSkipsGroupManager()
//    {
//        //Arrange
//        const int userId = 1;
//        const int ctoUserId = 3;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = InquiryAcceptPolicyScope.GroupManager,
//                    UserId = 2,
//                    OrderWeight = 2
//                },
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 2,
//                    Scope = InquiryAcceptPolicyScope.CTO,
//                    UserId = ctoUserId,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _identityContractMock
//            .Setup(contract => contract.GetUserGroupManagerId(userId, _token))
//            .ReturnsAsync(0);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>
//            {
//                new()
//                {
//                    UserId = ctoUserId,
//                    Position = PortalRoleNames.Cto
//                }
//            }.BuildMockDbSet().Object);

//        //Act
//        var result = await _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        result.Count.Should().Be(1);

//        result[0].AcceptUserId.Should().Be(3);
//    }

//    [Fact]
//    public async Task ComposeAsyncWhenGmForUserExistsAddsGroupManager()
//    {
//        //Arrange
//        const int userId = 1;
//        const int gmId = 2;
//        const int ctoUserId = 3;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = InquiryAcceptPolicyScope.GroupManager,
//                    UserId = gmId,
//                    OrderWeight = 2
//                },
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 2,
//                    Scope = InquiryAcceptPolicyScope.CTO,
//                    UserId = ctoUserId,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _identityContractMock
//            .Setup(contract => contract.GetUserGroupManagerId(userId, _token))
//            .ReturnsAsync(gmId);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>
//            {
//                new()
//                {
//                    UserId = ctoUserId,
//                    Position = PortalRoleNames.Cto
//                }
//            }.BuildMockDbSet().Object);

//        //Act
//        var result = await _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        result.Count.Should().Be(2);

//        result[0].AcceptUserId.Should().Be(gmId);
//        result[1].AcceptUserId.Should().Be(ctoUserId);
//    }

//    [Fact]
//    public async Task ComposeAsyncWhenThereIsNoHigherManagerSkipsHigherManager()
//    {
//        //Arrange
//        const int userId = 1;
//        const int ctoUserId = 3;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = InquiryAcceptPolicyScope.HigherManager,
//                    UserId = 2,
//                    OrderWeight = 2
//                },
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 2,
//                    Scope = InquiryAcceptPolicyScope.CTO,
//                    UserId = ctoUserId,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _identityContractMock
//            .Setup(contract => contract.GetHigherManagerIds(userId, _token))
//            .ReturnsAsync([]);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>
//            {
//                new()
//                {
//                    UserId = ctoUserId,
//                    Position = PortalRoleNames.Cto
//                }
//            }.BuildMockDbSet().Object);

//        //Act
//        var result = await _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        result.Count.Should().Be(1);

//        result[0].AcceptUserId.Should().Be(3);
//    }

//    [Fact]
//    public async Task ComposeAsyncWhenNoHigherManagerIdIsZeroSkipsHigherManager()
//    {
//        //Arrange
//        const int userId = 1;
//        const int ctoUserId = 3;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = InquiryAcceptPolicyScope.HigherManager,
//                    UserId = 2,
//                    OrderWeight = 2
//                },
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 2,
//                    Scope = InquiryAcceptPolicyScope.CTO,
//                    UserId = ctoUserId,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _identityContractMock
//            .Setup(contract => contract.GetHigherManagerIds(userId, _token))
//            .ReturnsAsync([0]);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>
//            {
//                new()
//                {
//                    UserId = ctoUserId,
//                    Position = PortalRoleNames.Cto
//                }
//            }.BuildMockDbSet().Object);

//        //Act
//        var result = await _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        result.Count.Should().Be(1);

//        result[0].AcceptUserId.Should().Be(3);
//    }

//    [Fact]
//    public async Task ComposeAsyncWhenHigherManagerExistsAddsHigherManager()
//    {
//        //Arrange
//        const int userId = 1;
//        const int higherManagerId = 2;
//        const int ctoUserId = 3;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = InquiryAcceptPolicyScope.HigherManager,
//                    UserId = higherManagerId,
//                    OrderWeight = 2
//                },
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 2,
//                    Scope = InquiryAcceptPolicyScope.CTO,
//                    UserId = ctoUserId,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _identityContractMock
//            .Setup(contract => contract.GetHigherManagerIds(userId, _token))
//            .ReturnsAsync([higherManagerId]);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>
//            {
//                new()
//                {
//                    UserId = ctoUserId,
//                    Position = PortalRoleNames.Cto
//                }
//            }.BuildMockDbSet().Object);

//        //Act
//        var result = await _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        result.Count.Should().Be(2);

//        result[0].AcceptUserId.Should().Be(higherManagerId);
//        result[1].AcceptUserId.Should().Be(ctoUserId);
//    }

//    [Theory]
//    [InlineData([InquiryAcceptPolicyScope.CTO])]
//    [InlineData([InquiryAcceptPolicyScope.CEO])]
//    public async Task ComposeAsyncWhenChiefNotFoundThrowsException(InquiryAcceptPolicyScope scope)
//    {
//        //Arrange
//        const int userId = 1;
//        const int chiefUserId = 3;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = scope,
//                    UserId = chiefUserId,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>().BuildMockDbSet().Object);

//        //Act
//        var func = () => _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        await func.Should().ThrowAsync<EntityNotFoundException>("User for approve not found");
//    }

//    [Theory]
//    [InlineData([InquiryAcceptPolicyScope.CTO, PortalRoleNames.Cto])]
//    [InlineData([InquiryAcceptPolicyScope.CEO, PortalRoleNames.Ceo])]
//    public async Task ComposeAsyncWhenChiefExistsAddsChief(InquiryAcceptPolicyScope scope, string role)
//    {
//        //Arrange
//        const int userId = 1;
//        const int chiefUserId = 3;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = scope,
//                    UserId = chiefUserId,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>()
//            {
//                new()
//                {
//                    UserId = chiefUserId,
//                    Position = role
//                }
//            }.BuildMockDbSet().Object);

//        //Act
//        var result = await _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        result.Count.Should().Be(1);
//        result[0].AcceptUserId.Should().Be(chiefUserId);
//    }

//    [Fact]
//    public async Task ComposeAsyncWhenUserScopeButNoUserIdThrowsException()
//    {
//        //Arrange
//        const int userId = 1;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = InquiryAcceptPolicyScope.User,
//                    UserId = null,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>().BuildMockDbSet().Object);

//        //Act
//        var func = () => _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        await func.Should().ThrowAsync<EntityNotFoundException>("User for approve not found");
//    }

//    [Fact]
//    public async Task ComposeAsyncWhenUserScopeAndUserExistsAddsUserId()
//    {
//        //Arrange
//        const int userId = 1;
//        const int templateId = 1;

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiryTemplateAcceptPolicies)
//            .Returns(new List<InquiryTemplateAcceptPolicyEntity>()
//            {
//                new()
//                {
//                    InquiryTemplateId = templateId,
//                    Id = 1,
//                    Scope = InquiryAcceptPolicyScope.User,
//                    UserId = userId,
//                    OrderWeight = 1
//                }
//            }.BuildMockDbSet().Object);

//        _inquiriesDbContextMock
//            .Setup(context => context.InquiriesUsers)
//            .Returns(new List<InquiriesUserEntity>().BuildMockDbSet().Object);

//        //Act
//        var result = await _composer.ComposeAsync(userId, templateId, _token);

//        //Assert
//        result.Count.Should().Be(1);
//        result[0].AcceptUserId.Should().Be(userId);
//    }
//}
