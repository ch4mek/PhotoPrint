namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

public record UploadPayslipInput(
    string FileContent,
    string Period,
    string UserEmail);
