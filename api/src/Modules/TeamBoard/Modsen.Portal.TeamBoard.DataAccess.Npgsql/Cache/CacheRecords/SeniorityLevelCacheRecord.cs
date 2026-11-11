namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache.CacheRecords;

internal record SeniorityLevelCacheRecord
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public int? OrderWeight { get; init; }
    public int? SeniorityLevelScope { get; init; }
    public int? Coefficient { get; init; }
    public double? SpecializationRangeTo {  get; init; }
}
