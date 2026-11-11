using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Assigns.Requests;

public class GetAssignsByTemplateRequestTests
{
    private readonly Mock<IInquiriesDbContext> _dbContextMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();

    private readonly GetAssignsByTemplateRequest _handler;

    private readonly CancellationToken _token = CancellationToken.None;

    public GetAssignsByTemplateRequestTests()
    {
        _handler = new GetAssignsByTemplateRequest(_dbContextMock.Object, _identityContractMock.Object);
    }

    [Fact]
    public async Task HandleWhenTemplateNotExistHasNoAssignsThrowsException()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>().BuildMockDbSet().Object);

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<AssignedUserDto>(queryParameters, nameof(AssignedUserDto.UserId));

        var query = new GetAssignsByTemplateRequest.Query(1, queryOptions);

        // Act
        var act = async () => await _handler.Handle(query, _token);

        // Assert
        await act.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage("template not found");
    }


    [Fact]
    public async Task HandleWhenVacationTemplateReturnsActiveAgreements()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        const int templateId = 1;

        var template = new InquiryTemplateEntity
        {
            Id = templateId,
            Name = "test",
            InquiryType = InquiryType.Vacation
        };

        _dbContextMock
            .Setup(c => c.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { template }.BuildMockDbSet().Object);

        var agreements = new List<UserAgreementEntity>
        {
            new()
            {
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = templateId },
                DateFrom = today.AddDays(-2),
                DateTo = today.AddDays(1),
                User = new InquiriesUserEntity
                {
                    LastName = "John",
                    FirstName = "Doe",
                    Email = "test",
                    DepartmentId = "it",
                    AvatarFileId = Guid.CreateVersion7().ToString(),
                    IsActive = true,
                },
            },
            new()
            {
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = templateId },
                DateFrom = today.AddDays(-10),
                DateTo = today.AddDays(-1),
                User = new InquiriesUserEntity
                {
                    LastName = "Jane",
                    FirstName = "Smith",
                    Email = "test",
                    DepartmentId = "it",
                    AvatarFileId = Guid.CreateVersion7().ToString(),
                    IsActive = true,
                },
                UserId = 2
            }
        };

        _dbContextMock
            .Setup(c => c.UserAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock
            .Setup(c => c.GetDepartmentsClassificator(_token))
            .ReturnsAsync([new DepartmentClassificationDto("it", "IT Department", "IT")]);

        var queryParams = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<AssignedUserDto>(
            queryParams, nameof(AssignedUserDto.UserId));

        var query = new GetAssignsByTemplateRequest.Query(templateId, queryOptions);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items.First().UserFullName.Should().Be("John Doe");
        result.Items.First().Department.Should().Be("IT");
    }

    [Fact]
    public async Task HandleWhenTemplateExistsButNoActiveAgreementsReturnsEmpty()
    {
        // Arrange
        const int templateId = 1;
        _dbContextMock.Setup(c => c.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { new() { Id = templateId, Name = "test", InquiryType = InquiryType.Vacation } }
                .BuildMockDbSet().Object);

        _dbContextMock.Setup(c => c.UserAgreements)
            .Returns(new List<UserAgreementEntity>().BuildMockDbSet().Object);

        var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<AssignedUserDto>(
            new Dictionary<string, string?>(), nameof(AssignedUserDto.UserId));

        var query = new GetAssignsByTemplateRequest.Query(templateId, queryOptions);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenTemplateTypeIsSickDayReturnsActiveAgreements()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        const int templateId = 2;

        _dbContextMock.Setup(c => c.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { new() { Id = templateId, Name = "test", InquiryType = InquiryType.SickDay } }
                .BuildMockDbSet().Object);

        var agreements = new List<UserAgreementEntity>
    {
        new()
        {
            InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = templateId },
            DateFrom = today.AddDays(-1),
            DateTo = today.AddDays(2),
            User = new InquiriesUserEntity
            {
                FirstName = "Alice",
                LastName = "Brown",
                Email = "test",
                DepartmentId = "hr",
                AvatarFileId = Guid.CreateVersion7().ToString(),
                IsActive = true,
            }
        }
    };

        _dbContextMock.Setup(c => c.UserAgreements).Returns(agreements.BuildMockDbSet().Object);
        _identityContractMock.Setup(c => c.GetDepartmentsClassificator(_token))
            .ReturnsAsync([new("hr", "HR Department", "HR")]);
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<AssignedUserDto>(queryParameters, nameof(AssignedUserDto.UserId));

        var query = new GetAssignsByTemplateRequest.Query(templateId, queryOptions);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items.First().Department.Should().Be("HR");
    }

    [Fact]
    public async Task HandleWhenTemplateTypeIsOtherUsesUserInquiryTemplates()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        const int templateId = 3;

        _dbContextMock.Setup(c => c.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { new() { Id = templateId, Name = "test", InquiryType = InquiryType.SocialVacation } }
                .BuildMockDbSet().Object);

        var userTemplates = new List<UserInquiryTemplateEntity>
        {
            new()
            {
                InquiryTemplateId = templateId,
                DateFrom = today.AddDays(-1),
                DateTo = today.AddDays(1),
                 
                AssignedToUserId = 5,
                AssignedToUser = new InquiriesUserEntity
                {
                    FirstName = "Bob",
                    LastName = "Green",
                    Email = "test",
                    DepartmentId = "finance",
                    AvatarFileId = Guid.CreateVersion7().ToString(),
                    IsActive = true
                }
            }
        };

        _dbContextMock.Setup(c => c.UserInquiryTemplates)
            .Returns(userTemplates.BuildMockDbSet().Object);

        _identityContractMock.Setup(c => c.GetDepartmentsClassificator(_token))
            .ReturnsAsync([new("finance", "Finance Dept", "Finance")]);

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<AssignedUserDto>(queryParameters, nameof(AssignedUserDto.UserId));

        var query = new GetAssignsByTemplateRequest.Query(templateId, queryOptions);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items.First().UserFullName.Should().Be("Green Bob");
        result.Items.First().Department.Should().Be("Finance");
    }

    [Fact]
    public async Task HandleWhenAgreementHasNullDepartmentMapsDepartmentToNull()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        const int templateId = 4;

        _dbContextMock.Setup(c => c.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { new() { Id = templateId, Name = "test", InquiryType = InquiryType.Vacation } }
                .BuildMockDbSet().Object);

        var agreements = new List<UserAgreementEntity>
    {
        new()
        {
            InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = templateId },
            DateFrom = today.AddDays(-1),
            DateTo = today.AddDays(1),
            User = new InquiriesUserEntity
            {
                FirstName = "Charlie",
                LastName = "Delta",
                Email = "test",
                DepartmentId = "unknown",
                AvatarFileId = Guid.CreateVersion7().ToString(),
                IsActive = true,
            }
        }
    };

        _dbContextMock.Setup(c => c.UserAgreements).Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock.Setup(c => c.GetDepartmentsClassificator(_token))
            .ReturnsAsync([]); // department not found

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<AssignedUserDto>(queryParameters, nameof(AssignedUserDto.UserId));

        var query = new GetAssignsByTemplateRequest.Query(templateId, queryOptions);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items.First().Department.Should().BeNull();
    }

    [Fact]
    public async Task HandleWhenUserHasRussianNamesUsesRussianFullName()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        const int templateId = 5;

        _dbContextMock.Setup(c => c.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { new() { Id = templateId, Name = "test", InquiryType = InquiryType.Vacation } }
                .BuildMockDbSet().Object);

        var agreements = new List<UserAgreementEntity>
    {
        new()
        {
            InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = templateId },
            DateFrom = today.AddDays(-1),
            DateTo = today.AddDays(1),
            User = new InquiriesUserEntity
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "test",
                FirstNameRu = "Иван",
                LastNameRu = "Иванов",
                PatronymicRu = "Иванович",
                DepartmentId = "it",
                AvatarFileId = Guid.CreateVersion7().ToString(),
                IsActive = true,
            }
        }
    };

        _dbContextMock.Setup(c => c.UserAgreements).Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock.Setup(c => c.GetDepartmentsClassificator(_token))
            .ReturnsAsync([new("it", "IT Department", "IT")]);

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<AssignedUserDto>(queryParameters, nameof(AssignedUserDto.UserId));

        var query = new GetAssignsByTemplateRequest.Query(templateId, queryOptions);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Items.Should().ContainSingle();
        var user = result.Items.First();
        user.UserFullName.Should().Be("Иванов Иван (Doe John)"); // проверка русского полного имени
        user.Department.Should().Be("IT");
    }

    [Fact]
    public async Task HandleWhenUserHasOnlyEnglishNamesUsesEnglishFullName()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        const int templateId = 6;

        _dbContextMock.Setup(c => c.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { new() { Id = templateId, Name = "test", InquiryType = InquiryType.Vacation } }
                .BuildMockDbSet().Object);

        var agreements = new List<UserAgreementEntity>
    {
        new()
        {
            InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = templateId },
            DateFrom = today.AddDays(-1),
            DateTo = today.AddDays(1),
            User = new InquiriesUserEntity
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "test",
                FirstNameRu = null,
                LastNameRu = null,
                PatronymicRu = null,
                DepartmentId = "it",
                AvatarFileId = Guid.CreateVersion7().ToString(),
                IsActive = true,
            }
        }
    };

        _dbContextMock.Setup(c => c.UserAgreements).Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock.Setup(c => c.GetDepartmentsClassificator(_token))
            .ReturnsAsync([new("it", "IT Department", "IT")]);

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<AssignedUserDto>(queryParameters, nameof(AssignedUserDto.UserId));

        var query = new GetAssignsByTemplateRequest.Query(templateId, queryOptions);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Items.Should().ContainSingle();
        var user = result.Items.First();
        user.UserFullName.Should().Be("Smith Jane");
        user.Department.Should().Be("IT");
    }

    [Fact]
    public async Task HandleWhenUserHasFutureAssignShouldReturnFutureAssign()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        const int templateId = 1;

        var template = new InquiryTemplateEntity
        {
            Id = templateId,
            Name = "test",
            InquiryType = InquiryType.Vacation
        };

        _dbContextMock
            .Setup(c => c.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { template }.BuildMockDbSet().Object);

        var agreements = new List<UserAgreementEntity>
        {
            new()
            {
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = templateId },
                DateFrom = today.AddDays(10),
                DateTo = null,
                User = new InquiriesUserEntity
                {
                    LastName = "John",
                    FirstName = "Doe",
                    Email = "test",
                    DepartmentId = "it",
                    AvatarFileId = Guid.CreateVersion7().ToString(),
                    IsActive = true,
                },
                UserId = 1
            },
            new()
            {
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = templateId },
                DateFrom = today.AddDays(-2),
                DateTo = today.AddDays(-1),
                User = new InquiriesUserEntity
                {
                    LastName = "Jane",
                    FirstName = "Smith",
                    Email = "test",
                    DepartmentId = "it",
                    AvatarFileId = Guid.CreateVersion7().ToString(),
                    IsActive = true,
                },
                UserId = 2
            }
        };

        _dbContextMock
            .Setup(c => c.UserAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock
            .Setup(c => c.GetDepartmentsClassificator(_token))
            .ReturnsAsync([new DepartmentClassificationDto("it", "IT Department", "IT")]);

        var queryParams = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<AssignedUserDto>(
            queryParams, nameof(AssignedUserDto.UserId));

        var query = new GetAssignsByTemplateRequest.Query(templateId, queryOptions);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items.First().UserFullName.Should().Be("John Doe");
        result.Items.First().Department.Should().Be("IT");
    }

    [Fact]
    public async Task HandleWhenUserHasCurrentAndFutureAssignShouldReturnCureentAssign()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        const int templateId = 1;

        var template = new InquiryTemplateEntity
        {
            Id = templateId,
            Name = "test",
            InquiryType = InquiryType.Vacation
        };

        _dbContextMock
            .Setup(c => c.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { template }.BuildMockDbSet().Object);

        var agreements = new List<UserAgreementEntity>
        {
            new()
            {
                Id = 1,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = templateId },
                DateFrom = today.AddDays(2),
                DateTo = null,
                User = new InquiriesUserEntity
                {
                    LastName = "John",
                    FirstName = "Doe",
                    Email = "test",
                    DepartmentId = "it",
                    AvatarFileId = Guid.CreateVersion7().ToString(),
                    IsActive = true,
                },
                UserId = 2
            },
            new()
            {
                Id = 2,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = templateId },
                DateFrom = today.AddDays(-2),
                DateTo = today.AddDays(1),
                User = new InquiriesUserEntity
                {
                    LastName = "John",
                    FirstName = "Doe",
                    Email = "test",
                    DepartmentId = "it",
                    AvatarFileId = Guid.CreateVersion7().ToString(),
                    IsActive = true,
                },
                UserId = 2
            }
        };

        _dbContextMock
            .Setup(c => c.UserAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock
            .Setup(c => c.GetDepartmentsClassificator(_token))
            .ReturnsAsync([new DepartmentClassificationDto("it", "IT Department", "IT")]);

        var queryParams = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<AssignedUserDto>(
            queryParams, nameof(AssignedUserDto.UserId));

        var query = new GetAssignsByTemplateRequest.Query(templateId, queryOptions);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Items.Should().ContainSingle();        
        result.Items.First().UserFullName.Should().Be("John Doe");
        result.Items.First().Department.Should().Be("IT");
        result.Items.First().DateFrom.Should().Be(today.AddDays(-2));
        result.Items.First().DateTo.Should().Be(today.AddDays(1));
    }

    [Fact]
    public async Task HandleWhenUserAssignedToSeveralInquiriesAtSameDateFromShouldReturnvalid()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        const int templateId = 1;

        var template = new InquiryTemplateEntity
        {
            Id = templateId,
            Name = "test",
            InquiryType = InquiryType.Vacation
        };

        _dbContextMock
            .Setup(c => c.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { template }.BuildMockDbSet().Object);

        var agreements = new List<UserAgreementEntity>
        {
            new()
            {
                Id = 1,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = templateId },
                DateFrom = today.AddDays(2),
                DateTo = null,
                User = new InquiriesUserEntity
                {
                    LastName = "John",
                    FirstName = "Doe",
                    Email = "test",
                    DepartmentId = "it",
                    AvatarFileId = Guid.CreateVersion7().ToString(),
                    IsActive = true,
                },
                UserId = 2
            },
            new()
            {
                Id = 2,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = 32 },
                DateFrom = today.AddDays(2),
                DateTo = today.AddDays(10),
                User = new InquiriesUserEntity
                {
                    LastName = "John",
                    FirstName = "Doe",
                    Email = "test",
                    DepartmentId = "it",
                    AvatarFileId = Guid.CreateVersion7().ToString(),
                    IsActive = true,
                },
                UserId = 2
            }
        };

        _dbContextMock
            .Setup(c => c.UserAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock
            .Setup(c => c.GetDepartmentsClassificator(_token))
            .ReturnsAsync([new DepartmentClassificationDto("it", "IT Department", "IT")]);

        var queryParams = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<AssignedUserDto>(
            queryParams, nameof(AssignedUserDto.UserId));

        var query = new GetAssignsByTemplateRequest.Query(templateId, queryOptions);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items.First().UserFullName.Should().Be("John Doe");
        result.Items.First().Department.Should().Be("IT");
        result.Items.First().DateFrom.Should().Be(today.AddDays(2));
        result.Items.First().DateTo.Should().Be(null);
    }
}
