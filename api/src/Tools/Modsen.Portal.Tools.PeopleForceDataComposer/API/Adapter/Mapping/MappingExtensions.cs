using Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.PeopleForceApiModels;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Contract;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.Mapping;

internal static class MappingExtensions
{
    internal static EmployeeModel MapEmployee(this EmployeeApiModel model)
    {
        model.Position ??= new PositionApiModel();

        return new EmployeeModel
        {
            Id = model.Id,
            IsActive = model.IsActive,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            Email = model.Email,
            DepartmentId = model.Department?.Name,
            DepartmentManagerId = model.Department?.ManagerId,
            ReportingToId = model.ReportingTo?.Id,
            ReportingToEmail = model.ReportingTo?.Email,
            Position = model.Position.Name,
            DateOfBirth = model.DateOfBirth,
            HireDate = model.HiredOn,
            PersonalEmail = model.PersonalEmail,
            MobileNumber = model.MobileNumber,
            WorkPhoneNumber = model.WorkPhoneNumber
        };
    }
}
