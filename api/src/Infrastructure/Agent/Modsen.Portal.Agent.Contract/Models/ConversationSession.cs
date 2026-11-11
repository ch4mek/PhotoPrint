namespace Modsen.Portal.Agent.Contract.Models
{
    public sealed class ConversationSession
    {
        public required string ConversationId { get; set; }
        public required string Token { get; set; }
        public DateTimeOffset ExpiresAtUtc { get; set; }
        public string? Watermark { get; set; }
    }
}
