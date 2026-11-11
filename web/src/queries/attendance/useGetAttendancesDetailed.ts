import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ServerResponseError } from '../types';
import { convertDirectionToQueryParam } from '../utils/convertDirectionToQueryParam';
import { convertFilterArrayToQueryParam } from '../utils/convertFilterArrayToQueryParam';
import { andFilter, containsFilter, orFilter } from '../utils/filters';
import { parseUserSearchString } from '../utils/parseUserSearchString';

import { attendancesQueriesKeys } from '@/constants/queriesKeys';
import { GET_ATTENDANCES_DETAILED_ENDPOINT } from '@/constants/queryPaths';
import { FULL_HOURS_IN_DAY, TIMEZONE_OFFSET } from '@/constants/time';
import { getEndOfDayUTC, getStartOfDayUTC } from '@/helpers/dateRangeHelpers';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { AttendanceDetailedFilters } from '@/pages/Attendance/components/DetailedTab/types';
import { GetAttendancesDetailedResponse } from '@/types/attendances';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

const buildTimeFilters = (
	timeFrom: string | null,
	timeTo: string | null
): string[] => {
	const hasFrom = timeFrom !== null;
	const hasTo = timeTo !== null;

	if (!hasFrom && !hasTo) {
		return [];
	}

	const fromNum = hasFrom ? Number(timeFrom) : null;
	const toNum = hasTo ? Number(timeTo) : null;

	const crossesMidnight = fromNum !== null && toNum !== null && fromNum > toNum;

	const localHourExpression = `((hour(checkedAtUtc) add ${TIMEZONE_OFFSET}) mod ${FULL_HOURS_IN_DAY})`;

	if (crossesMidnight) {
		const filter = `(${localHourExpression} ge ${fromNum} or ${localHourExpression} le ${toNum})`;

		return [filter];
	}

	const filters: string[] = [];

	if (fromNum !== null) {
		const filter = `${localHourExpression} ge ${fromNum}`;
		filters.push(filter);
	}

	if (toNum !== null) {
		const filter = `${localHourExpression} le ${toNum}`;
		filters.push(filter);
	}

	return filters;
};

export const useGetAttendancesDetailed = (
	top: number,
	skip: number,
	orderBy: string | undefined,
	filter: AttendanceDetailedFilters,
	allDepartmentsLength: number,
	options?: PatchedQueryOptions<
		GetAttendancesDetailedResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		GetAttendancesDetailedResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			attendancesQueriesKeys.getAttendancesDetailedData,
			skip,
			top,
			orderBy,
			filter,
		],
		queryFn: async () => {
			const {
				id,
				user,
				departments,
				group,
				direction,
				door,
				location,
				date,
				time,
			} = filter;

			const parsedUser = user?.trim()
				? parseUserSearchString(user.trim())
				: null;

			let userFilter = null;

			if (parsedUser) {
				const { russianName, englishName, fallbackTerm } = parsedUser;

				if (russianName && englishName) {
					userFilter = andFilter([
						containsFilter('userFullNameRu', russianName),
						containsFilter('userFullName', englishName),
					]);
				} else if (russianName) {
					userFilter = containsFilter('userFullNameRu', russianName);
				} else if (englishName) {
					userFilter = containsFilter('userFullName', englishName);
				} else if (fallbackTerm) {
					userFilter = orFilter([
						containsFilter('userFullName', fallbackTerm),
						containsFilter('userFullNameRu', fallbackTerm),
					]);
				}
			}

			try {
				const query = buildQuery({
					filter: {
						and: [
							id?.trim() &&
								`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
									id.trim()
								)}', Edm.String)`,
							userFilter,
							departments?.length && {
								userDepartmentId: convertFilterArrayToQueryParam(
									departments,
									allDepartmentsLength
								),
							},
							group?.trim() &&
								`contains(tolower(userResourceGroupName), tolower('${replaceSpecialCharacters(
									group.trim()
								)}'))`,
							direction?.length && {
								direction: { in: convertDirectionToQueryParam(direction) },
							},
							door?.trim() &&
								`contains(tolower(door), tolower('${replaceSpecialCharacters(
									door.trim()
								)}'))`,
							location?.trim() &&
								`contains(tolower(location), tolower('${replaceSpecialCharacters(
									location.trim()
								)}'))`,
							date?.from &&
								`cast(checkedAtUtc, Edm.DateTimeOffset) ge cast(${getStartOfDayUTC(
									date.from
								)}, Edm.DateTimeOffset)`,
							date?.to &&
								`cast(checkedAtUtc, Edm.DateTimeOffset) le cast(${getEndOfDayUTC(
									date.to
								)}, Edm.DateTimeOffset)`,
							...buildTimeFilters(time?.from ?? null, time?.to ?? null),
						],
					},
					orderBy,
					top,
					skip: skip * top,
				});

				const res = await axios.get<GetAttendancesDetailedResponse>(
					`${GET_ATTENDANCES_DETAILED_ENDPOINT}${query}`
				);

				return res.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},

		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		refetchOnMount: true,
		...options,
	});
