using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Messages.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.Messages.Core.Loader
{
    internal interface IUserMessageLoader
    {
        Task<PageableListOfItems<MessageCoreDto>> LoadAsync(int id,
            ODataQueryOptions<MessageCoreDto> queryOptions,
            CancellationToken cancellationToken);
    }

    internal class UserMessageLoader : IUserMessageLoader
    {
        private readonly IIdentityDbContext _dbContext;
        
        public UserMessageLoader(IIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<PageableListOfItems<MessageCoreDto>> LoadAsync(int id, ODataQueryOptions<MessageCoreDto> queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.UserMessages
                .Where(message => message.UserId == id)
                .Select(m => new MessageCoreDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    Message = m.Message,
                    Role = m.Role,
                    CreatedAtUtc = m.CreatedAtUtc
                })
                .AsQueryable();

            var totalCount = queryOptions.RebuildWithoutPagination()
                .ApplyTo(query).Cast<MessageCoreDto>().Count();

            if (totalCount == 0)
            {
                return Task.FromResult(EmptyPageableListOfItems<MessageCoreDto>.Value);
            }

            var result = queryOptions
                .ApplyTo(query)
                .Cast<MessageCoreDto>()
                .ToList();

            return Task.FromResult(result.ToPageable(queryOptions, totalCount));
        }
    }
}
