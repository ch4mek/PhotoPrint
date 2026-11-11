using System.Text.Json.Serialization;

namespace Modsen.Portal.Agent.Dto
{
    internal sealed class ActivitySetDto
    {
        [JsonPropertyName("activities")]
        public List<DirectLineActivityDto>? Activities { get; set; }

        [JsonPropertyName("watermark")]
        public string? Watermark { get; set; }
    }

    internal sealed class DirectLineActivityDto
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("from")]
        public ChannelAccountDto? From { get; set; }
    }

    internal sealed class ChannelAccountDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}
