using FluentValidation;

namespace Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

internal sealed class QueryFilterDto
{
    public string? FilterColumn { get; set; }
    public string? FilterValue { get; set; }

    internal sealed class Validator : AbstractValidator<QueryFilterDto>
    {
        public Validator()
        {
            RuleFor(optionsDto => optionsDto.FilterValue)
                .NotNull()
                .When(optionsDto => optionsDto.FilterColumn is not null);
        }
    }
}
