using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Validators;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.Validators;

public class CreateUserInquiryAgreementRequestValidatorTests
{
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<ITeamBoardContract> _teamBoardContractMock = new();
    private readonly Mock<IIsEnoughAvailableDaysForInquiryBusinessRule> _isEnoughAvailableDaysForInquiryBusinessRule = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();

    private readonly CreateUserInquiryAgreementRequestValidator _validator;

    private readonly CancellationToken _token = CancellationToken.None;

    public CreateUserInquiryAgreementRequestValidatorTests()
    {
        _validator = new CreateUserInquiryAgreementRequestValidator(
            _inquiriesDbContextMock.Object,
            _clockServiceMock.Object,
            _teamBoardContractMock.Object,
            _isEnoughAvailableDaysForInquiryBusinessRule.Object,
            _identityContractMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task ValidateInquiryAsyncWhenInquiryTemplateIsNotActiveShouldThrowEntityConflictException()
    {
        //Arrange
        var command = new CreateUserInquiryAgreementRequest.Command();
        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = false,
            Name = "test",
        };

        //Act
        var func = () => _validator.ValidateInquiryAsync(command, inquiryTemplate, _token);

        //Assert
        await func.Should().ThrowAsync<EntityConflictException>("You can't create inquiry because it is inactive");

    }

    [Fact]
    public async Task ValidateInquiryAsyncWhenUserCannotCreateInquiryShouldThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1;
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            Name = "test",
        };

        //Act
        var func = () => _validator.ValidateInquiryAsync(command, inquiryTemplate, _token);

        //Assert
        await func.Should().ThrowAsync<ActionNotAllowedException>("You can't create inquiry for this user");

    }

    [Fact]
    public async Task ValidateInquiryAsyncWhenInquiryInHoursMoreThan8HoursShouldThrowInvalidDataAppException()
    {
        //Arrange
        var userId = 1;
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
            Hours = 10
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            Name = "test",
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);


        //Act
        var func = () => _validator.ValidateInquiryAsync(command, inquiryTemplate, _token);

        //Assert
        await func.Should().ThrowAsync<InvalidDataAppException>("You can't create inquiry with more than 8 hours");

    }

    [Fact]
    public async Task ValidateInquiryAsyncWhenInquiryRequestedOnProbationPeriodButCannotBeRequestedOnItShouldThrowInvalidDataAppException()
    {
        //Arrange
        var userId = 1;
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            ProhibitOnProbationPeriod = true,
            Name = "test",
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        //Act
        var func = () => _validator.ValidateInquiryAsync(command, inquiryTemplate, _token);

        //Assert
        await func.Should().ThrowAsync<ActionNotAllowedException>("Absence cannot be requested during the probation period");

    }

    [Fact]
    public async Task ValidateInquiryAsyncWhenUserInquiryNotFoundShouldThrowInvalidDataAppException()
    {
        //Arrange
        var userId = 1;
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            Name = "test",
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>().BuildMockDbSet().Object);


        //Act
        var func = () => _validator.ValidateInquiryAsync(command, inquiryTemplate, _token);

        //Assert
        await func.Should().ThrowAsync<InvalidDataAppException>("Absence is not assigned to the user");

    }

    [Fact]
    public async Task ValidateInquiryAsyncWhenUserDoesNotHaveUserInquiryShouldThrowInvalidDataAppException()
    {
        //Arrange
        var userId = 1;
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            Id = 1,
            IsActive = true,
            Name = "test",
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>()
            {
                new()
                {
                    InquiryTemplateId = 2,
                    AssignedToUserId = userId
                }
            }.BuildMockDbSet().Object);


        //Act
        var func = () => _validator.ValidateInquiryAsync(command, inquiryTemplate, _token);

        //Assert
        await func.Should().ThrowAsync<InvalidDataAppException>("Absence is not assigned to the user");

    }

    [Fact]
    public async Task ValidateInquiryAsyncWhenInquiryIsBeforeUserInquiryAssignShouldNotThrowAnyException()
    {
        //Arrange
        var userId = 1;
        var dateFrom = DateOnly.FromDateTime(DateTime.Today);
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
            DateFrom = dateFrom.AddMonths(-1)
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            Name = "test",
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>()
            {
                new()
                {
                    AssignedToUserId = userId,
                    DateFrom = dateFrom
                }
            }.BuildMockDbSet().Object);


        //Act
        var func = () => _validator.ValidateInquiryAsync(command, inquiryTemplate, _token);

        //Assert
        await func.Should().NotThrowAsync();

    }

    [Fact]
    public async Task ValidateInquiryAsyncWhenInquiryRequireAttachmentsButNoFilesProvidedShouldThrowInvalidDataAppException()
    {
        //Arrange
        var userId = 1;
        var dateFrom = DateOnly.FromDateTime(DateTime.Today);
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
            DateFrom = dateFrom.AddMonths(10),
            Files = []
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            RequireAttachments = true,
            Name = "test",
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>()
            {
                new()
                {
                    AssignedToUserId = userId,
                    DateFrom = dateFrom
                }
            }.BuildMockDbSet().Object);


        //Act
        var func = () => _validator.ValidateInquiryAsync(command, inquiryTemplate, _token);

        //Assert
        await func.Should().ThrowAsync<InvalidDataAppException>("Please attach the required document to proceed");

    }

    [Fact]
    public async Task ValidateInquiryAsyncWhenRequestIsValidShouldNotThrow()
    {
        //Arrange
        var userId = 1;
        var dateFrom = DateOnly.FromDateTime(DateTime.Today);
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
            DateFrom = dateFrom.AddMonths(10),
            Files = []
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            RequireAttachments = false,
            InquiryType = InquiryType.HomeOffice,
            Name = "test",
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>()
            {
                new()
                {
                    AssignedToUserId = userId,
                    DateFrom = dateFrom
                }
            }.BuildMockDbSet().Object);

        //Act
        var func = () => _validator.ValidateInquiryAsync(command, inquiryTemplate, _token);

        //Assert
        await func.Should().NotThrowAsync();

    }

    [Fact]
    public async Task ValidateAgreementAsyncWhenInquiryTemplateIsNotActiveShouldThrowEntityConflictException()
    {
        //Arrange
        var command = new CreateUserInquiryAgreementRequest.Command();
        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = false,
            Name = "test",
        };

        var agreement = new InquiryTemplateAgreementEntity
        {
            InquiryTemplate = inquiryTemplate
        };

        var inquiryType = InquiryType.HomeOffice;

        //Act
        var func = () => _validator.ValidateAgreementAsync(command, agreement, inquiryType, _token);

        //Assert
        await func.Should().ThrowAsync<EntityConflictException>("You cannot create agreement because it is inactive");

    }

    [Fact]
    public async Task ValidateAgreementAsyncWhenUserCannotCreateInquiryShouldThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1;
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            Name = "test",
        };

        var agreement = new InquiryTemplateAgreementEntity
        {
            InquiryTemplate = inquiryTemplate
        };

        var inquiryType = InquiryType.HomeOffice;

        //Act
        var func = () => _validator.ValidateAgreementAsync(command, agreement, inquiryType, _token);

        //Assert
        await func.Should().ThrowAsync<ActionNotAllowedException>("You can't create inquiry for this user");

    }

    [Fact]
    public async Task ValidateAgreementAsyncWhenInquiryInHoursMoreThan8HoursShouldThrowInvalidDataAppException()
    {
        //Arrange
        var userId = 1;
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
            Hours = 10
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            Name = "test",
        };

        var agreement = new InquiryTemplateAgreementEntity
        {
            InquiryTemplate = inquiryTemplate
        };

        var inquiryType = InquiryType.HomeOffice;

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        //Act
        var func = () => _validator.ValidateAgreementAsync(command, agreement, inquiryType, _token);

        //Assert
        await func.Should().ThrowAsync<InvalidDataAppException>("You can't create inquiry with more than 8 hours");

    }

    [Fact]
    public async Task ValidateAgreementAsyncWhenInquiryRequestedOnProbationPeriodButCannotBeRequestedOnItShouldThrowInvalidDataAppException()
    {
        //Arrange
        var userId = 1;
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            ProhibitOnProbationPeriod = true,
            Name = "test",
        };

        var agreement = new InquiryTemplateAgreementEntity
        {
            InquiryTemplate = inquiryTemplate
        };

        var inquiryType = InquiryType.HomeOffice;

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        //Act
        var func = () => _validator.ValidateAgreementAsync(command, agreement, inquiryType, _token);

        //Assert
        await func.Should().ThrowAsync<ActionNotAllowedException>("Absence cannot be requested during the probation period");

    }

    [Fact]
    public async Task ValidateAgreementAsyncWhenUserAgreementNotFoundShouldThrowInvalidDataAppException()
    {
        //Arrange
        var userId = 1;
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            Name = "test",
        };

        var agreement = new InquiryTemplateAgreementEntity
        {
            InquiryTemplate = inquiryTemplate
        };

        var inquiryType = InquiryType.HomeOffice;

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(new List<UserAgreementEntity>().BuildMockDbSet().Object);


        //Act
        var func = () => _validator.ValidateAgreementAsync(command, agreement, inquiryType, _token);

        //Assert
        await func.Should().ThrowAsync<InvalidDataAppException>("Absence is not assigned to the user");

    }

    [Fact]
    public async Task ValidateAgreementAsyncWhenUserHasNoSuchAgreementShouldThrowInvalidDataAppException()
    {
        //Arrange
        var userId = 1;
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            Name = "test",
        };

        var agreement = new InquiryTemplateAgreementEntity
        {
            Id = 1,
            InquiryTemplate = inquiryTemplate
        };

        var inquiryType = InquiryType.HomeOffice;

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(new List<UserAgreementEntity>()
            {
                new()
                {
                    UserId = userId,
                    InquiryAgreementTemplateId = 2
                }
            }.BuildMockDbSet().Object);


        //Act
        var func = () => _validator.ValidateAgreementAsync(command, agreement, inquiryType, _token);

        //Assert
        await func.Should().ThrowAsync<InvalidDataAppException>("Absence is not assigned to the user");

    }

    [Fact]
    public async Task ValidateAgreementAsyncWhenInquiryIsBeforeUserAgreementAssignShouldNotThrowAnyException()
    {
        //Arrange
        var userId = 1;
        var dateFrom = DateOnly.FromDateTime(DateTime.Today);
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
            DateFrom = dateFrom.AddMonths(-1)
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            Name = "test",
        };

        var agreement = new InquiryTemplateAgreementEntity
        {
            InquiryTemplate = inquiryTemplate
        };

        var inquiryType = InquiryType.HomeOffice;

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(new List<UserAgreementEntity>()
            {
                new()
                {
                    DateFrom = dateFrom,
                    UserId = userId
                }
            }.BuildMockDbSet().Object);

        _isEnoughAvailableDaysForInquiryBusinessRule
            .Setup(rule => rule.IsCorrespondsToBusiness(command.InquiryTemplateId, command.UserId, command.DateFrom,
                command.DateTo, _token))
            .ReturnsAsync(true);

        //Act
        var func = () => _validator.ValidateAgreementAsync(command, agreement, inquiryType, _token);

        //Assert
        await func.Should().NotThrowAsync();

    }

    [Fact]
    public async Task ValidateAgreementAsyncWhenInquiryRequireAttachmentsButNoFilesProvidedShouldThrowInvalidDataAppException()
    {
        //Arrange
        var userId = 1;
        var dateFrom = DateOnly.FromDateTime(DateTime.Today);
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
            DateFrom = dateFrom.AddMonths(10),
            Files = []
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            RequireAttachments = true,
            Name = "test",
        };

        var agreement = new InquiryTemplateAgreementEntity
        {
            InquiryTemplate = inquiryTemplate
        };

        var inquiryType = InquiryType.HomeOffice;

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(new List<UserAgreementEntity>()
            {
                new()
                {
                    DateFrom = dateFrom,
                    UserId = userId
                }
            }.BuildMockDbSet().Object);


        //Act
        var func = () => _validator.ValidateAgreementAsync(command, agreement, inquiryType, _token);

        //Assert
        await func.Should().ThrowAsync<InvalidDataAppException>("Please attach the required document to proceed");

    }

    [Fact]
    public async Task ValidateAgreementAsyncWhenRequestIsValidShouldNotThrow()
    {
        //Arrange
        var userId = 1;
        var dateFrom = DateOnly.FromDateTime(DateTime.Today);
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
            DateFrom = dateFrom.AddMonths(10),
            Files = []
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            RequireAttachments = false,
            InquiryType = InquiryType.HomeOffice,
            Name = "test",
        };

        var agreement = new InquiryTemplateAgreementEntity
        {
            InquiryTemplate = inquiryTemplate
        };

        var inquiryType = InquiryType.HomeOffice;

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(new List<UserAgreementEntity>()
            {
                new()
                {
                    DateFrom = dateFrom,
                    UserId = userId
                }
            }.BuildMockDbSet().Object);

        _isEnoughAvailableDaysForInquiryBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(command.InquiryTemplateId, command.UserId, command.DateFrom,
                command.DateTo, _token))
            .ReturnsAsync(true);

        //Act
        var func = () => _validator.ValidateAgreementAsync(command, agreement, inquiryType, _token);

        //Assert
        await func.Should().NotThrowAsync();

    }


    [Fact]
    public async Task ValidateInquiryAsyncWhenWhenCreatorIsCtoShouldNotThrowAnyException()
    {
        //Arrange
        var userId = 1;
        var dateFrom = DateOnly.FromDateTime(DateTime.Today);
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
            DateFrom = dateFrom.AddMonths(-1)
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            Name = "test",
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>()
            {
                new()
                {
                    AssignedToUserId = userId,
                    DateFrom = dateFrom
                }
            }.BuildMockDbSet().Object);


        //Act
        var func = () => _validator.ValidateInquiryAsync(command, inquiryTemplate, _token);

        //Assert
        await func.Should().NotThrowAsync();

    }


    [Fact]
    public async Task ValidateInquiryAsyncWhenWhenCreatorHasManageHrClaimShouldNotThrowAnyException()
    {
        //Arrange
        var userId = 1;
        var dateFrom = DateOnly.FromDateTime(DateTime.Today);
        var command = new CreateUserInquiryAgreementRequest.Command()
        {
            UserId = userId,
            DateFrom = dateFrom.AddMonths(-1)
        };

        var inquiryTemplate = new InquiryTemplateEntity()
        {
            IsActive = true,
            Name = "test",
        };

        _authenticatedUserContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(true);

        var today = DateOnly.FromDateTime(DateTime.Today);

        _clockServiceMock
            .Setup(clock => clock.TodayUtc())
            .Returns(today);

        _teamBoardContractMock
            .Setup(contract => contract.GetUserContractDates(userId, _token))
            .ReturnsAsync(new UserContractDatesOutput
            {
                UserId = userId,
                HireDate = today.AddMonths(-1),
                ContractEndDate = null,
                FireDate = null,
                ContractStartDate = today.AddMonths(-1),
            });

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>()
            {
                new()
                {
                    AssignedToUserId = userId,
                    DateFrom = dateFrom
                }
            }.BuildMockDbSet().Object);


        //Act
        var func = () => _validator.ValidateInquiryAsync(command, inquiryTemplate, _token);

        //Assert
        await func.Should().NotThrowAsync();

    }
}
