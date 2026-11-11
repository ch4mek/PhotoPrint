using FluentValidation;
using Microsoft.AspNetCore.OData.Query;

namespace Modsen.Portal.Core.UseCases.ValidationRules;

public sealed class ODataQueryOptionsValidator : AbstractValidator<ODataQueryOptions>
{
    public ODataQueryOptionsValidator()
    {
        RuleFor(x => x.RawValues.Apply).Null();
        RuleFor(x => x.RawValues.Compute).Null();
        RuleFor(x => x.RawValues.Count).Null();
        RuleFor(x => x.RawValues.DeltaToken).Null();
        RuleFor(x => x.RawValues.Expand).Null();
        RuleFor(x => x.RawValues.Format).Null();
        RuleFor(x => x.RawValues.Search).Null();
        RuleFor(x => x.RawValues.Select).Null();
        RuleFor(x => x.RawValues.SkipToken).Null();
        RuleFor(x => x.RawValues.Top).NotEmpty();
        RuleFor(x => x.RawValues.Skip).NotEmpty();
    }
}
