import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError, isAxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { SETTINGS_CALENDAR_WORKING_HOURS_API_ENDPOINT } from '@/constants/queryPaths';
import type { WorkingHoursPerMonth } from '@/types/settings';

import { CreateCalendarTimeParams } from './types';

export const useCreateCalendarTime = (
	options?: UseMutationOptions<
		WorkingHoursPerMonth,
		AxiosError,
		CreateCalendarTimeParams
	>
) =>
	useMutation<WorkingHoursPerMonth, AxiosError, CreateCalendarTimeParams>({
		mutationFn: (params: CreateCalendarTimeParams) =>
			axios
				.post<WorkingHoursPerMonth>(
					SETTINGS_CALENDAR_WORKING_HOURS_API_ENDPOINT,
					{
						...params,
					}
				)
				.then((res) => res.data)
				.catch((error) => {
					if (isAxiosError(error)) {
						throw error;
					}
					throw new Error(SOMETHING_WENT_WRONG);
				}),
		...options,
	});
