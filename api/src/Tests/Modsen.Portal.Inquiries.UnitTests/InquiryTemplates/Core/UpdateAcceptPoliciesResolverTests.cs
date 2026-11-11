using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Dto;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryTemplates.Core;

public class UpdateAcceptPoliciesResolverTests
{
    private readonly UpdateTemplateAcceptPoliciesResolver _resolver;

    private readonly Mock<IInquiriesDbContext> _inquiriesDbContext = new();
    private readonly Mock<IUpdatePolicyInquiryAcceptsResolver> _updatePolicyInquiryAcceptsResolver = new();
    private readonly Mock<INotificationSender> _notificationSender = new();
    private readonly Mock<IGetSubstitutesResolver> _getSubstitutesResolver = new();
    private readonly Mock<IGetAbsentApproversResolver> _getAbsentApproversResolverMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateAcceptPoliciesResolverTests()
    {
        _resolver = new(
            _inquiriesDbContext.Object,
            _updatePolicyInquiryAcceptsResolver.Object,
            _notificationSender.Object,
            _getSubstitutesResolver.Object,
            _getAbsentApproversResolverMock.Object);
    }

    private readonly InquiryTemplateEntity _templateEntity = new InquiryTemplateEntity
    {
        Id = 1,
        Name = "Template",
        AccrualType = null,
        ActivityType = ActivityType.NonWorkingPaid,
        Comment = null,
        IncludeHolidays = false,
        IncludeWeekends = false,
        InquiryType = InquiryType.SocialVacation,
        InquiryTypeOrderWeight = 10,
        IsActive = true,
        IsDefault = false,
        IsStaked = false,
        PeopleForceLeaveTypeId = null,
        ProhibitOnProbationPeriod = false,
        RequireAttachments = false,
        TrackingUnitType = TrackingUnit.Days,
        UserInquiryAgreementAssigns = [],
        InquiryTemplateAcceptPolicyPolicies =
        [
            new InquiryTemplateAcceptPolicyEntity
            {
                Id = 1,
                InquiryTemplateId = 1,
                OrderWeight = 1,
                Scope = InquiryAcceptPolicyScopes.GroupManager,
                UserId = null,
            },

            new InquiryTemplateAcceptPolicyEntity
            {
                Id = 2,
                InquiryTemplateId = 1,
                OrderWeight = 2,
                Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                UserId = null,
            },

            new InquiryTemplateAcceptPolicyEntity
            {
                Id = 3,
                InquiryTemplateId = 1,
                OrderWeight = 3,
                Scope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO,
                UserId = null,
            },

            new InquiryTemplateAcceptPolicyEntity
            {
                Id = 4,
                InquiryTemplateId = 1,
                OrderWeight = 4,
                Scope = InquiryAcceptPolicyScopes.User,
                UserId = 25,
            }

        ],
        InquiryTemplateAgreements = [],
        InquiryTemplateAssigns = [],
    };

    [Fact]
    public async Task ResolveWhenAcceptPoliciesTheSameShouldNotRemove()
    {
        // Arrange
        var newAcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
        {
            new UpdateInquiryTemplateAcceptPolicyDto
            {
                OrderWeight = 1,
                Scope = InquiryAcceptPolicyScopes.GroupManager,
                UserId = null,
            },
            new UpdateInquiryTemplateAcceptPolicyDto
            {
                OrderWeight = 2,
                Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                UserId = null,
            },
            new UpdateInquiryTemplateAcceptPolicyDto
            {
                OrderWeight = 3,
                Scope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO,
                UserId = null,
            },
            new UpdateInquiryTemplateAcceptPolicyDto
            {
                OrderWeight = 4,
                Scope = InquiryAcceptPolicyScopes.User,
                UserId = 25,
            }
        };

        // Act
        await _resolver.ResolveAsync(_templateEntity, newAcceptPolicies, _cancellationToken);

        // Assert
        _inquiriesDbContext.Verify(x => x.InquiryTemplateAcceptPolicies, Times.Never);
    }

    [Fact]
    public async Task ResolveAsyncWhenThereAreNoAcceptsForInquiryShouldUpdateValid()
    {
        // Arrange
        var newAcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
        {
            new UpdateInquiryTemplateAcceptPolicyDto
            {
                OrderWeight = 1,
                Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                UserId = null,
            },
            new UpdateInquiryTemplateAcceptPolicyDto
            {
                OrderWeight = 2,
                Scope = InquiryAcceptPolicyScopes.GroupManager,
                UserId = null,
            },
            new UpdateInquiryTemplateAcceptPolicyDto
            {
                OrderWeight = 3,
                Scope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO,
                UserId = null,
            },
            new UpdateInquiryTemplateAcceptPolicyDto
            {
                OrderWeight = 4,
                Scope = InquiryAcceptPolicyScopes.User,
                UserId = 25,
            }
        };

        _inquiriesDbContext.Setup(x => x.InquiryTemplateAcceptPolicies)
            .Returns(_templateEntity.InquiryTemplateAcceptPolicyPolicies.BuildMockDbSet().Object);

        var inquiries = new List<UserInquiryEntity>();

        _inquiriesDbContext.Setup(x => x.UserInquiries)
            .Returns(inquiries.BuildMockDbSet().Object);

        _inquiriesDbContext.Setup(x => x.UserInquiryAccepts)
            .Returns(new List<UserInquiryAcceptEntity>().BuildMockDbSet().Object);

        var inquiryIds = inquiries.Select(x => x.Id).ToArray();
        var notifications1 = new List<InquiryUserNotifyDto>();
        var notifications2 = new List<InquiryUserNotifyDto>();
        _updatePolicyInquiryAcceptsResolver.Setup(x => x.ResolveAsync(inquiryIds, _cancellationToken))
            .ReturnsAsync( (notifications1, notifications2) );

        _getSubstitutesResolver.Setup(x => x.ResolveAsync(notifications2, _cancellationToken))
            .ReturnsAsync([]);

        // Act
        await _resolver.ResolveAsync(_templateEntity, newAcceptPolicies, _cancellationToken);

        // Assert
        _inquiriesDbContext.Verify(x => x.InquiryTemplateAcceptPolicies, Times.Exactly(2));
        _inquiriesDbContext.Verify(x => x.UserInquiries, Times.Once);
        _updatePolicyInquiryAcceptsResolver.Verify(x => x.ResolveAsync(new int[0],
            _cancellationToken), Times.Once);
    }
}
