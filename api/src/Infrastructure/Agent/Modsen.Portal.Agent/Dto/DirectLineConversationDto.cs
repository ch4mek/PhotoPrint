using System.Text.Json.Serialization;

namespace Modsen.Portal.Agent.Dto
{
    internal sealed class DirectLineConversationDto
    {
        [JsonPropertyName("conversationId")]
        public string? ConversationId { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("expires_in")]
        public int? ExpiresIn { get; set; }
    }
}
