using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;

namespace Modsen.Portal.TeamBoard.Requests.Calendar.Requests;

internal sealed class UpdateWorkingHoursPerMonthRequest
    : ICommandHandler<UpdateWorkingHoursPerMonthRequest.Command, WorkingHoursPerMonthDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public UpdateWorkingHoursPerMonthRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<WorkingHoursPerMonthDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var workingHoursEntity = await _teamBoardDbContext.WorkingHoursPerMonths
            .FirstOrDefaultAsync(workingHours => workingHours.Id == request.Id, cancellationToken);

        if (workingHoursEntity is null)
        {
            throw new EntityNotFoundException($"There is no working hours per month entry with Id {request.Id}");
        }

        workingHoursEntity.Hours = request.Hours;

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return new WorkingHoursPerMonthDto
        {
            Id = workingHoursEntity.Id,
            Year = workingHoursEntity.Year,
            MonthOfYear = workingHoursEntity.MonthOfYear,
            CountryId = workingHoursEntity.CountryId,
            Hours = workingHoursEntity.Hours
        };
    }

    internal sealed record Command(int Id, int Hours) : ICommand<WorkingHoursPerMonthDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Hours).GreaterThanOrEqualTo(0).LessThanOrEqualTo(248);
        }
    }
}
