using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryAccepts.Core.Resolvers;

public class NextApproversToNotifyResolverTests
{
    private readonly NextApproversToNotifyResolver _resolver;
    private readonly Mock<IGetAbsentApproversResolver> _getAbsentApproversResolver = new();
    private readonly Mock<IGetSubstituteResolver> _getSubstituteResolver = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private static readonly int[] _approverIds = [1, 2];

    public NextApproversToNotifyResolverTests()
    {
        _resolver = new(_getAbsentApproversResolver.Object, _getSubstituteResolver.Object);
    }

    [Fact]
    public async Task ResolveWhenMainApproversAreNotAbsentShouldReturnOnlyMainApprovers()
    {
        // Arrange
        var acceptEntities = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                AcceptUserId = 1,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
            },
            new UserInquiryAcceptEntity
            {
                AcceptUserId = 2,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
            },
        };

        var userInquiry = new UserInquiryEntity
        {
            InquiryTemplateId = 1,
            UserId = 1000,
        };

        _getAbsentApproversResolver.Setup(x => x.ResolveAsync(_approverIds, _cancellationToken))
            .ReturnsAsync([]);

        // Act
        var result = await _resolver.ResolveAsync(acceptEntities, userInquiry, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(_approverIds);
    }

    [Fact]
    public async Task ResolveWhenMainApproversAreAbsentSubstituteIsNotSpecifiedShouldReturnOnlyMainApprovers()
    {
        // Arrange
        var acceptEntities = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                AcceptUserId = 1,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
            },
            new UserInquiryAcceptEntity
            {
                AcceptUserId = 2,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
            },
        };

        var userInquiry = new UserInquiryEntity
        {
            InquiryTemplateId = 1,
            UserId = 1000,
        };

        var absent = new List<int> { 1 };

        _getAbsentApproversResolver.Setup(x => x.ResolveAsync(_approverIds, _cancellationToken))
            .ReturnsAsync(absent);

        _getSubstituteResolver.Setup(x => x.ResolveAsync(userInquiry.InquiryTemplateId, userInquiry.UserId,
                InquiryAcceptPolicyScopes.ProjectManager, null, _cancellationToken))
            .ReturnsAsync((int?)null);

        // Act
        var result = await _resolver.ResolveAsync(acceptEntities, userInquiry, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(_approverIds);
    }

    [Fact]
    public async Task ResolveWhenMainApproversAreAbsentSubstituteIsSpecifiedShouldReturnMainApproversAndSubstitute()
    {
        // Arrange
        var acceptEntities = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                AcceptUserId = 1,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
            },
            new UserInquiryAcceptEntity
            {
                AcceptUserId = 2,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
            },
        };

        var userInquiry = new UserInquiryEntity
        {
            InquiryTemplateId = 1,
            UserId = 1000,
        };

        var absent = new List<int> { 1 };
        var substituteId = 25;

        _getAbsentApproversResolver.Setup(x => x.ResolveAsync(_approverIds, _cancellationToken))
            .ReturnsAsync(absent);

        _getSubstituteResolver.Setup(x => x.ResolveAsync(userInquiry.InquiryTemplateId, userInquiry.UserId,
                InquiryAcceptPolicyScopes.ProjectManager, 1, _cancellationToken))
            .ReturnsAsync(substituteId);

        var expected = new List<int> { 1, 2, 25 };
        // Act
        var result = await _resolver.ResolveAsync(acceptEntities, userInquiry, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
