using System.Reflection;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Identity.Requests.Messages.Core.Composers
{
    internal interface IUserDataComposer
    {
        Task<Dictionary<string, object?>> ComposeAsync(string email, CancellationToken cancellationToken);
    }

    internal class UserDataComposer : IUserDataComposer
    {
        private readonly ITeamBoardContract _teamBoardContract;

        public UserDataComposer(ITeamBoardContract teamBoardContract)
        {
            _teamBoardContract = teamBoardContract;
        }

        public async Task<Dictionary<string, object?>> ComposeAsync(string email, CancellationToken cancellationToken)
        {
            var permanentInfo = await _teamBoardContract.GetPermanentUserInfo(email, cancellationToken);
            var impermanentInfo = await _teamBoardContract.GetImpermanentUserInfo(email, cancellationToken);

            return CreateDictionary([permanentInfo, impermanentInfo]);
        }

        public static Dictionary<string, object?> CreateDictionary(params object?[] objects)
        {
            var result = new Dictionary<string, object?>();

            foreach (var obj in objects.Where(o => o != null).ToList())
            {
                var props = obj!.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanRead);

                foreach (var prop in props.Where(p => p.GetValue(obj) != null))
                {
                    result[prop.Name] = prop.GetValue(obj);
                }
            }

            return result;
        }
    }
}
