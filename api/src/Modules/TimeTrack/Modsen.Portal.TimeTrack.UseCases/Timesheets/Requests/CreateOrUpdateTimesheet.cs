using FluentValidation;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Composers;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

internal sealed class CreateOrUpdateTimesheet : ICommandHandler<CreateOrUpdateTimesheet.Command, CreateOrUpdateTimesheet.Response>
{
    private readonly ITimesheetComposer _timesheetComposer;

    public CreateOrUpdateTimesheet(ITimesheetComposer timesheetComposer)
    {
        _timesheetComposer = timesheetComposer;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var timesheetCoreDto = await _timesheetComposer.Compose(request.Model, cancellationToken);
        return new Response(timesheetCoreDto);
    }


    internal sealed record Command(CreateOrUpdateTrackedTimeDto Model) :
        ICommand<Response>,
        ITransactionalRequest;

    internal record Response(TimesheetCoreDto Model);

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Model.TaskName)
                .BeEmptyOrInLength(2, 250)
                .SpacesRule();

            When(x => x.Model is UpsertFactDto, () =>
            {
                RuleFor(x => ((UpsertFactDto)x.Model).FactTrackedInMinutes)
                .GreaterThan(0)
                .LessThan(1_440)
                .WithMessage("You can only create a timesheet with a fact tracked time greater than 0 and less then 1440");
            });

            When(x => x.Model is UpsertBillableDto, () =>
            {
                RuleFor(x => ((UpsertBillableDto)x.Model).BillableTrackedInMinutes)
                .GreaterThan(0)
                .LessThan(1_440)
                .WithMessage("You can only create a timesheet with a billable tracked time greater than 0 and less then 1440");
            });
        }
    }
}
