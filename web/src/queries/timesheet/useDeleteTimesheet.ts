import {
	UseMutationOptions,
	useMutation,
	UseMutationResult,
} from '@tanstack/react-query';
import axios, { AxiosResponse, AxiosError } from 'axios';

import { GET_TIMESHEET_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

import {
	DeleteTimesheetVariables,
	TimesheetDeleteResponse,
	TrackedType,
} from './types';

export const useDeleteTimesheet = <T extends DeleteTimesheetVariables>(
	type: TrackedType,
	options?: UseMutationOptions<
		TimesheetDeleteResponse,
		AxiosError<ServerResponseError>,
		T,
		T
	>
): UseMutationResult<
	TimesheetDeleteResponse,
	AxiosError<ServerResponseError>,
	T,
	T
> =>
	useMutation<TimesheetDeleteResponse, AxiosError<ServerResponseError>, T, T>({
		...options,
		mutationFn: (params: T) =>
			axios
				.delete(`${GET_TIMESHEET_API_ENDPOINT}/${params.id}/${type}`)
				.then((res: AxiosResponse) => res.data)
				.catch(handleRequestError),
	});
