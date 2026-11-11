using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Managements.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Managements.Requests.RequestDtos;

namespace Modsen.Portal.Finance.Requests.Managements.Core.Extensions;

internal static class MenagementsQueryExtensions
{
    public static void ApplyFiltering(this List<ManagementUserDto> list, ManagementsFilterCoreDto filter)
    {
        for (var i = list.Count - 1; i >= 0; i--)
        {
            var user = list[i];

            var userMatches = DoesUserMatch(user, filter);

            if (!userMatches)
            {
                list.RemoveAt(i);

                continue;
            }

            if (user.Projects != null && user.Projects.Count > 0)
            {
                user.Projects = BuildProjectsQuery(user, filter).ToList();

                if (user.Projects.Count == 0)
                {
                    list.RemoveAt(i);
                }
            }
            else
            {
                list.RemoveAt(i);
            }
        }
    }

    public static IEnumerable<ManagementUserDto> ApplyOrdering(this IEnumerable<ManagementUserDto> query, ManagementsOrderingCoreDto ordering)
    {
        query = ordering.Field switch
        {
            ManagementsOrderingField.UserId =>
                ordering.IsDescendingOrdering
                    ? query.OrderByDescending(x => x.Id)
                    : query.OrderBy(x => x.Id),

            ManagementsOrderingField.UserFullName => OrderUsersByName(query, ordering.IsDescendingOrdering),

            ManagementsOrderingField.DepartmentId => OrderUsersByDepartmentId(query, ordering.IsDescendingOrdering),

            ManagementsOrderingField.TrackedHours =>
                ordering.IsDescendingOrdering
                    ? query.OrderByDescending(x => x.BillableTrackedHoursAmount)
                        .Select(x =>
                        {
                            x.Projects = x.Projects
                                .OrderByDescending(p => p.BillableTrackedHours)
                                .ToList();

                            return x;
                        })
                    : query.OrderBy(x => x.BillableTrackedHoursAmount)
                        .Select(x =>
                        {
                            x.Projects = x.Projects
                                .OrderBy(p => p.BillableTrackedHours)
                                .ToList();

                            return x;
                        }),

            ManagementsOrderingField.IncomeByn =>
                ordering.IsDescendingOrdering
                    ? query.OrderByDescending(x => x.IncomeBynAmount)
                        .Select(x =>
                        {
                            x.Projects = x.Projects
                                .OrderByDescending(p => p.IncomeByn)
                                .ToList();

                            return x;
                        })
                    : query.OrderBy(x => x.IncomeBynAmount)
                        .Select(x =>
                        {
                            x.Projects = x.Projects
                                .OrderBy(p => p.IncomeByn)
                                .ToList();

                            return x;
                        }),

            ManagementsOrderingField.Kpi =>
                ordering.IsDescendingOrdering
                    ? query.OrderByDescending(x => x.Kpi)
                    : query.OrderBy(x => x.Kpi),

            _ => query
        };

        return query;
    }

    private static IEnumerable<ManagementUserDto> OrderUsersByName(IEnumerable<ManagementUserDto> users, bool isDescendingOrdering)
    {
        var usersOrderedByNameLanguage = users.OrderBy(x => string.IsNullOrEmpty(x.LastNameRu) && string.IsNullOrEmpty(x.FirstNameRu));

        return isDescendingOrdering
            ? usersOrderedByNameLanguage.ThenByDescending(x => BuildFullName(x.LastNameRu, x.FirstNameRu, x.LastName, x.FirstName))
            : usersOrderedByNameLanguage.ThenBy(x => BuildFullName(x.LastNameRu, x.FirstNameRu, x.LastName, x.FirstName));
    }

    private static IEnumerable<ManagementUserDto> OrderUsersByDepartmentId(IEnumerable<ManagementUserDto> query, bool isDescendingOrdering)
    {
        return isDescendingOrdering
            ? query.OrderByDescending(x => x.DepartmentId == null ? 1 : 0)
                .ThenByDescending(x => x.DepartmentId)
            : query.OrderBy(x => x.DepartmentId == null ? 1 : 0)
                .ThenBy(x => x.DepartmentId);
    }

    private static string BuildFullName(string? lastNameRu, string? firstNameRu, string lastName, string firstName)
    {
        if (lastNameRu != null || firstNameRu != null)
        {
            var ruName = $"{lastNameRu ?? ""} {firstNameRu ?? ""}".Trim();
            var enName = $"{lastName} {firstName}".Trim();

            return $"{ruName} ({enName})";
        }

        return $"{lastName} {firstName}".Trim();
    }

    private static bool DoesUserMatch(ManagementUserDto user, ManagementsFilterCoreDto filter)
    {
        var userMatches = true;

        if (!filter.UserFullName.IsNullOrEmpty())
        {
            userMatches = userMatches &&
                filter.UserFullName.Contains(
                    BuildFullName(user.LastNameRu, user.FirstNameRu, user.LastName, user.FirstName),
                    StringComparison.OrdinalIgnoreCase);
        }

        if (filter.UserId != null)
        {
            userMatches = userMatches && user.Id == filter.UserId;
        }

        if (filter.DepartmentIds is { Count: > 0 })
        {
            userMatches = userMatches &&
                user.DepartmentId != null &&
                filter.DepartmentIds.Contains(user.DepartmentId);
        }

        return userMatches;
    }

    private static IEnumerable<ManagementProjectDto> BuildProjectsQuery(ManagementUserDto user, ManagementsFilterCoreDto filter)
    {
        var projectQuery = user.Projects.AsEnumerable();

        if (!filter.ProjectName.IsNullOrEmpty())
        {
            projectQuery = projectQuery.Where(p =>
                p.ProjectName.Contains(filter.ProjectName.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!filter.ClientName.IsNullOrEmpty())
        {
            projectQuery = projectQuery.Where(p =>
                p.ClientName.Contains(filter.ClientName.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!filter.ClientLegalEntity.IsNullOrEmpty())
        {
            projectQuery = projectQuery.Where(p =>
                p.ClientLegalEntity != null &&
                p.ClientLegalEntity.Contains(filter.ClientLegalEntity.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return projectQuery;
    }
}
