using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests;

internal sealed class UpdateKpiColorsRequest : ICommandHandler<UpdateKpiColorsRequest.Command, List<KpiColorDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public UpdateKpiColorsRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<List<KpiColorDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        var colors = await _teamBoardDbContext.KpiColors.AsNoTracking().ToListAsync(cancellationToken);
        if (colors.Count != request.UpdateDtos.Count)
        {
            throw new InvalidDataAppException("Not all colors were updated");
        }

        ValidateIntervals(request.UpdateDtos);

        var updatedColors = colors.Join(
            request.UpdateDtos,
            oldColors => oldColors.Id,
            newColors => newColors.Id,
            (oldColors, newColors) => new KpiColor
            {
                Id = oldColors.Id,
                RightIntervalBorder = newColors.RightIntervalBorder,
                Name = oldColors.Name,
                ColorOrderWeight = oldColors.ColorOrderWeight
            })
            .ToList();

        _teamBoardDbContext.KpiColors.UpdateRange(updatedColors);
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return updatedColors.Select(color => new KpiColorDto
        {
            Id = color.Id,
            Name = color.Name,
            RightIntervalBorder = color.RightIntervalBorder
        }).ToList();
    }

    private static void ValidateIntervals(List<UpdateKpiColorDto> colors)
    {
        if (colors[^1].RightIntervalBorder != 9999)
        {
            throw new InvalidDataAppException("Last color interval border must be 9999");
        }

        for (var i = 1; i < colors.Count; i++)
        {
            var prevRight = colors[i - 1].RightIntervalBorder;
            var currentRight = colors[i].RightIntervalBorder;

            if (currentRight <= prevRight && colors[i].Id != colors[^1].Id)
            {
                throw new InvalidDataAppException("The set orange value must be greater than the set red value");
            }
        }
    }

    internal sealed record Command(List<UpdateKpiColorDto> UpdateDtos)
        : ICommand<List<KpiColorDto>>, ITransactionalRequest;

    internal sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.UpdateDtos).NotEmpty();
            RuleForEach(x => x.UpdateDtos).SetValidator(new UpdateDtoValidator());
        }

        internal sealed class UpdateDtoValidator : AbstractValidator<UpdateKpiColorDto>
        {
            public UpdateDtoValidator()
            {
                RuleFor(x => x.RightIntervalBorder)
                    .PositiveDigits()
                    .WithMessage("Invalid percent format");
            }
        }
    }


}
