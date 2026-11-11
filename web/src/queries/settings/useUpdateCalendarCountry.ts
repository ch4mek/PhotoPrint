import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError, isAxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { SETTINGS_CALENDAR_COUNTRIES_API_ENDPOINT } from '@/constants/queryPaths';
import { CalendaryCountry } from '@/types/configuration';

export const useUpdateCalendarCountry = (
	options?: UseMutationOptions<CalendaryCountry, AxiosError, CalendaryCountry>
) =>
	useMutation<CalendaryCountry, AxiosError, CalendaryCountry>({
		mutationFn: (params: CalendaryCountry) =>
			axios
				.put<CalendaryCountry>(`${SETTINGS_CALENDAR_COUNTRIES_API_ENDPOINT}`, {
					...params,
				})
				.then((res) => res.data)
				.catch((error) => {
					if (isAxiosError(error)) {
						throw error;
					}
					throw new Error(SOMETHING_WENT_WRONG);
				}),
		...options,
	});
