import axios, { AxiosError } from 'axios';

import { TimesheetQueryKeys } from '@/constants/queriesKeys';
import { GET_TIMESHEETS_CALENDAR_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { TimesheetCalendarResponse } from './types';

export const useGetTimesheetCalendar = (
	date: string,
	options?: PatchedQueryOptions<
		TimesheetCalendarResponse[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback({
		queryKey: [TimesheetQueryKeys.timesheetCalender],
		queryFn: () =>
			axios
				.get<TimesheetCalendarResponse[]>(
					`${GET_TIMESHEETS_CALENDAR_ENDPOINT}`,
					{
						params: {
							forDate: date,
						},
					}
				)
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
