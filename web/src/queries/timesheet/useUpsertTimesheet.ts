import {
	useMutation,
	UseMutationOptions,
	UseMutationResult,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_TIMESHEET_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import {
	TimesheetUpsertResponse,
	TrackedType,
} from '@/queries/timesheet/types';
import { ServerResponseError } from '@/queries/types';

export const useUpsertTimesheet = <T>(
	type: TrackedType,
	options?: UseMutationOptions<
		TimesheetUpsertResponse,
		AxiosError<ServerResponseError>,
		T,
		T
	>
): UseMutationResult<
	TimesheetUpsertResponse,
	AxiosError<ServerResponseError>,
	T,
	T
> =>
	useMutation<TimesheetUpsertResponse, AxiosError<ServerResponseError>, T, T>({
		...options,
		mutationFn: (params: T) =>
			axios
				.post(`${GET_TIMESHEET_API_ENDPOINT}/${type}/upsert`, params)
				.then((res) => res.data)
				.catch(handleRequestError),
	});
