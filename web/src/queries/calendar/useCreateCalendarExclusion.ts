import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_CALENDAR_EXCLUSIONS_API_ENDPOINT } from '@/constants/queryPaths';

import { CalendarExclusionsParams } from './types';

export const useCreateCalendarExclusion = (
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
		mutationFn: (data) =>
			axios
				.post(`${GET_CALENDAR_EXCLUSIONS_API_ENDPOINT}`, data)
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
