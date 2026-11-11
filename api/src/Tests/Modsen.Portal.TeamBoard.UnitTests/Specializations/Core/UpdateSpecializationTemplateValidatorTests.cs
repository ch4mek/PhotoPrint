using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Specializations.Core;

public class UpdateSpecializationTemplateValidatorTests
{
    private readonly Mock<IIsSpecializationNotAssignedToAnyUserBusinessRule>
        _isSpecializationNotAssignedToAnyUserBusinessRuleMock = new();
    private readonly Mock<ICanUserUpdateSpecializationTemplateBusinessRule>
        _canUserUpdateSpecializationTemplateBusinessRuleMock = new();
    private readonly Mock<IIsSpecializationTemplateNameUniqueBusinessRule>
        _isSpecializationTemplateNameUniqueBusinessRuleMock = new();
    private readonly Mock<IGetSpecializationTemplateByIdDbQuery> _getSpecializationTemplateByIdDbQueryMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly UpdateSpecializationTemplateValidator _validator;

    public UpdateSpecializationTemplateValidatorTests()
    {
        _validator = new UpdateSpecializationTemplateValidator(
            _isSpecializationNotAssignedToAnyUserBusinessRuleMock.Object,
            _canUserUpdateSpecializationTemplateBusinessRuleMock.Object,
            _isSpecializationTemplateNameUniqueBusinessRuleMock.Object,
            _getSpecializationTemplateByIdDbQueryMock.Object);
    }

    [Fact]
    public async Task ValidateAsyncWhenTemplateNotExistsThrowsEntityNotFoundException()
    {
        //Arrange
        const int id = 1;
        var requestDto = new UpdateSpecializationTemplateDto(id, "test", [], []);

        _getSpecializationTemplateByIdDbQueryMock
            .Setup(dbQuery => dbQuery.ExecuteAsync(id, _cancellationToken))
            .ReturnsAsync((SpecializationTemplate?)null);

        //Act
        var action = async () => await _validator.ValidateAsync(requestDto, _cancellationToken);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>($"Specialization by id {id} is not found");
    }

    [Fact]
    public async Task ValidateAsyncWhenTemplateNameIsNotUniqueThrowsInvalidDataAppException()
    {
        //Arrange
        const int id = 1;
        var requestDto = new UpdateSpecializationTemplateDto(id, "test", [], []);

        _getSpecializationTemplateByIdDbQueryMock
            .Setup(dbQuery => dbQuery.ExecuteAsync(id, _cancellationToken))
            .ReturnsAsync(new SpecializationTemplate() { Name = "test", });

        _isSpecializationTemplateNameUniqueBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(requestDto.Name, id, _cancellationToken))
            .ReturnsAsync(false);

        //Act
        var action = async () => await _validator.ValidateAsync(requestDto, _cancellationToken);

        //Assert
        await action.Should().ThrowAsync<InvalidDataAppException>("Specialization with the same name already exists");
    }

    [Fact]
    public async Task ValidateAsyncWhenUserCanNotUpdateTemplateThrowsActionNotAllowedException()
    {
        //Arrange
        const int id = 1;
        var requestDto = new UpdateSpecializationTemplateDto(id, "test", [], []);

        _getSpecializationTemplateByIdDbQueryMock
            .Setup(dbQuery => dbQuery.ExecuteAsync(id, _cancellationToken))
            .ReturnsAsync(new SpecializationTemplate()
            {
                Name = "test",
                CreatedById = 1
            });

        _isSpecializationTemplateNameUniqueBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(requestDto.Name, id, _cancellationToken))
            .ReturnsAsync(true);

        _canUserUpdateSpecializationTemplateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(It.IsAny<int>(), id, _cancellationToken))
            .ReturnsAsync(false);

        //Act
        var action = async () => await _validator.ValidateAsync(requestDto, _cancellationToken);

        //Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task ValidateAsyncWhenTemplateIsAssignedToAnyUserFromDepartmentsThrowsEntityConflictException()
    {
        //Arrange
        const int id = 1;
        var requestDto = new UpdateSpecializationTemplateDto(id, "test", [], []);

        _getSpecializationTemplateByIdDbQueryMock
            .Setup(dbQuery => dbQuery.ExecuteAsync(id, _cancellationToken))
            .ReturnsAsync(new SpecializationTemplate
            {
                CreatedById = 1,
                Name = "test",
                SpecializationTemplateDepartments =
                [
                    new SpecializationTemplateDepartment { DepartmentId = "Net" }
                ]
            });

        _isSpecializationTemplateNameUniqueBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(requestDto.Name, id, _cancellationToken))
            .ReturnsAsync(true);

        _canUserUpdateSpecializationTemplateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(It.IsAny<int>(), id, _cancellationToken))
            .ReturnsAsync(true);

        _isSpecializationNotAssignedToAnyUserBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(id, It.IsAny<string[]>(), _cancellationToken))
            .ReturnsAsync(false);

        //Act
        var action = async () => await _validator.ValidateAsync(requestDto, _cancellationToken);

        //Assert
        await action.Should().ThrowAsync<EntityConflictException>();
    }

    [Fact]
    public async Task ValidateAsyncWhenRequestValidNotThrowsAnyException()
    {
        //Arrange
        const int id = 1;
        var requestDto = new UpdateSpecializationTemplateDto(id, "test", [], []);

        _getSpecializationTemplateByIdDbQueryMock
            .Setup(dbQuery => dbQuery.ExecuteAsync(id, _cancellationToken))
            .ReturnsAsync(new SpecializationTemplate
            {
                CreatedById = 1,
                Name = "test",
                SpecializationTemplateDepartments =
                [
                    new SpecializationTemplateDepartment { DepartmentId = "Net" }
                ]
            });

        _isSpecializationTemplateNameUniqueBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(requestDto.Name, id, _cancellationToken))
            .ReturnsAsync(true);

        _canUserUpdateSpecializationTemplateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(It.IsAny<int>(), id, _cancellationToken))
            .ReturnsAsync(true);

        _isSpecializationNotAssignedToAnyUserBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(id, It.IsAny<string[]>(), _cancellationToken))
            .ReturnsAsync(true);

        //Act
        var action = async () => await _validator.ValidateAsync(requestDto, _cancellationToken);

        //Assert
        await action.Should().NotThrowAsync();
    }
}
