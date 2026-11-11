using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.Requests.Messages.Core.CoreDto
{
    internal class MessageCoreDto
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public int UserId { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public MessageRole Role { get; set; }

        public static MessageCoreDto Of(UserMessage message)
        {
            return new MessageCoreDto
            {
                Id = message.Id,
                Message = message.Message,
                UserId = message.UserId,
                CreatedAtUtc = message.CreatedAtUtc,
                Role = message.Role
            };
        }
    }
}
