namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests.RequestDto;

public record CreateConversationFileDto(
    string FileId,
    string FileName,
    long FileSize);
