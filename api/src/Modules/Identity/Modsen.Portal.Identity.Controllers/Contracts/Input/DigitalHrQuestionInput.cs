using System.Text.Json.Serialization;
using Modsen.Portal.Core.Api.Converters;

namespace Modsen.Portal.Identity.Controllers.Contracts.Input
{
    internal class DigitalHrQuestionInput
    {
        [JsonConverter(typeof(SideTrimmingConverter))]
        public required string Question { get; set; }
    }
}
