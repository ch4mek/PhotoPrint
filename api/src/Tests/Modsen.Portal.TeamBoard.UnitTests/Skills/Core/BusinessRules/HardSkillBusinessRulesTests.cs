using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.BusinessRules;

namespace Modsen.Portal.TeamBoard.UnitTests.Skills.Core.BusinessRules;

public class HardSkillBusinessRulesTests
{
    private const int DefaultHardSkillId = 100;
    private const int DefaultSpecializationTemplateId = 10;

    [Fact]
    public void IsActiveHardSkillReturnsTrueWhenSkillIsActiveAndAssignedToTemplate()
    {
        var snapshot = CreateSnapshot();

        AssignSkillToTemplate(snapshot);

        var result = HardSkillBusinessRules.IsActiveHardSkill(snapshot);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsActiveHardSkillReturnsFalseWhenSkillIsInactive()
    {
        var snapshot = CreateSnapshot();
        snapshot.HardSkill!.IsActive = false;

        AssignSkillToTemplate(snapshot);

        var result = HardSkillBusinessRules.IsActiveHardSkill(snapshot);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsActiveHardSkillReturnsFalseWhenSkillIsNotAssignedToTemplate()
    {
        var snapshot = CreateSnapshot();

        var result = HardSkillBusinessRules.IsActiveHardSkill(snapshot);

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(DefaultSpecializationTemplateId, true)]
    [InlineData(999, false)]
    public void IsHardSkillAssignedToSpecializationTemplateReturnsExpectedResult(
        int templateId,
        bool expected)
    {
        var snapshot = CreateSnapshot();

        snapshot.HardSkill!.SpecializationTemplateHardSkills.Add(
            new SpecializationTemplateHardSkill
            {
                HardSkillId = snapshot.HardSkillId,
                SpecializationTemplateId = templateId
            });

        var result = HardSkillBusinessRules.IsHardSkillAssignedToSpecializationTemplate(snapshot);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, true, null, true)]
    [InlineData(true, false, "Trainee=", true)]
    [InlineData(false, true, "Trainee=", true)]
    [InlineData(false, true, null, false)]
    public void ShouldIncludeHardSkillReturnsCorrectResultBasedOnActivityAndSeniority(
        bool isSkillActive,
        bool isAssigned,
        string? seniorityLevelId,
        bool expected)
    {
        var snapshot = CreateSnapshot();

        snapshot.HardSkill!.IsActive = isSkillActive;
        snapshot.SeniorityLevelId = seniorityLevelId;

        if (isAssigned)
        {
            AssignSkillToTemplate(snapshot);
        }

        var result = HardSkillBusinessRules.ShouldIncludeHardSkill(snapshot);

        result.Should().Be(expected);
    }

    private static UserSkillsSnapshotSpecializationHardSkill CreateSnapshot(
        int hardSkillId = DefaultHardSkillId,
        int specializationTemplateId = DefaultSpecializationTemplateId)
    {
        return new UserSkillsSnapshotSpecializationHardSkill
        {
            HardSkillId = hardSkillId,
            SeniorityLevelId = null,
            UserSkillsSnapshotSpecialization = new UserSkillsSnapshotSpecialization
            {
                SpecializationTemplateId = specializationTemplateId
            },
            HardSkill = new HardSkill
            {
                Id = hardSkillId,
                IsActive = true,
                SpecializationTemplateHardSkills = [],
                Name = null,
                HardSkillGroupId = null
            }
        };
    }

    private static void AssignSkillToTemplate(UserSkillsSnapshotSpecializationHardSkill snapshot)
    {
        snapshot.HardSkill!.SpecializationTemplateHardSkills.Add(
            new SpecializationTemplateHardSkill
            {
                HardSkillId = snapshot.HardSkillId,
                SpecializationTemplateId =
                    snapshot.UserSkillsSnapshotSpecialization!.SpecializationTemplateId
            });
    }
}
