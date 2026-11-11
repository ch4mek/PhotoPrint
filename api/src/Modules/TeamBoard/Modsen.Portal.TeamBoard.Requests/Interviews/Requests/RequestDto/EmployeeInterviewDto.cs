using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.TeamBoard.Requests.Interviews.Requests.RequestDto;

public record EmployeeInterviewDto
{
    public int Id { get; init; }
    public int UserId { get; init; }

    /// <summary>
    /// Combines Eng and Ru full names Ivan Ivanov (Иван Иванов)
    /// </summary>
    public string? UserFullName { get; set; }

    public string? UserFullNameEng { get; init; }
    public string? UserFullNameRu { get; init; }
    public string? UserDepartmentId { get; init; }
    public int? UserResourceGroupId { get; init; }
    public string Specialization { get; init; }
    public DateOnly? UserInterviewDate { get; init; }
    public TimeOnly? UserInterviewFromUtc { get; init; }
    public UserInterviewStatus? Status { get; init; }
    public string? UserPosition { get; init; }

    public EmployeeInterviewDto()
    {
        Specialization = string.Empty;
    }

    public EmployeeInterviewDto(
        int id,
        int userId,
        string? fullNameEng,
        string? fullNameRu,
        string? departmentId,
        int? resourceGroupId,
        string specialization,
        DateOnly? interviewDate,
        TimeOnly? interviewFromUtc,
        UserInterviewStatus? status,
        string? userPosition)
    {
        Id = id;
        UserId = userId;
        UserFullNameEng = fullNameEng.IsNullOrWhiteSpace() ? null : fullNameEng;
        UserFullNameRu = fullNameRu.IsNullOrWhiteSpace() ? null : fullNameRu;
        UserDepartmentId = departmentId;
        UserResourceGroupId = resourceGroupId;
        Specialization = specialization;
        UserInterviewDate = interviewDate;
        UserInterviewFromUtc = interviewFromUtc;
        Status = status;
        UserPosition = userPosition;
        FillFullUserName();
    }

    public void FillFullUserName()
    {
        var fullNameRuIsEmpty = UserFullNameRu.IsNullOrWhiteSpace();
        var fullNameEngIsEmpty = UserFullNameEng.IsNullOrWhiteSpace();

        if (fullNameRuIsEmpty && !fullNameEngIsEmpty)
        {
            UserFullName = UserFullNameEng;
        }
        if (!fullNameRuIsEmpty && fullNameEngIsEmpty)
        {
            UserFullName = UserFullNameRu;
        }
        if (!fullNameRuIsEmpty && !fullNameEngIsEmpty)
        {
            UserFullName = $"{UserFullNameRu} ({UserFullNameEng})";
        }
    }
}
