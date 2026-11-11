using System.Text.Json.Serialization;

namespace Modsen.Portal.Agent.Dto
{
    internal sealed class DirectLineReconnectDto
    {
        [JsonPropertyName("conversationId")]
        public string? ConversationId { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }
}
