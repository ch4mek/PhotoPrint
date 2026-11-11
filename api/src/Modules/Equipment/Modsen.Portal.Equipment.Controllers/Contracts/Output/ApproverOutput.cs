using Modsen.Portal.Core.Utils;
using Modsen.Portal.Equipment.Requests.Dto;

namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

public class ApproverOutput
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public string? FullNameRu { get; set; }

    internal static ApproverOutput? Of(ApproverDto? source)
    {
        if (source == null)
        {
            return null;
        }

        return new ApproverOutput
        {
            Id = source.Id,
            FullName = ToFullName.Reverse(source.FirstName, source.LastName),
            FullNameRu = source.LastNameRu == null && source.FirstNameRu == null ?
                null :
                ToFullName.Reverse(source.FirstNameRu, source.LastNameRu).Trim(),
        };
    }
}
