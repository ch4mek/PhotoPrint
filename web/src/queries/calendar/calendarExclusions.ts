import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ServerResponseError } from '../types';
import { convertFilterArrayToQueryParam } from '../utils';
import { convertDateFromToValueToQueryParam } from '../utils/convertFromToValueToQueryParam';

import { SettingsCalendarExclusionsQueriesKeys } from '@/constants/queriesKeys';
import { GET_CALENDAR_EXCLUSIONS_API_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { HolidaysFilters } from '@/pages/Settings/components/CalendarTab/components/HolidaysGrid/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { CalendarExclusionsResponse } from './types';

export const useCalendarExclusions = (
	page: number,
	rowsPerPage: number,
	order: SortDirection,
	orderByCondition: string,
	filters: HolidaysFilters,
	allCountriesAmount: number,
	allDateTypeAmount: number,
	queryOptions?: PatchedQueryOptions<
		CalendarExclusionsResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		CalendarExclusionsResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			SettingsCalendarExclusionsQueriesKeys.exclusions,
			page,
			rowsPerPage,
			order,
			orderByCondition,
			filters,
		],
		queryFn: async () => {
			const { name, date, newDate, dateType, countryId } = filters || {};

			const query = buildQuery({
				filter: {
					and: [
						name &&
							`(contains(tolower(name), tolower('${replaceSpecialCharacters(
								name
							)}')))`,
						...convertDateFromToValueToQueryParam(date, 'date'),
						...convertDateFromToValueToQueryParam(newDate, 'newDate'),
						dateType && {
							dateType: convertFilterArrayToQueryParam(
								dateType,
								allDateTypeAmount
							),
						},
						countryId && {
							countryId: convertFilterArrayToQueryParam(
								countryId,
								allCountriesAmount
							),
						},
					],
				},
				top: rowsPerPage,
				skip: page * rowsPerPage,
				orderBy:
					orderByCondition && order ? `${orderByCondition} ${order}` : '',
			});

			const res = await axios.get<CalendarExclusionsResponse>(
				`${GET_CALENDAR_EXCLUSIONS_API_ENDPOINT}/${query}`
			);

			return res.data;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		...queryOptions,
	});
