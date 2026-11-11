import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_CALENDAR_EXCLUSIONS_API_ENDPOINT } from '@/constants/queryPaths';

import { CalendarExclusionsParams } from './types';

export const useUpdateCalendarExclusion = (
	options?: UseMutationOptions<
		CalendarExclusionsParams,
		AxiosError<ServerResponseError>,
		CalendarExclusionsParams
	>
) =>
	useMutation<
		CalendarExclusionsParams,
		AxiosError<ServerResponseError>,
		CalendarExclusionsParams
	>({
		...options,
		mutationFn: ({ id, ...data }) =>
			axios
				.put(`${GET_CALENDAR_EXCLUSIONS_API_ENDPOINT}/${id}`, data)
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
