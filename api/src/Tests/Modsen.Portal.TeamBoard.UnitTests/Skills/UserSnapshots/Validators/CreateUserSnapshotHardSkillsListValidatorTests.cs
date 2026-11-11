using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Skills.UserSnapshots.Validators;

public class CreateUserSnapshotHardSkillsListValidatorTests
{
    private readonly CreateUserSnapshotHardSkillsListValidator _validator;
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContext = new();
    private readonly Mock<IAreAllHardSkillsListedBusinessRule> _areAllHardSkillsListedBusinessRule = new();
    private readonly Mock<IHardSkillsExistBusinessRule> _hardSkillsExistBusinessRule = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateUserSnapshotHardSkillsListValidatorTests()
    {
        _validator = new CreateUserSnapshotHardSkillsListValidator(_teamBoardDbContext.Object,
            _areAllHardSkillsListedBusinessRule.Object, _hardSkillsExistBusinessRule.Object);
    }

    [Fact]
    public async Task ValidateWhenSeniorityLevelIsNotSignedThrowInvalidDataAppException()
    {
        // Arrange
        var specialization = new CreateUserSpecializationDto
        {
            SpecializationSeniorityLevelId = "Junior"
        };

        // Act
        var act = () => _validator.ValidateAsync(specialization, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidDataAppException>().WithMessage("Specialization seniority level without sign can't be used");
    }

    [Fact]
    public async Task ValidateWhenNotAllHardSkillsAreListedThrowEntityConflictException()
    {
        // Arrange
        var specialization = new CreateUserSpecializationDto
        {
            SpecializationSeniorityLevelId = "JuniorL2",
            SpecializationTemplateId = 1,
            HardSkills =
            [
                new HardSkillSeniorityDto
                {
                    Id = 1,
                }
            ]
        };

        _areAllHardSkillsListedBusinessRule.Setup(x => x.IsCorrespondsToBusiness(
                specialization.SpecializationTemplateId, specialization.HardSkills, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var act = () => _validator.ValidateAsync(specialization, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityConflictException>().WithMessage("Not all hard skills are listed in request");
    }

    [Fact]
    public async Task ValidateWhenSeniorityLevelDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var specialization = new CreateUserSpecializationDto
        {
            SpecializationSeniorityLevelId = "UnsupportedL2",
            SpecializationTemplateId = 1,
            HardSkills =
            [
                new HardSkillSeniorityDto
                {
                    Id = 1,
                }
            ]
        };

        _areAllHardSkillsListedBusinessRule.Setup(x => x.IsCorrespondsToBusiness(
                specialization.SpecializationTemplateId, specialization.HardSkills, _cancellationToken))
            .ReturnsAsync(true);

        var seniorityLevels = new[]
        {
            new SeniorityLevel
            {
                Id = "JuniorL2",
                Description = "Junior=",
                ExternalName = "Junior=",
                InternalName = "Junior=",
                ShortName = "JL2"
            }
        };

        _teamBoardDbContext.Setup(x => x.SeniorityLevels).Returns(seniorityLevels.BuildMockDbSet().Object);

        // Act
        var act = () => _validator.ValidateAsync(specialization, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Seniority with id {specialization.SpecializationSeniorityLevelId} does not exist");
    }

    [Fact]
    public async Task ValidateWhenHardSkillsNotExistThrowException()
    {
        // Arrange
        var specialization = new CreateUserSpecializationDto
        {
            SpecializationSeniorityLevelId = "JuniorL2",
            SpecializationTemplateId = 1,
            HardSkills =
            [
                new HardSkillSeniorityDto
                {
                    Id = 1,
                }
            ]
        };

        _areAllHardSkillsListedBusinessRule.Setup(x => x.IsCorrespondsToBusiness(
                specialization.SpecializationTemplateId, specialization.HardSkills, _cancellationToken))
            .ReturnsAsync(true);

        var seniorityLevels = new[]
        {
            new SeniorityLevel
            {
                Id = "JuniorL2",
                Description = "Junior=",
                ExternalName = "Junior=",
                InternalName = "Junior=",
                ShortName = "JL2"
            }
        };

        var exception = new EntityNotFoundException("exception from inner business rule");

        _teamBoardDbContext.Setup(x => x.SeniorityLevels).Returns(seniorityLevels.BuildMockDbSet().Object);
        _hardSkillsExistBusinessRule.Setup(x => x.IsCorrespondsToBusiness(specialization.HardSkills, _cancellationToken))
            .ThrowsAsync(exception);

        // Act
        var act = () => _validator.ValidateAsync(specialization, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage(exception.Message);
    }

    [Fact]
    public async Task ValidateWhenRequestIsValidNotThrowException()
    {
        // Arrange
        var specialization = new CreateUserSpecializationDto
        {
            SpecializationSeniorityLevelId = "JuniorL2",
            SpecializationTemplateId = 1,
            HardSkills =
            [
                new HardSkillSeniorityDto
                {
                    Id = 1,
                }
            ]
        };

        _areAllHardSkillsListedBusinessRule.Setup(x => x.IsCorrespondsToBusiness(
                specialization.SpecializationTemplateId, specialization.HardSkills, _cancellationToken))
            .ReturnsAsync(true);

        var seniorityLevels = new[]
        {
            new SeniorityLevel
            {
                Id = "JuniorL2",
                Description = "Junior=",
                ExternalName = "Junior=",
                InternalName = "Junior=",
                ShortName = "JL2"
            }
        };

        _teamBoardDbContext.Setup(x => x.SeniorityLevels).Returns(seniorityLevels.BuildMockDbSet().Object);

        // Act
        var act = () => _validator.ValidateAsync(specialization, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
