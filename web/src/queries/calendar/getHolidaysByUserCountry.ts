import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ServerResponseError } from '../types';

import { SettingsCalendarExclusionsQueriesKeys } from '@/constants/queriesKeys';
import { GET_CALENDAR_EXCLUSIONS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import {
	CalendarExclusions,
	CalendarExclusionsResponse,
	GetCalendarExclusionsItem,
} from './types';

export const useGetHolidaysByUserCountry = (
	countryId: number | null,
	queryOptions?: PatchedQueryOptions<
		GetCalendarExclusionsItem[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		GetCalendarExclusionsItem[],
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			SettingsCalendarExclusionsQueriesKeys.exclusionsByUserCountry,
			`${countryId}`,
		],
		queryFn: async () => {
			const query = buildQuery({
				filter: {
					and: [
						`(countryId eq ${countryId})`,
						`(dateType eq '${CalendarExclusions.Holiday}')`,
					],
				},
				skip: 0,
				top: 1000,
			});

			const res = await axios.get<CalendarExclusionsResponse>(
				`${GET_CALENDAR_EXCLUSIONS_API_ENDPOINT}/${query}`
			);

			return res.data.items;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		...queryOptions,
	});
