namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache.CacheRecords;

internal record EnglishLevelCacheRecord
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public int? OrderWeight { get; init; }
}
