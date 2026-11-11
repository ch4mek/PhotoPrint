using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Modsen.Portal.TeamBoard.Requests.UserScores.Core
{
    public interface IScoreSignatureValidator
    {
        bool IsValid(
            int userId,
            int score,
            long timestamp,
            string signature);
    }

    public class ScoreSignatureValidator : IScoreSignatureValidator
    {
        private readonly int _timestamp;
        private readonly string _secret;
        private readonly string SecretPath = "ScoreSecurity:Secret";
        private readonly string ValidTimestampInSecondsPath = "ScoreSecurity:TimestampInSeconds";

        public ScoreSignatureValidator(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            _secret = configuration[SecretPath]
                ?? throw new InvalidOperationException("Score security secret is not configured.");

            _timestamp = configuration.GetValue<int>(ValidTimestampInSecondsPath);
            if (_timestamp == default)
            {
                throw new InvalidOperationException("Timestamp for validation is not configured.");
            }
        }

        public bool IsValid(
            int userId,
            int score,
            long timestamp,
            string signature)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var time = Math.Abs(now - timestamp);
            if (time > _timestamp || time < 0)
            {
                return false;
            }

            var payload = $"{userId}:{score}:{timestamp}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret));

            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var expectedSignature = Convert.ToHexString(hash);

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSignature),
                Encoding.UTF8.GetBytes(signature));
        }
    }
}
