using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Contract.Contract.Output
{
    public class PermanentUserInfoOutput
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FirstNameRu { get; set; }
        public string? LastNameRu { get; set; }
        public string? PatronymicRu { get; set; }

        public string? Email { get; set; }

        public DateOnly? HireDate { get; set; }
        public DateOnly? FirstProjectInCompanyStarted { get; set; }
        public IReadOnlyCollection<ContractOutput>? Contracts { get; set; }
        public IReadOnlyCollection<ManagerOutput>? Managers { get; set; }
        public string? Departament { get; set; }
        public string? GroupName { get; set; }

        public DateOnly? DateOfBirth { get; set; }
        public string? Country { get; set; }
        public string? Location { get; set; }
        public string? Position { get; set; }
        public string? CurrentLocation { get; set; }
        public string? SeniorityLevel { get; set; }
        public string? EnglishLevel { get; set; }

        public string? PersonalEmail { get; set; }
        public string? PersonalEmailSecond { get; set; }
        public string? PersonalPhone { get; set; }
        public string? Telegram { get; set; }
        public string? LinkedIn { get; set; }

        public MilitaryServiceStatus? MilitaryServiceStatus { get; set; }
        public bool? IsGraduateJobPlacement { get; set; }
    }

    public class ManagerOutput
    {
        public string? Position { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FirstNameRu { get; set; }
        public string? LastNameRu { get; set; }
    }

    public class ContractOutput
    {
        public DateOnly DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
    }
}
