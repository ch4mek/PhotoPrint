using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

public record PageableOdpLabBoardInternsOutput(PageableListOfItems<OdpLabBoardInternDto> LabBoardInterns);
