using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
{
    private readonly ICreateOrUpdateUserDbQuery _createOrUpdateUserDbQuery;
    private readonly ITeamBoardDbContext _dbContext;
    private readonly IConnectionFactory _connectionFactory;

    public UserCreatedEventConsumer(
        ICreateOrUpdateUserDbQuery createOrUpdateUserDbQuery,
        ITeamBoardDbContext dbContext,
        IConnectionFactory connectionFactory)
    {
        _createOrUpdateUserDbQuery = createOrUpdateUserDbQuery;
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var @event = context.Message;
        var user = new CreateOrUpdateUserCoreDto
        {
            UserId = @event.UserId,
            FirstName = @event.FirstName,
            LastName = @event.LastName,
            FirstNameRu = @event.FirstNameRu,
            LastNameRu = @event.LastNameRu,
            PatronymicRu = @event.PatronymicRu,
            DepartmentId = @event.DepartmentId,
            Email = @event.Email,
            Color = @event.Color,
            AvatarFileId = @event.AvatarFileId,
            Position = @event.Position,
            PositionOrderWeight = @event.PositionOrderWeight,
            Location = @event.Location?.ToString(),
            ResourceGroupId = @event.ResourceGroupId,
            RegisteredLocationId = @event.RegisteredLocationId,
            HireDate = @event.HireDate,
            CountryId = @event.CountryId,
            CurrentLocation = @event.CurrentLocation,
            IsGraduateJobPlacement = @event.IsGraduateJobPlacement,
            MilitaryServiceStatus = @event.MilitaryServiceStatus,
            PersonalEmail = @event.PersonalEmail,
            PersonalEmailSecond = @event.PersonalEmailSecond,
            PersonalPhone = @event.PersonalPhone,
            Telegram = @event.Telegram,
            LinkedIn = @event.LinkedIn,
            DateOfBirth = @event.DateOfBirth,
            CreatedById = @event.CreatedById,
        };

        await _createOrUpdateUserDbQuery.Execute(user, context.CancellationToken);

        if (@event.ContractDateFrom != null)
        {
            var userContract = new UserContract
            {
                ContractFrom = @event.ContractDateFrom.Value,
                ContractTo = @event.ContractDateTo,
                UserId = @event.UserId,
                IsActive = true,
            };

            _dbContext.UserContracts.Add(userContract);
        }
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserCreatedEventConsumer>("user-created-teamboard-queue", "user-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserCreatedEventConsumer,
        UserCreatedEventConsumerDefinition>;
}
