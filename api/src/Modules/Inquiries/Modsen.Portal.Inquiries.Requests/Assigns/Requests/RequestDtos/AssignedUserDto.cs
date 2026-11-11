using System.Linq.Expressions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos
{
    public class AssignedUserDto
    {
        public int UserId { get; set; }
        public required string UserFullName { get; set; }
        public string? DepartmentId { get; set; }
        public string? AvatarFileId { get; set; }
        public string? Position { get; set; }
        public string? UserFirstName { get; set; }
        public string? UserLastName { get; set; }
        public DateOnly DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }

        internal static readonly Expression<Func<UserInquiryTemplateEntity, AssignedUserDto>> MapTemplate = x => new AssignedUserDto
        {
            UserId = x.AssignedToUserId,
            UserFullName = (x.AssignedToUser!.LastNameRu != null && x.AssignedToUser.FirstNameRu != null) ?
                x.AssignedToUser.LastNameRu + " " + x.AssignedToUser.FirstNameRu +
                " (" + x.AssignedToUser.LastName + " " + x.AssignedToUser.FirstName + ")"
                : x.AssignedToUser.LastName + " " + x.AssignedToUser.FirstName,

            DepartmentId = x.AssignedToUser.DepartmentId,
            Position = x.AssignedToUser.Position,
            AvatarFileId = x.AssignedToUser.AvatarFileId,
            UserFirstName = x.AssignedToUser.FirstName,
            UserLastName = x.AssignedToUser.LastName,
            DateFrom = x.DateFrom,
            DateTo = x.DateTo
        };

        internal static readonly Expression<Func<UserAgreementEntity, AssignedUserDto>> MapAgreement = x => new AssignedUserDto
        {
            UserId = x.UserId,
            UserFullName = x.User!.LastNameRu != null && x.User.FirstNameRu != null
                ? x.User.LastNameRu + " " + x.User.FirstNameRu +
                  " (" + x.User.LastName + " " + x.User.FirstName + ")"
                : x.User.LastName + " " + x.User.FirstName,
            DepartmentId = x.User.DepartmentId,
            Position = x.User.Position,
            AvatarFileId = x.User.AvatarFileId,
            UserFirstName = x.User.FirstName,
            UserLastName = x.User.LastName,
            DateFrom = x.DateFrom,
            DateTo = x.DateTo
        };
    }
}
