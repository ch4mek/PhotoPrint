import axios, { AxiosError, isAxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { SettingsWorkingHoursQueriesKeys } from '@/constants/queriesKeys';
import { SETTINGS_CALENDAR_WORKING_HOURS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import type { WorkingHoursResponse } from '@/types/settings';

export const useGetWorkingHours = (
	year: number,
	options?: PatchedQueryOptions<WorkingHoursResponse, AxiosError>
) =>
	useQueryWithCallback<WorkingHoursResponse, AxiosError>({
		queryKey: [SettingsWorkingHoursQueriesKeys.getWorkingHours, year],
		queryFn: () =>
			axios
				.get<WorkingHoursResponse>(
					SETTINGS_CALENDAR_WORKING_HOURS_API_ENDPOINT,
					{
						params: {
							year,
						},
					}
				)
				.then((res) => res.data)
				.catch((error) => {
					if (isAxiosError(error)) {
						throw error;
					}
					throw new Error(SOMETHING_WENT_WRONG);
				}),
		refetchOnWindowFocus: false,
		...options,
	});
