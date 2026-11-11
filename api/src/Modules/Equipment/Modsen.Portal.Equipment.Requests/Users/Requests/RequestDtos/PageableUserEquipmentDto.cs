using Modsen.Portal.Core.Utils;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;

namespace Modsen.Portal.Equipment.Requests.Users.Requests.RequestDtos;

internal class PageableUserEquipmentDto : PageableListOfItems<OdpUserAssignments> { }

