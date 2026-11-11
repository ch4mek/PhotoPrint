import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError, isAxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { SETTINGS_CALENDAR_COUNTRIES_API_ENDPOINT } from '@/constants/queryPaths';
import { CalendaryCountry } from '@/types/configuration';

export const useDeleteCalendarCountry = (
	options?: UseMutationOptions<CalendaryCountry, AxiosError, number>
) =>
	useMutation<CalendaryCountry, AxiosError, number>({
		mutationFn: (id: number) =>
			axios
				.delete<CalendaryCountry>(
					`${SETTINGS_CALENDAR_COUNTRIES_API_ENDPOINT}/${id}`
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
