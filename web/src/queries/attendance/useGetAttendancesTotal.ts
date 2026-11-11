import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import duration from 'dayjs/plugin/duration';
import buildQuery from 'odata-query';

import { ServerResponseError } from '../types';
import { convertFilterArrayToQueryParam } from '../utils/convertFilterArrayToQueryParam';
import { andFilter, containsFilter, orFilter } from '../utils/filters';
import { parseUserSearchString } from '../utils/parseUserSearchString';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { attendancesQueriesKeys } from '@/constants/queriesKeys';
import { GET_ATTENDANCES_TOTAL_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import dayjs from '@/lib/dayjs';
import { AttendanceTotalFilters } from '@/pages/Attendance/components/TotalTab/types';
import { GetAttendancesTotalResponse } from '@/types/attendances';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

dayjs.extend(duration);

const INCLUDE_FULL_HOUR_OFFSET = 1;

export const useGetAttendancesTotal = (
	top: number,
	skip: number,
	orderBy: string | undefined,
	filter: AttendanceTotalFilters,
	allDepartmentsLength: number,
	options?: PatchedQueryOptions<
		GetAttendancesTotalResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		GetAttendancesTotalResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			attendancesQueriesKeys.getAttendancesTotalData,
			skip,
			top,
			orderBy,
			filter,
		],
		queryFn: async () => {
			const { userId, user, departments, group, date, time } = filter;

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

			const timeFromMinutes =
				time?.from != null
					? dayjs.duration(Number(time.from), 'hours').asMinutes()
					: null;

			const timeToMinutes =
				time?.to != null
					? dayjs
							.duration(Number(time.to) + INCLUDE_FULL_HOUR_OFFSET, 'hours')
							.asMinutes()
					: null;

			try {
				const query = buildQuery({
					filter: {
						and: [
							userId?.trim() &&
								`cast(userId, Edm.String) eq cast('${replaceSpecialCharacters(
									userId.trim()
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
							date?.from &&
								`cast(date, Edm.Date) ge cast(${dayjs(date.from).format(
									YYYY_MM_DD_DAYJS_DATE_FORMAT
								)}, Edm.Date)`,
							date?.to &&
								`cast(date, Edm.Date) le cast(${dayjs(date.to).format(
									YYYY_MM_DD_DAYJS_DATE_FORMAT
								)}, Edm.Date)`,
							timeFromMinutes !== null &&
								`totalTimeInMinutes ge ${timeFromMinutes}`,
							timeToMinutes !== null &&
								`totalTimeInMinutes lt ${timeToMinutes}`,
						],
					},
					orderBy,
					top,
					skip: skip * top,
				});

				const res = await axios.get<GetAttendancesTotalResponse>(
					`${GET_ATTENDANCES_TOTAL_ENDPOINT}${query}`
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
