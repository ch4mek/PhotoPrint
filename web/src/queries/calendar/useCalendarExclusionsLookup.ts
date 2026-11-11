import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SettingsCalendarExclusionsQueriesKeys } from '@/constants/queriesKeys';
import { GET_CALENDAR_EXCLUSIONS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { GetCalendarExclusionsLookupItem } from './types';

export const useCalendarExclusionsLookup = (
	queryOptions?: PatchedQueryOptions<
		Array<GetCalendarExclusionsLookupItem>,
		AxiosError
	>
) =>
	useQueryWithCallback<Array<GetCalendarExclusionsLookupItem>, AxiosError>({
		...queryOptions,
		queryKey: [SettingsCalendarExclusionsQueriesKeys.exclusionsLookup],
		queryFn: () =>
			axios
				.get<Array<GetCalendarExclusionsLookupItem>>(
					`${GET_CALENDAR_EXCLUSIONS_API_ENDPOINT}/lookup`
				)
				.then((res) => res.data),
		placeholderData: keepPreviousData,

		refetchOnWindowFocus: false,
	});
