using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

public record EmployeeEventDto
{
    /// <summary>
    /// Combines Eng and Ru full names Ivan Ivanov (Иван Иванов)
    /// </summary>
    public string? FullName { get; private init; }

    public int UserId { get; private init; }
    public string? FullNameEng { get; private init; }
    public string? FullNameRu { get; private init; }
    public string? DepartmentId { get; private init; }
    public int? ResourceGroupId { get; private init; }

    public EmployeeEventType EventType { get; private init; }

    /// <summary>
    /// Date in the current days span when user will celebrate the event
    /// </summary>
    public DateOnly EventCelebrationDate { get; private init; }

    /// <summary>
    /// Date when the event first time occured (e.g. for birthday that will be when the person has born).
    /// This property can be used, for example, if the amount of anniversaries required (EventCelebrationDate.Year - OriginalEventDate.Year)
    /// </summary>
    public DateOnly OriginalEventDate { get; private init; }

    public string? Position { get; private init; }

    public EmployeeEventDto(
        int userId,
        string? fullNameEng,
        string? fullNameRu,
        string? departmentId,
        int? resourceGroupId,
        EmployeeEventType eventType,
        DateOnly eventCelebrationDate,
        DateOnly originalEventDate,
        string? position)
    {
        if (fullNameEng.IsNullOrWhiteSpace() && fullNameRu.IsNullOrWhiteSpace())
        {
            throw new ArgumentException($"Both full names (RU, ENG) are missing for the user (#{userId})");
        }

        UserId = userId;
        FullNameEng = fullNameEng.IsNullOrWhiteSpace() ? null : fullNameEng;
        FullNameRu = fullNameRu.IsNullOrWhiteSpace() ? null : fullNameRu;
        DepartmentId = departmentId;
        ResourceGroupId = resourceGroupId;
        EventType = eventType;
        EventCelebrationDate = eventCelebrationDate;
        OriginalEventDate = originalEventDate;
        Position = position;

        FullName = FullNameRu.IsNullOrWhiteSpace()
            ? FullNameEng
            : $"{FullNameRu} ({FullNameEng})";
    }
}

public enum EmployeeEventType
{
    None = 0,
    EmploymentAnniversary = 1,
    Birthday = 2
}
