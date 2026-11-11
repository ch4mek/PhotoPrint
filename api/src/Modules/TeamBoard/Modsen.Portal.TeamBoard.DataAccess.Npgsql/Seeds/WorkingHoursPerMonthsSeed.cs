using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Seeds
{
    internal class WorkingHoursPerMonthsSeed : ISeedEntitiesProvider<TeamBoardDbContext>
    {
        private readonly TeamBoardDbContext _dbContext;

        public WorkingHoursPerMonthsSeed(TeamBoardDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            var countryBelarus = _dbContext.Countries.FirstOrDefault(x => x.Name == "Belarus");
            if (countryBelarus != null)
            {
                foreach (var workingHoursPerMonth in GetWorkingHours(countryBelarus, year: 2024))
                {
                    if (!_dbContext.WorkingHoursPerMonths.Any(x =>
                            x.Year == workingHoursPerMonth.Year &&
                            x.MonthOfYear == workingHoursPerMonth.MonthOfYear &&
                            x.CountryId == workingHoursPerMonth.CountryId))
                    {
                        _dbContext.WorkingHoursPerMonths.Add(workingHoursPerMonth);
                    }
                }
            }


            //foreach (var country in _dbContext.Countries.ToArray())
            //{
            //    foreach (var year in new [] { 2020, 2021, 2022, 2023, 2024, 2025, 2026 })
            //    {
            //        var hours = GetWorkingHours(country, year)
            //            .Select(wh =>
            //            {
            //                wh.Hours += Random.Shared.GetItems(new[] { 8, -8, 16, -16, 8, -8 }, 1).First();
            //                return wh;
            //            });
            //        _dbContext.WorkingHoursPerMonths.AddRange(hours);
            //    }

            //}

            _dbContext.SaveChanges();
        }

        private static IEnumerable<WorkingHoursPerMonth> GetWorkingHours(Country country, int year)
        {
            yield return Create(year, 01, country.Id, 168);
            yield return Create(year, 02, country.Id, 168);
            yield return Create(year, 03, country.Id, 160);
            yield return Create(year, 04, country.Id, 176);
            yield return Create(year, 05, country.Id, 160);
            yield return Create(year, 06, country.Id, 160);
            yield return Create(year, 07, country.Id, 176);
            yield return Create(year, 08, country.Id, 176);
            yield return Create(year, 09, country.Id, 168);
            yield return Create(year, 10, country.Id, 184);
            yield return Create(year, 11, country.Id, 160);
            yield return Create(year, 12, country.Id, 168);
        }

        private static WorkingHoursPerMonth Create(int year, int monthOfYear, int countryId, int hours)
        {
            return new WorkingHoursPerMonth
            {
                Year = year,
                MonthOfYear = monthOfYear,
                CountryId = countryId,
                Hours = hours
            };
        }
    }
}
