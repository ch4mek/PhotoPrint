using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache.CacheRecords;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Resolvers;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Skills;

public class SpecializationSeniorityResolverTests
{
    private readonly Mock<ISeniorityLevelCache> _mockSeniorityLevelCache;
    private readonly SpecializationSeniorityResolver _resolver;
    private readonly List<SeniorityLevelCacheRecord> _seniorityLevels =
    [
        new() { Id = "None", Coefficient = 0, SpecializationRangeTo = 0.54 },
        new() { Id = "TraineeL1", Coefficient = 1, SpecializationRangeTo = 0.99 },
        new() { Id = "TraineeL2", Coefficient = 1, SpecializationRangeTo = 1.33 },
        new() { Id = "TraineeL3", Coefficient = 1, SpecializationRangeTo = 1.66 },
        new() { Id = "Trainee", Coefficient = 1, SpecializationRangeTo = 0 },
        new() { Id = "JuniorL1", Coefficient = 2, SpecializationRangeTo = 1.99 },
        new() { Id = "JuniorL2", Coefficient = 2, SpecializationRangeTo = 2.33 },
        new() { Id = "JuniorL3", Coefficient = 2, SpecializationRangeTo = 2.66 },
        new() { Id = "Junior", Coefficient = 2, SpecializationRangeTo = 0 },
        new() { Id = "MiddleL1", Coefficient = 3, SpecializationRangeTo = 2.99 },
        new() { Id = "MiddleL2", Coefficient = 3, SpecializationRangeTo = 3.33 },
        new() { Id = "MiddleL3", Coefficient = 3, SpecializationRangeTo = 3.66 },
        new() { Id = "Middle", Coefficient = 3, SpecializationRangeTo = 0 },
        new() { Id = "SeniorL1", Coefficient = 4, SpecializationRangeTo = 3.99 },
        new() { Id = "SeniorL2", Coefficient = 4, SpecializationRangeTo = 4.33 },
        new() { Id = "SeniorL3", Coefficient = 4, SpecializationRangeTo = 4.66 },
        new() { Id = "Senior", Coefficient = 4, SpecializationRangeTo = 0 },
        new() { Id = "LeadL1", Coefficient = 5, SpecializationRangeTo = 4.99 },
        new() { Id = "LeadL2", Coefficient = 5, SpecializationRangeTo = 5.33 },
        new() { Id = "LeadL3", Coefficient = 5, SpecializationRangeTo = 5.66 },
        new() { Id = "Lead", Coefficient = 5, SpecializationRangeTo = 0 },
        new() { Id = "ArchitectL1", Coefficient = 6, SpecializationRangeTo = 5.99 },
        new() { Id = "ArchitectL2", Coefficient = 6, SpecializationRangeTo = 6.00 },
        new() { Id = "Architect", Coefficient = 6, SpecializationRangeTo = 0 }
    ];

    public SpecializationSeniorityResolverTests()
    {
        _mockSeniorityLevelCache = new Mock<ISeniorityLevelCache>();
        _resolver = new SpecializationSeniorityResolver(_mockSeniorityLevelCache.Object);
    }

    [Fact]
    public async Task ResolveAsyncWhenDataIsValidReturnAverageSeniority()
    {
        // Arrange
        var hardSkillSeniorities = new[]
        {
            new HardSkillSeniorityDto { SeniorityLevelId = "Junior" },
            new HardSkillSeniorityDto { SeniorityLevelId = "Middle" }
        };
        var cancellationToken = CancellationToken.None;

        _mockSeniorityLevelCache
            .Setup(cache => cache.GetAllSeniorityLevels(cancellationToken))
            .ReturnsAsync(_seniorityLevels);

        // Act
        var result = await _resolver.ResolveAsync(hardSkillSeniorities, cancellationToken);

        // Assert
        result.Should().Be("JuniorL3");
    }

    [Fact]
    public async Task ResolveAsyncWhenOnlyNoneProvidedReturnNone()
    {
        // Arrange
        var hardSkillSeniorities = new[]
        {
            new HardSkillSeniorityDto { SeniorityLevelId = "None" }
        };
        var cancellationToken = CancellationToken.None;

        _mockSeniorityLevelCache
            .Setup(cache => cache.GetAllSeniorityLevels(cancellationToken))
            .ReturnsAsync(_seniorityLevels);

        // Act
        var result = await _resolver.ResolveAsync(hardSkillSeniorities, cancellationToken);

        // Assert
        result.Should().Be("None");
    }

    [Fact]
    public async Task ResolveAsyncWhenOnlyOneSeniorityProvidedReturnL2Seniority()
    {
        // Arrange
        var hardSkillSeniorities = new[]
        {
            new HardSkillSeniorityDto { SeniorityLevelId = "Junior" }
        };
        var cancellationToken = CancellationToken.None;

        _mockSeniorityLevelCache
            .Setup(cache => cache.GetAllSeniorityLevels(cancellationToken))
            .ReturnsAsync(_seniorityLevels);

        // Act
        var result = await _resolver.ResolveAsync(hardSkillSeniorities, cancellationToken);

        // Assert
        result.Should().Be("JuniorL2");
    }

    [Fact]
    public async Task ResolveAsyncWhenMultipleSameSenioritiesProvidedReturnL2Seniority()
    {
        // Arrange
        var hardSkillSeniorities = new[]
        {
            new HardSkillSeniorityDto { SeniorityLevelId = "Senior" },
            new HardSkillSeniorityDto { SeniorityLevelId = "Senior" },
            new HardSkillSeniorityDto { SeniorityLevelId = "Senior" },
            new HardSkillSeniorityDto { SeniorityLevelId = "Senior" }
        };
        var cancellationToken = CancellationToken.None;

        _mockSeniorityLevelCache
            .Setup(cache => cache.GetAllSeniorityLevels(cancellationToken))
            .ReturnsAsync(_seniorityLevels);

        // Act
        var result = await _resolver.ResolveAsync(hardSkillSeniorities, cancellationToken);

        // Assert
        result.Should().Be("SeniorL2");
    }

    [Fact]
    public async Task ResolveAsyncWhenMultipleSameAndOneLessSenioritiesProvidedReturnL1()
    {
        // Arrange
        var hardSkillSeniorities = new[]
        {
            new HardSkillSeniorityDto { SeniorityLevelId = "Lead" },
            new HardSkillSeniorityDto { SeniorityLevelId = "Architect" },
            new HardSkillSeniorityDto { SeniorityLevelId = "Architect" },
            new HardSkillSeniorityDto { SeniorityLevelId = "Architect" },
            new HardSkillSeniorityDto { SeniorityLevelId = "Architect" }
        };
        var cancellationToken = CancellationToken.None;

        _mockSeniorityLevelCache
            .Setup(cache => cache.GetAllSeniorityLevels(cancellationToken))
            .ReturnsAsync(_seniorityLevels);

        // Act
        var result = await _resolver.ResolveAsync(hardSkillSeniorities, cancellationToken);

        // Assert
        result.Should().Be("ArchitectL1");
    }
}
