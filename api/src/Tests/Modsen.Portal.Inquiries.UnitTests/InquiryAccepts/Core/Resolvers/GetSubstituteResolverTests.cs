using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryAccepts.Core.Resolvers;

public class GetSubstituteResolverTests
{
    private readonly GetSubstituteResolver _resolver;
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContext = new();
    private readonly Mock<IIdentityContract> _identityContract = new();

    public GetSubstituteResolverTests()
    {
        _resolver = new(_inquiriesDbContext.Object, _identityContract.Object);
    }

    [Fact]
    public async Task ResolveWhenPolicyIsNotFoundShouldReturnNull()
    {
        // Arrange
        _inquiriesDbContext.SetupGet(x => x.InquiryTemplateAcceptPolicies)
            .Returns(Array.Empty<InquiryTemplateAcceptPolicyEntity>().BuildMockDbSet().Object);

        var templateId = 1;
        var userIdInquiryFor = 1;
        var mainScope = InquiryAcceptPolicyScopes.GroupManager;

        // Act
        var result = await _resolver.ResolveAsync(templateId, userIdInquiryFor, mainScope, null, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(InquiryAcceptPolicyScopes.GroupManager)]
    [InlineData(InquiryAcceptPolicyScopes.HeadOfDepartment)]
    [InlineData(InquiryAcceptPolicyScopes.User)]
    [InlineData(InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO)]
    [InlineData(InquiryAcceptPolicyScopes.ProjectManager)]
    public async Task ResolveShouldReturnValidUserId(InquiryAcceptPolicyScopes substituteScope)
    {
        // Arrange
        var userIdInquiryFor = 22;
        var templateId = 33;
        var mainScope = InquiryAcceptPolicyScopes.GroupManager;

        var gmId = 1;
        var headId = 2;
        var anyUserId = 3;
        var chiefId = 4;

        int? policyUserId = substituteScope == InquiryAcceptPolicyScopes.User ? anyUserId : null;

        var policy = new InquiryTemplateAcceptPolicyEntity
        {
            InquiryTemplateId = templateId,
            Scope = mainScope,
            SubstituteScope = substituteScope,
            SubstituteUserId = policyUserId
        };

        _inquiriesDbContext.SetupGet(x => x.InquiryTemplateAcceptPolicies)
            .Returns(new List<InquiryTemplateAcceptPolicyEntity> { policy }.BuildMockDbSet().Object);

        _identityContract.Setup(x => x.GetUserGroupManagerId(userIdInquiryFor, CancellationToken.None))
            .ReturnsAsync(gmId);

        _identityContract.Setup(x => x.GetUserHeadId(userIdInquiryFor, CancellationToken.None))
            .ReturnsAsync(headId);

        _identityContract.Setup(x => x.GetUserChiefIdAsync(userIdInquiryFor, CancellationToken.None))
            .ReturnsAsync(chiefId);

        var expectation = new Dictionary<InquiryAcceptPolicyScopes, int?>
        {
            [InquiryAcceptPolicyScopes.GroupManager] = gmId,
            [InquiryAcceptPolicyScopes.User] = anyUserId,
            [InquiryAcceptPolicyScopes.HeadOfDepartment] = headId,
            [InquiryAcceptPolicyScopes.ProjectManager] = null,
            [InquiryAcceptPolicyScopes.CTO | InquiryAcceptPolicyScopes.CEO] = chiefId,
        };

        // Act

        var result = await _resolver.ResolveAsync(templateId, userIdInquiryFor, mainScope, null, CancellationToken.None);

        // Assert
        result.Should().Be(expectation[substituteScope]);
    }
}
