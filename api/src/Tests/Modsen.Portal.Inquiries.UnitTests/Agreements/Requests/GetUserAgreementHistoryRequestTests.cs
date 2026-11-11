using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Agreements.Requests;
using Modsen.Portal.Inquiries.Requests.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Requests;

public class GetUserAgreementHistoryRequestTests
{
    private readonly Mock<IHasUserAccessBusinessRule> _hasUserAccessBusinessRuleMock = new();
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;
    private readonly GetUserAgreementHistoryRequest _handler;

    public GetUserAgreementHistoryRequestTests()
    {
        _handler = new GetUserAgreementHistoryRequest(_hasUserAccessBusinessRuleMock.Object,
            _inquiriesDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserHasNoAccessToInquiriesShouldThrowNotFoundException()
    {
        //Arrange
        var userId = 1;
        var accessOptions = AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;

        _hasUserAccessBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(null, userId, accessOptions, _token))
            .ReturnsAsync(false);

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAbsenceHistoryCoreDto>(queryParameters);

        var query = new GetUserAgreementHistoryRequest.Query(userId, null, queryOptions);

        //Act
        var function = () => _handler.Handle(query, _token);

        //Assert
        await function.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.UserNotFound);

    }

    [Fact]
    public async Task HandleWhenYearFilterAppliedShouldReturnHistoryForSpecifiedYear()
    {
        //Arrange
        var userId = 1;
        var accessOptions = AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;

        _hasUserAccessBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(null, userId, accessOptions, _token))
            .ReturnsAsync(true);

        var inquiryTemplate = new InquiryTemplateEntity
        {
            Id = 1,
            Name = "Inquiry template name",
            InquiryType = InquiryType.Vacation,
            TrackingUnitType = TrackingUnit.Days
        };

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiries)
            .Returns(new List<UserInquiryEntity> { }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>
            {
                new()
                {
                    UserId = userId,
                    UserInquiryId = null,
                    AmountOfDays = 2.08m,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    TransactionDate = new DateTime(2025, 6, 1),
                    UserAgreementId = 1,
                    InquiryTemplateId = inquiryTemplate.Id,
                    InquiryTemplate = inquiryTemplate,
                },
                new()
                {
                    UserId = userId,
                    UserInquiryId = null,
                    AmountOfDays = 2.08m,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    TransactionDate = new DateTime(2024, 6, 1),
                    UserAgreementId = 1,
                    InquiryTemplateId = inquiryTemplate.Id,
                    InquiryTemplate = inquiryTemplate,
                },
            }.BuildMockDbSet().Object);

        var expectedResult = new UserAbsenceHistoryCoreDto
        {
            Id = null,
            AmountOfDays = 2.08m,
            TransactionDate = new DateTime(2024, 6, 1),
            TransactionType = UserAgreementTransactionType.Accrual,
            InquiryTemplateName = "Inquiry template name",
            InquiryTemplateId = 1,
            InquiryId = null,
            UserAgreementId = 1,
            Used = null,
            Accrued = 2.08m,
            Remainder = 2.08m,
            InquiryType = InquiryType.Vacation,
            TrackingUnitType = TrackingUnit.Days,
            Documents = []
        };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAbsenceHistoryCoreDto>(queryParameters);

        var query = new GetUserAgreementHistoryRequest.Query(userId, 2024, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Total.Should().Be(1);
        result.Items.Should().BeEquivalentTo([expectedResult]);

    }

    [Fact]
    public async Task HandleWhenInquiryTypeFilterAppliedShouldReturnHistoryWithSpecifiedTypeEntities()
    {
        //Arrange
        var userId = 1;
        var accessOptions = AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;

        _hasUserAccessBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(null, userId, accessOptions, _token))
            .ReturnsAsync(true);

        var vacationInquiryTemplate = new InquiryTemplateEntity
        {
            Id = 1,
            Name = "Vacation",
            InquiryType = InquiryType.Vacation,
            TrackingUnitType = TrackingUnit.Days
        };

        var sickDayInquiryTemplate = new InquiryTemplateEntity
        {
            Id = 2,
            Name = "Sick day",
            InquiryType = InquiryType.SickDay,
            TrackingUnitType = TrackingUnit.Days
        };

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiries)
            .Returns(new List<UserInquiryEntity> { }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>
            {
                new()
                {
                    UserId = userId,
                    UserInquiryId = null,
                    AmountOfDays = 5m,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    TransactionDate = new DateTime(2025, 6, 1),
                    UserAgreementId = 2,
                    InquiryTemplateId = sickDayInquiryTemplate.Id,
                    InquiryTemplate = sickDayInquiryTemplate,
                },
                new()
                {
                    UserId = userId,
                    UserInquiryId = null,
                    AmountOfDays = 2.08m,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    TransactionDate = new DateTime(2025, 6, 1),
                    UserAgreementId = 1,
                    InquiryTemplateId = vacationInquiryTemplate.Id,
                    InquiryTemplate = vacationInquiryTemplate,
                },
            }.BuildMockDbSet().Object);

        var expectedResult = new UserAbsenceHistoryCoreDto
        {
            Id = null,
            AmountOfDays = 5m,
            TransactionDate = new DateTime(2025, 6, 1),
            TransactionType = UserAgreementTransactionType.Accrual,
            InquiryTemplateName = "Sick day",
            InquiryTemplateId = 2,
            InquiryId = null,
            UserAgreementId = 2,
            Used = null,
            Accrued = 5m,
            Remainder = 5m,
            InquiryType = InquiryType.SickDay,
            TrackingUnitType = TrackingUnit.Days,
            Documents = []
        };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
            [nameof(ODataRawQueryOptions.Filter)] = "(inquiryType eq 'SickDay')",

        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAbsenceHistoryCoreDto>(queryParameters);

        var query = new GetUserAgreementHistoryRequest.Query(userId, null, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Total.Should().Be(1);
        result.Items.Should().BeEquivalentTo([expectedResult]);

    }

    [Fact]
    public async Task HandleWhenUserHasTransactionsShouldReturnHistoryWithTransactions()
    {
        //Arrange
        var userId = 1;
        var accessOptions = AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;

        _hasUserAccessBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(null, userId, accessOptions, _token))
            .ReturnsAsync(true);

        var inquiryTemplate = new InquiryTemplateEntity
        {
            Id = 1,
            Name = "Inquiry template name",
            InquiryType = InquiryType.Vacation,
            TrackingUnitType = TrackingUnit.Days
        };

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiries)
            .Returns(new List<UserInquiryEntity> { }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>
            {
                new()
                {
                    UserId = userId,
                    UserInquiryId = null,
                    AmountOfDays = 2.08m,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    TransactionDate = new DateTime(2025, 6, 1),
                    UserAgreementId = 1,
                    InquiryTemplateId = inquiryTemplate.Id,
                    InquiryTemplate = inquiryTemplate,
                }
            }.BuildMockDbSet().Object);

        var expectedResult = new UserAbsenceHistoryCoreDto
        {
            Id = null,
            AmountOfDays = 2.08m,
            TransactionDate = new DateTime(2025, 6, 1),
            TransactionType = UserAgreementTransactionType.Accrual,
            InquiryTemplateName = "Inquiry template name",
            InquiryTemplateId = 1,
            InquiryId = null,
            UserAgreementId = 1,
            Used = null,
            Accrued = 2.08m,
            Remainder = 2.08m,
            InquiryType = InquiryType.Vacation,
            TrackingUnitType = TrackingUnit.Days,
            Documents = []
        };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAbsenceHistoryCoreDto>(queryParameters);

        var query = new GetUserAgreementHistoryRequest.Query(userId, null, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Total.Should().Be(1);
        result.Items.Should().BeEquivalentTo([expectedResult]);

    }

    [Fact]
    public async Task HandleWhenUserHasApprovedInquiryWithoutApproversShouldReturnHistoryWithTransactionDateWhenRequestCreated()
    {
        //Arrange
        var userId = 1;
        var accessOptions = AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;

        _hasUserAccessBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(null, userId, accessOptions, _token))
            .ReturnsAsync(true);

        var inquiryTemplate = new InquiryTemplateEntity
        {
            Id = 1,
            Name = "Inquiry template name",
            InquiryType = InquiryType.Vacation,
            TrackingUnitType = TrackingUnit.Days
        };

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryFiles)
            .Returns(new List<UserInquiryFileEntity>()
            {
                new()
                {
                    UserInquiryId = 1,
                    FileId = "fileId",
                    FileName = "fileName",
                    FileSize = 1000
                }
            }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiries)
            .Returns(new List<UserInquiryEntity>
            {
                new()
                {
                    Id = 1,
                    UserId = userId,
                    AgreementTemplateId = null,
                    Status = UserInquiryStatus.Approved,
                    CreatedAtUtc = new DateTime(2025, 5, 1),
                    InquiryTemplateId = 5,
                    InquiryTemplate = new InquiryTemplateEntity()
                    {
                        Name = "template without agreement",
                        InquiryType = InquiryType.SickLeave,
                        TrackingUnitType = TrackingUnit.Days,
                    },
                    TotalAccountantDays = 5
                }
            }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>
            {
                new()
                {
                    UserId = userId,
                    UserInquiryId = null,
                    AmountOfDays = 2.08m,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    TransactionDate = new DateTime(2025, 6, 1),
                    UserAgreementId = 1,
                    InquiryTemplateId = inquiryTemplate.Id,
                    InquiryTemplate = inquiryTemplate,
                }
            }.BuildMockDbSet().Object);

        var expectedResult = new UserAbsenceHistoryCoreDto[]
        {
            new ()
            {
                Id = null,
                AmountOfDays = 2.08m,
                TransactionDate = new DateTime(2025, 6, 1),
                TransactionType = UserAgreementTransactionType.Accrual,
                InquiryTemplateName = "Inquiry template name",
                InquiryTemplateId = 1,
                InquiryId = null,
                UserAgreementId = 1,
                Used = null,
                Accrued = 2.08m,
                Remainder = 2.08m,
                InquiryType = InquiryType.Vacation,
                TrackingUnitType = TrackingUnit.Days,
                Documents = []
            },
            new ()
            {
                Id = null,
                AmountOfDays = 5,
                TransactionDate = new DateTime(2025, 5, 1),
                TransactionType = UserAgreementTransactionType.Using,
                InquiryTemplateName = "template without agreement",
                InquiryTemplateId = 5,
                InquiryId = 1,
                UserAgreementId = null,
                Used = 5,
                Accrued = null,
                Remainder = 5,
                InquiryType = InquiryType.SickLeave,
                TrackingUnitType = TrackingUnit.Days,
                Documents = [new InquiryFileCoreDto("fileId", "fileName", 1000)]
            },

        };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAbsenceHistoryCoreDto>(queryParameters);

        var query = new GetUserAgreementHistoryRequest.Query(userId, null, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Total.Should().Be(2);
        result.Items.Should().BeEquivalentTo(expectedResult);

    }

    [Fact]
    public async Task HandleWhenUserHasTransactionsAndApprovedInquiriesShouldReturnHistoryWithBoth()
    {
        //Arrange
        var userId = 1;
        var accessOptions = AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;

        _hasUserAccessBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(null, userId, accessOptions, _token))
            .ReturnsAsync(true);

        var inquiryTemplate = new InquiryTemplateEntity
        {
            Id = 1,
            Name = "Inquiry template name",
            InquiryType = InquiryType.Vacation,
            TrackingUnitType = TrackingUnit.Days
        };

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryFiles)
            .Returns(new List<UserInquiryFileEntity>()
            {
                new()
                {
                    UserInquiryId = 1,
                    FileId = "fileId",
                    FileName = "fileName",
                    FileSize = 1000
                }
            }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiries)
            .Returns(new List<UserInquiryEntity>
            {
                new()
                {
                    Id = 1,
                    UserId = userId,
                    AgreementTemplateId = null,
                    Status = UserInquiryStatus.Approved,
                    UserInquiryAccepts =
                    [
                        new() { AcceptedAtUtc = new DateTime(2025, 5, 1) }

                    ],
                    InquiryTemplateId = 5,
                    InquiryTemplate = new InquiryTemplateEntity()
                    {
                        Name = "template without agreement",
                        InquiryType = InquiryType.SickLeave,
                        TrackingUnitType = TrackingUnit.Days,
                    },
                    TotalAccountantDays = 5

                }
            }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>
            {
                new()
                {
                    UserId = userId,
                    UserInquiryId = null,
                    AmountOfDays = 2.08m,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    TransactionDate = new DateTime(2025, 6, 1),
                    UserAgreementId = 1,
                    InquiryTemplateId = inquiryTemplate.Id,
                    InquiryTemplate = inquiryTemplate,
                }
            }.BuildMockDbSet().Object);

        var expectedResult = new UserAbsenceHistoryCoreDto[]
        {
            new ()
            {
                Id = null,
                AmountOfDays = 2.08m,
                TransactionDate = new DateTime(2025, 6, 1),
                TransactionType = UserAgreementTransactionType.Accrual,
                InquiryTemplateName = "Inquiry template name",
                InquiryTemplateId = 1,
                InquiryId = null,
                UserAgreementId = 1,
                Used = null,
                Accrued = 2.08m,
                Remainder = 2.08m,
                InquiryType = InquiryType.Vacation,
                TrackingUnitType = TrackingUnit.Days,
                Documents = []
            },
            new ()
            {
                Id = null,
                AmountOfDays = 5,
                TransactionDate = new DateTime(2025, 5, 1),
                TransactionType = UserAgreementTransactionType.Using,
                InquiryTemplateName = "template without agreement",
                InquiryTemplateId = 5,
                InquiryId = 1,
                UserAgreementId = null,
                Used = 5,
                Accrued = null,
                Remainder = 5,
                InquiryType = InquiryType.SickLeave,
                TrackingUnitType = TrackingUnit.Days,
                Documents = [new InquiryFileCoreDto("fileId", "fileName", 1000)]
            },

        };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAbsenceHistoryCoreDto>(queryParameters);

        var query = new GetUserAgreementHistoryRequest.Query(userId, null, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Total.Should().Be(2);
        result.Items.Should().BeEquivalentTo(expectedResult);

    }

    [Fact]
    public async Task HandleWhenOrderByInquiryTypeShouldReturnOrderedPageableList()
    {
        //Arrange
        var userId = 1;
        var accessOptions = AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;

        _hasUserAccessBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(null, userId, accessOptions, _token))
            .ReturnsAsync(true);

        var inquiryTemplate = new InquiryTemplateEntity
        {
            Id = 1,
            Name = "Inquiry template name",
            InquiryType = InquiryType.Vacation,
            TrackingUnitType = TrackingUnit.Days
        };

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryFiles)
            .Returns(new List<UserInquiryFileEntity>()
            {
                new()
                {
                    UserInquiryId = 1,
                    FileId = "fileId",
                    FileName = "fileName",
                    FileSize = 1000
                }
            }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiries)
            .Returns(new List<UserInquiryEntity>
            {
                new()
                {
                    Id = 1,
                    UserId = userId,
                    AgreementTemplateId = null,
                    Status = UserInquiryStatus.Approved,
                    CreatedAtUtc = new DateTime(2025, 5, 1),
                    InquiryTemplateId = 5,
                    InquiryTemplate = new InquiryTemplateEntity()
                    {
                        Name = "template without agreement",
                        InquiryType = InquiryType.SickLeave,
                        TrackingUnitType = TrackingUnit.Days,
                    },
                    TotalAccountantDays = 5
                },
                new UserInquiryEntity()
                {
                    Id = 2,
                    UserId = userId,
                    AgreementTemplateId = null,
                    Status = UserInquiryStatus.Approved,
                    CreatedAtUtc = new DateTime(2025, 5, 1),
                    InquiryTemplateId = 2,
                    InquiryTemplate = new InquiryTemplateEntity()
                    {
                        Name = "template without agreement",
                        InquiryType = InquiryType.OwnExpense,
                        TrackingUnitType = TrackingUnit.Days,
                    },
                    TotalAccountantDays = 5
                },
                new UserInquiryEntity()
                {
                    Id = 3,
                    UserId = userId,
                    AgreementTemplateId = null,
                    Status = UserInquiryStatus.Approved,
                    CreatedAtUtc = new DateTime(2025, 5, 1),
                    InquiryTemplateId = 3,
                    InquiryTemplate = new InquiryTemplateEntity()
                    {
                        Name = "template without agreement",
                        InquiryType = InquiryType.WeekendTransfer,
                        TrackingUnitType = TrackingUnit.Days,
                    },
                    TotalAccountantDays = 5
                },
                new UserInquiryEntity()
                {
                    Id = 4,
                    UserId = userId,
                    AgreementTemplateId = null,
                    Status = UserInquiryStatus.Approved,
                    CreatedAtUtc = new DateTime(2025, 5, 1),
                    InquiryTemplateId = 4,
                    InquiryTemplate = new InquiryTemplateEntity()
                    {
                        Name = "template without agreement",
                        InquiryType = InquiryType.MilitaryEnlistmentOffice,
                        TrackingUnitType = TrackingUnit.Days,
                    },
                    TotalAccountantDays = 5,
                },
                new UserInquiryEntity()
                {
                    Id = 5,
                    UserId = userId,
                    AgreementTemplateId = null,
                    Status = UserInquiryStatus.Approved,
                    CreatedAtUtc = new DateTime(2025, 5, 1),
                    InquiryTemplateId = 6,
                    InquiryTemplate = new InquiryTemplateEntity()
                    {
                        Name = "template without agreement",
                        InquiryType = InquiryType.SocialVacation,
                        TrackingUnitType = TrackingUnit.Days,
                    },
                    TotalAccountantDays = 5,
                },
                new UserInquiryEntity()
                {
                    Id = 6,
                    UserId = userId,
                    AgreementTemplateId = 1,
                    Status = UserInquiryStatus.Approved,
                    CreatedAtUtc = new DateTime(2025, 5, 1),
                    InquiryTemplateId = 8,
                    InquiryTemplate = new InquiryTemplateEntity()
                    {
                        Name = "template with agreement",
                        InquiryType = InquiryType.Vacation,
                        TrackingUnitType = TrackingUnit.Days,
                    },
                    TotalAccountantDays = 5
                }
            }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>
            {
                new()
                {
                    UserId = userId,
                    UserInquiryId = null,
                    AmountOfDays = 2.08m,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    TransactionDate = new DateTime(2025, 6, 1),
                    UserAgreementId = 1,
                    InquiryTemplateId = inquiryTemplate.Id,
                    InquiryTemplate = inquiryTemplate,
                },
                new()
                {
                    UserId = userId,
                    UserInquiryId = 1,
                    TransactionDate = new DateTime(2025, 6, 1),
                    UserAgreementId = 1,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    InquiryTemplateId = inquiryTemplate.Id,
                    InquiryTemplate = inquiryTemplate,
                    AmountOfDays = 1
                },
                new()
                {
                    UserId = userId,
                    UserInquiryId = 2,
                    TransactionDate = new DateTime(2025, 6, 1),
                    UserAgreementId = 1,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    InquiryTemplateId = inquiryTemplate.Id,
                    InquiryTemplate = inquiryTemplate,
                    AmountOfDays = 1
                },
                new()
                {
                    UserId = userId,
                    UserInquiryId = 3,
                    TransactionDate = new DateTime(2025, 6, 1),
                    UserAgreementId = 1,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    InquiryTemplateId = inquiryTemplate.Id,
                    InquiryTemplate = inquiryTemplate,
                    AmountOfDays = 1
                },
                new()
                {
                    UserId = userId,
                    UserInquiryId = 4,
                    TransactionDate = new DateTime(2025, 6, 1),
                    UserAgreementId = 1,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    InquiryTemplateId = inquiryTemplate.Id,
                    InquiryTemplate = inquiryTemplate,
                    AmountOfDays = 1
                },
                new()
                {
                    UserId = userId,
                    UserInquiryId = 5,
                    TransactionDate = new DateTime(2025, 6, 1),
                    UserAgreementId = 1,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    InquiryTemplateId = inquiryTemplate.Id,
                    InquiryTemplate = inquiryTemplate,
                    AmountOfDays = 1
                }
            }.BuildMockDbSet().Object);

        var expectedInquiryTypeOrder = new List<InquiryType>
        {
            InquiryType.Vacation,
            InquiryType.SickLeave,
            InquiryType.OwnExpense,
            InquiryType.WeekendTransfer,
            InquiryType.MilitaryEnlistmentOffice,
            InquiryType.SocialVacation,
        };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "11",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
            [nameof(ODataRawQueryOptions.OrderBy)] = "InquiryTypeOrderWeight asc"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAbsenceHistoryCoreDto>(queryParameters);

        var query = new GetUserAgreementHistoryRequest.Query(userId, null, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(11);
        result.Total.Should().Be(11);
        result.Items.Select(x => x.InquiryType).Distinct().Should().BeEquivalentTo(expectedInquiryTypeOrder);

    }
}
