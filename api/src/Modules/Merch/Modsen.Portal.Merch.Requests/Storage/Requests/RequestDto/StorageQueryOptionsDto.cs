using FluentValidation;
using Modsen.Portal.Core.Merch;

namespace Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

internal sealed class StorageQueryOptionsDto
{
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }

    public IReadOnlyCollection<QueryFilterDto>? Filters { get; set; }

    public int? AmountMin { get; set; }
    public int? AmountMax { get; set; }

    public IReadOnlyCollection<StorageAvailabilityStatus>? StatusIds { get; set; }
    public IReadOnlyCollection<int>? StorageIds { get; set; }

    internal sealed class Validator : AbstractValidator<StorageQueryOptionsDto>
    {
        public Validator()
        {
            RuleFor(optionsDto => optionsDto.Filters)
                .ForEach(x =>
                    x.SetValidator(new QueryFilterDto.Validator()));
        }
    }
}
